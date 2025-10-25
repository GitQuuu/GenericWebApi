using DAL;
using Services.Authentication;
using Services.Authentication.IdentityProviderService;
using Services.Authentication.TokenService;
using Services.Authentication.UserService;
using Services.ResponseService;

namespace Api.Extensions;

public static class ApplicationServicesExtensions
{
	public static IServiceCollection AddApplicationServices(this IServiceCollection services)
	{
		// Authentication services
		services.AddScoped<IAuthenticationOrchestrator, AuthenticationOrchestrator>();
		services.AddScoped<IIdentityProviderService, IdentityProviderService>();
		services.AddScoped<ITokenService, TokenService>();
		services.AddScoped<IUserService, UserService>();
		
		// Response service
		services.AddScoped<IResponseService, ResponseService>();
		
		return services;
	}
}
