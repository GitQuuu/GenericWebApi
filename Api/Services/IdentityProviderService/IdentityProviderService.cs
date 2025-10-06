using Api.Services.TokenService;
using Microsoft.AspNetCore.Identity;

namespace Api.Services.IdentityProviderService;

/// <summary>
/// Service responsible for handling identity provider integrations and operations.
/// </summary>
public partial class IdentityProviderService : IIdentityProviderService
{
	private readonly UserManager<IdentityUser> _userManager;
	private readonly IConfiguration _configuration;
	private readonly ITokenService _tokenService;

	public IdentityProviderService(
		UserManager<IdentityUser> userManager,
		IConfiguration configuration,
		ITokenService tokenService)
	{
		_userManager = userManager;
		_configuration = configuration;
		_tokenService = tokenService;
	}
	
}
