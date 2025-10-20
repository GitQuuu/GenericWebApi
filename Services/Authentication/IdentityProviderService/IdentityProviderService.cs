using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Services.Authentication.IdentityProviderService;

/// <summary>
/// Service responsible for handling identity provider integrations and operations.
/// </summary>
public partial class IdentityProviderService : IIdentityProviderService
{
	private readonly UserManager<IdentityUser> _userManager;
	private readonly IConfiguration _configuration;

	public IdentityProviderService(UserManager<IdentityUser> userManager,
								   IConfiguration configuration)
	{
		_userManager   = userManager;
		_configuration = configuration;
	}
}