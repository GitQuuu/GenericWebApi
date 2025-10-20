using System.Reflection;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DAL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Services.Authentication;
using Services.Authentication.IdentityProviderService;
using Services.Authentication.TokenService;
using Services.ResponseService;
using IdentityProviderService = Services.Authentication.IdentityProviderService.ExchangeMicrosoftTokenAsync.IdentityProviderService;

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
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAuthenticationOrchestrator, AuthenticationOrchestrator>();
builder.Services.AddScoped<IIdentityProviderService, IdentityProviderService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IResponseService, ResponseService>();
builder.Services.AddAuthentication(options =>
               {
                   options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; //  local JWT
                   options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
               })

               // Local JWT (already in your code)
               .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
               {
                   options.RequireHttpsMetadata = true;
                   options.SaveToken            = true;
                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuer           = true,
                       ValidIssuer              = builder.Configuration["Auth:Local:Issuer"],
                       ValidateAudience         = true,
                       ValidAudience            = builder.Configuration["Auth:Local:Audience"],
                       ValidateLifetime         = true,
                       ValidateIssuerSigningKey = true,
                       IssuerSigningKey = new SymmetricSecurityKey(
                                                                   Encoding.UTF8.GetBytes(builder.Configuration["Auth:Local:SigningKey"]!)),
                       ClockSkew     = TimeSpan.FromMinutes(3),
                       NameClaimType = ClaimTypes.Name,
                       RoleClaimType = ClaimTypes.Role,
                   };
               })

               // Entra (Microsoft) scheme used only by /api/auth/exchange
               .AddJwtBearer("Entra", options =>
               {
                   options.Authority = builder.Configuration["Auth:Entra:Authority"];
                   options.Audience  = builder.Configuration["Auth:Entra:Audience"]; 

                   // Optional hardening:
                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       // If single tenant, validate exact issuer:
                       // ValidIssuer = $"https://login.microsoftonline.com/{builder.Configuration["Auth:Entra:TenantId"]}/v2.0",
                       ValidateIssuer = false,

                       // For multi-tenant, you can leave issuer flexible and check tid in events (below).
                       NameClaimType = "email",
                   };

                   // Optional: reject tokens from other tenants (multi-tenant guard)
                   options.Events = new JwtBearerEvents
                   {
                       OnTokenValidated = ctx =>
                       {
                           var requiredTid = builder.Configuration["Auth:Entra:TenantId"];
                           if (!string.IsNullOrEmpty(requiredTid))
                           {
                               var tid = ctx.Principal?.FindFirst("tid")?.Value;
                               if (!string.Equals(tid, requiredTid, StringComparison.OrdinalIgnoreCase))
                                   ctx.Fail("Invalid tenant.");
                           }

                           return Task.CompletedTask;
                       }
                   };
               })
               .AddJwtBearer("Google", options =>
               {
                   options.Authority = "https://accounts.google.com";
                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuer = true,
                       ValidIssuers = new[]
                       {
                           "https://accounts.google.com",
                           "accounts.google.com"
                       },
                       ValidateAudience = true,
                       ValidAudience    = builder.Configuration["Auth:Google:ClientId"], // your Web Client ID
                       ValidateLifetime = true,
                       NameClaimType    = "email",
                       RoleClaimType    = "roles"
                   };

                   // prevent HTML/redirect challenges on APIs
                   options.Events = new JwtBearerEvents
                   {
                       OnChallenge = ctx =>
                       {
                           ctx.HandleResponse();
                           ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
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
		options.RoutePrefix = string.Empty; // ✅ Swagger at root URL (which will be /api)
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