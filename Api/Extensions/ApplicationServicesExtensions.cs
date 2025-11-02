using System.Reflection;
using System.Security.Claims;
using System.Text;
using DAL;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Services.Authentication;
using Services.Authentication.IdentityProviderService;
using Services.Authentication.TokenService;
using Services.Authentication.UserService;
using Services.EmailService;
using Services.ResponseService;

namespace Api.Extensions;

/// <summary>
/// Provides extension methods for configuring application services.
/// </summary>
public static class ApplicationServicesExtensions
{
	/// <summary>
	/// Configures database and Entity Framework
	/// </summary>
	public static void AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
	{
		var connectionString = configuration.GetConnectionString("DefaultConnection") 
			?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
		
		services.AddDbContext<ApplicationDbContext>(options =>
			options.UseSqlite(connectionString));
		
		services.AddDatabaseDeveloperPageExceptionFilter();
	}
	
	/// <summary>
	/// Configures ASP.NET Core Identity
	/// </summary>
	public static void AddIdentityConfiguration(this IServiceCollection services)
	{
		services.AddDefaultIdentity<IdentityUser>(options => 
				options.SignIn.RequireConfirmedAccount = true)
			.AddRoles<IdentityRole>()
			.AddEntityFrameworkStores<ApplicationDbContext>()
			.AddDefaultTokenProviders();
	}
	
	/// <summary>
	/// Configures JWT authentication with multiple schemes (Local, Entra, Google)
	/// </summary>
	public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
	{
		services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddLocalJwtBearer(configuration, environment)
			.AddEntraJwtBearer(configuration)
			.AddGoogleJwtBearer(configuration);
	}
	
	private static AuthenticationBuilder AddLocalJwtBearer(this AuthenticationBuilder builder, IConfiguration configuration, IWebHostEnvironment environment)
	{
		return builder.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
		{
			// Debug: Log configuration values
			var signingKey = configuration["Auth:Local:SigningKey"];
			var issuer = configuration["Auth:Local:Issuer"];
			var audience = configuration["Auth:Local:Audience"];
			
			Console.WriteLine($"JWT Config - Issuer: {issuer}, Audience: {audience}, SigningKey Length: {signingKey?.Length}");
			
			// Require HTTPS in production, allow HTTP in development
			options.RequireHttpsMetadata = !environment.IsDevelopment();
			options.SaveToken = true;
			options.TokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidIssuer = issuer,
				ValidateAudience = true,
				ValidAudience = audience,
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(
					Encoding.UTF8.GetBytes(signingKey!)),
				ClockSkew = TimeSpan.FromMinutes(3),
				NameClaimType = ClaimTypes.Name,
				RoleClaimType = ClaimTypes.Role,
			};
			
			// Add event handlers for debugging in development
			if (environment.IsDevelopment())
			{
				options.Events = new JwtBearerEvents
				{
					OnAuthenticationFailed = context =>
					{
						Console.WriteLine($"Authentication failed: {context.Exception.Message}");
						return Task.CompletedTask;
					},
					OnTokenValidated = context =>
					{
						Console.WriteLine("Token validated successfully");
						return Task.CompletedTask;
					},
					OnChallenge = context =>
					{
						Console.WriteLine($"OnChallenge: {context.Error}, {context.ErrorDescription}");
						return Task.CompletedTask;
					}
				};
			}
		});
	}
	
	private static AuthenticationBuilder AddEntraJwtBearer(this AuthenticationBuilder builder, IConfiguration configuration)
	{
		return builder.AddJwtBearer("Entra", options =>
		{
			options.Authority = configuration["Auth:Entra:Authority"];
			options.Audience = configuration["Auth:Entra:Audience"];
			
			options.TokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuer = false,
				NameClaimType = "email",
			};
			
			options.Events = new JwtBearerEvents
			{
				OnTokenValidated = ctx =>
				{
					var requiredTid = configuration["Auth:Entra:TenantId"];
					if (!string.IsNullOrEmpty(requiredTid))
					{
						var tid = ctx.Principal?.FindFirst("tid")?.Value;
						if (!string.Equals(tid, requiredTid, StringComparison.OrdinalIgnoreCase))
							ctx.Fail("Invalid tenant.");
					}
					
					return Task.CompletedTask;
				}
			};
		});
	}
	
	private static AuthenticationBuilder AddGoogleJwtBearer(this AuthenticationBuilder builder, IConfiguration configuration)
	{
		return builder.AddJwtBearer("Google", options =>
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
				ValidAudience = configuration["Auth:Google:ClientId"],
				ValidateLifetime = true,
				NameClaimType = "email",
				RoleClaimType = "roles"
			};
			
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
	}
	
	/// <summary>
	/// Configures Swagger/OpenAPI documentation
	/// </summary>
	public static void AddSwaggerConfiguration(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddEndpointsApiExplorer();
		
		services.AddSwaggerGen(options =>
		{
			options.SwaggerDoc("v1", new OpenApiInfo
			{
				Title = configuration["SwaggerUi:Title"],
				Version = "v1",
				Description = configuration["SwaggerUi:Description"],
				Contact = new OpenApiContact
				{
					Email = configuration["SwaggerUi:Contact:Email"],
					Name = configuration["SwaggerUi:Contact:Name"],
					Url = new Uri(configuration["SwaggerUi:Contact:Url"] ?? string.Empty),
				}
			});
			
			// Add XML comments if needed
			var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
			var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
			options.IncludeXmlComments(xmlPath);
			
			options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
			{
				Name = "Authorization",
				Type = SecuritySchemeType.Http,
				Scheme = "bearer",
				BearerFormat = "JWT",
				In = ParameterLocation.Header,
				Description = "JWT Authorization header using the Bearer scheme. Just enter your token below (no need to type 'Bearer').",
			});
			
			options.AddSecurityRequirement(new OpenApiSecurityRequirement
			{
				{
					new OpenApiSecurityScheme
					{
						Reference = new OpenApiReference
						{
							Type = ReferenceType.SecurityScheme,
							Id = "Bearer"
						}
					},
					Array.Empty<string>()
				}
			});
		});
	}
	
	/// <summary>
	/// Registers application services
	/// </summary>
	public static void AddApplicationServices(this IServiceCollection services)
	{
		// Authentication services
		services.AddScoped<IAuthenticationOrchestrator, AuthenticationOrchestrator>();
		services.AddScoped<IIdentityProviderService, IdentityProviderService>();
		services.AddScoped<ITokenService, TokenService>();
		services.AddScoped<IUserService, UserService>();
		
		// Response service
		services.AddScoped<IResponseService, ResponseService>();
		
		// Email service
		services.AddScoped<IEmailService, EmailService>();
	}
}
