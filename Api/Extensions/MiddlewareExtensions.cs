namespace Api.Extensions;

public static class MiddlewareExtensions
{
	public static IApplicationBuilder ConfigureDevelopmentMiddleware(this WebApplication app)
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
		
		return app;
	}
	
	public static IApplicationBuilder ConfigureRequestPipeline(this WebApplication app)
	{
		app.UseHttpsRedirection();
		app.UseRouting();
		app.UseAuthentication();
		app.UseAuthorization();
		app.MapControllers();
		
		return app;
	}
}
