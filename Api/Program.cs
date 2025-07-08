using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DAL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
														options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>( options => 
															 options.SignIn.RequireConfirmedAccount = true)
	   .AddRoles<IdentityRole>()
	   .AddEntityFrameworkStores<ApplicationDbContext>()
	   .AddDefaultTokenProviders();

builder.Services.AddControllers();
// ✅ Add Swagger generator
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<IdentitySeeder>();
// ✅ Add Authentication services (e.g., JWT)
builder.Services.AddAuthentication(options =>
	   {
		   options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
		   options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
	   })
	   .AddJwtBearer(options =>
	   {
		   options.Authority = "https://emerging-sponge-52.clerk.accounts.dev";
		   options.Audience  = builder.Configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience not found in appsettings.json") ;;
		   options.RequireHttpsMetadata = false;
		   // If you're not using Authority, you can manually set the parameters
		   options.TokenValidationParameters = new TokenValidationParameters
		   {
			   ValidateIssuer = true,
			   ValidateAudience = true,
			   ValidateLifetime = true,
			   ValidateIssuerSigningKey = true,
			   IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("Jwt:SecretKey not found in appsettings.json"))),
			   ClockSkew = TimeSpan.FromMinutes(2) // Allow small time drift
		   };

		   // Optional events for logging, error handling, etc.
		   options.Events = new JwtBearerEvents
		   {
			   OnAuthenticationFailed = context =>
			   {
				   Console.WriteLine($"Authentication failed: {context.Exception}");
				   return Task.CompletedTask;
			   },
			   OnTokenValidated = context =>
			   {
				   Console.WriteLine($"Token validated for: {context.Principal.Identity?.Name}");
				   return Task.CompletedTask;
			   }
		   };
	   });

builder.Services.AddSwaggerGen(options =>
{
	options.SwaggerDoc("v1", new() { Title = "My API", Version = "v1" });

	// Add XML comments if needed
	var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
	var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
	options.IncludeXmlComments(xmlPath);
	
	options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Name         = "Authorization",
		Type         = SecuritySchemeType.ApiKey,
		Scheme       = "Bearer",
		BearerFormat = "JWT",
		In           = ParameterLocation.Header,
		Description  = "Enter 'Bearer' [space] and then your token.",
	});

	options.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme {
				Reference = new OpenApiReference {
					Type = ReferenceType.SecurityScheme,
					Id   = "Bearer"
				}
			},
			Array.Empty<string>()
		}
	});
	
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
	var seeder = scope.ServiceProvider.GetRequiredService<IdentitySeeder>();
	await seeder.SeedAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseMigrationsEndPoint();
	app.UseCors(x => 
					x.AllowAnyHeader()
					 .AllowAnyMethod()
					 .AllowAnyOrigin()
					 );
	app.UseSwagger();
	app.UseSwaggerUI(options =>
	{
		options.RoutePrefix = string.Empty; // ✅ Swagger at root URL
		options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
	});
}
else
{
	app.UseExceptionHandler("/Home/Error");

	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication(); 
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
					   name : "default",
					   pattern : "{controller=Home}/{action=Index}/{id?}")
   .WithStaticAssets();


app.Run();