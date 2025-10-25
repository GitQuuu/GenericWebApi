using DAL;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Api.Extensions;

public static class DatabaseExtensions
{
	public static void AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
	{
		var connectionString = configuration.GetConnectionString("DefaultConnection") 
			?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
		
		services.AddDbContext<ApplicationDbContext>(options =>
			options.UseSqlite(connectionString));
		
		services.AddDatabaseDeveloperPageExceptionFilter();
	}
	
	public static void AddIdentityConfiguration(this IServiceCollection services)
	{
		services.AddDefaultIdentity<IdentityUser>(options => 
				options.SignIn.RequireConfirmedAccount = true)
			.AddRoles<IdentityRole>()
			.AddEntityFrameworkStores<ApplicationDbContext>()
			.AddDefaultTokenProviders();
	}
	
	public static async Task SeedDatabaseAsync(this WebApplication app)
	{
		using var scope = app.Services.CreateScope();
		var seeder = scope.ServiceProvider.GetRequiredService<IdentitySeeder>();
		await seeder.SeedAsync();
	}
}
