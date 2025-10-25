namespace Api.Extensions;

public static class MiddlewareExtensions
{
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
	
	public static void ConfigureRequestPipeline(this WebApplication app)
	{
		app.UseHttpsRedirection();
		app.UseRouting();
		app.UseAuthentication();
		app.UseAuthorization();
		app.MapControllers();
	}
}
