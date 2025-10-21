using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Services.Authentication.UserService;

namespace Services.Authentication.IdentityProviderService;

/// <summary>
/// Service responsible for handling identity provider integrations and operations.
/// </summary>
public partial class IdentityProviderService : IIdentityProviderService
{
	private readonly IUserService _userService;
	private readonly IConfiguration _configuration;

	public IdentityProviderService(IUserService userService,
								   IConfiguration configuration)
	{
		_userService   = userService;
		_configuration = configuration;
	}
}