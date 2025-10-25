using System.Reflection;
using Microsoft.OpenApi.Models;

namespace Api.Extensions;

public static class SwaggerExtensions
{
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
				Type = SecuritySchemeType.ApiKey,
				Scheme = "Bearer",
				BearerFormat = "JWT",
				In = ParameterLocation.Header,
				Description = "Enter 'Bearer' [space] and then your token.",
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
	
	public static void UseSwaggerConfiguration(this WebApplication app)
	{
		if (app.Environment.IsDevelopment())
		{
			app.UseSwagger();
			app.UseSwaggerUI(options =>
			{
				options.RoutePrefix = string.Empty; // Swagger at root URL
				options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
			});
		}
	}
}
