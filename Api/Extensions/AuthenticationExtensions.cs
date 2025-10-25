using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Api.Extensions;

public static class AuthenticationExtensions
{
	public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddLocalJwtBearer(configuration)
			.AddEntraJwtBearer(configuration)
			.AddGoogleJwtBearer(configuration);
		
		return services;
	}
	
	private static AuthenticationBuilder AddLocalJwtBearer(this AuthenticationBuilder builder, IConfiguration configuration)
	{
		return builder.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
		{
			options.RequireHttpsMetadata = true;
			options.SaveToken = true;
			options.TokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidIssuer = configuration["Auth:Local:Issuer"],
				ValidateAudience = true,
				ValidAudience = configuration["Auth:Local:Audience"],
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(
					Encoding.UTF8.GetBytes(configuration["Auth:Local:SigningKey"]!)),
				ClockSkew = TimeSpan.FromMinutes(3),
				NameClaimType = ClaimTypes.Name,
				RoleClaimType = ClaimTypes.Role,
			};
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
}
