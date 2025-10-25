using DAL;

namespace Api.Extensions;

public static class MiddlewareExtensions
{
	/// <summary>
	/// Seeds the database with initial data
	/// </summary>
	public static async Task SeedDatabaseAsync(this WebApplication app)
	{
		using var scope = app.Services.CreateScope();
		var seeder = scope.ServiceProvider.GetRequiredService<IdentitySeeder>();
		await seeder.SeedAsync();
	}
	
	/// <summary>
	/// Configures environment-specific middleware (CORS, migrations, HSTS)
	/// </summary>
	public static void ConfigureDevelopmentMiddleware(this WebApplication app)
	{
		if (app.Environment.IsDevelopment())
		{
			app.UseMigrationsEndPoint();
			app.UseCors(x => x
				.AllowAnyHeader()
				.AllowAnyMethod()
				.AllowAnyOrigin());
		}
		else
		{
			// The default HSTS value is 30 days. You may want to change this for production scenarios.
			app.UseHsts();
		}
	}
	
	/// <summary>
	/// Configures Swagger UI (development only)
	/// </summary>
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
	
	/// <summary>
	/// Configures the HTTP request pipeline
	/// </summary>
	public static void ConfigureRequestPipeline(this WebApplication app)
	{
		app.UseHttpsRedirection();
		app.UseRouting();
		app.UseAuthentication();
		app.UseAuthorization();
		app.MapControllers();
	}
}
