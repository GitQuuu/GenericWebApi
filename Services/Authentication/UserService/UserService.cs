using Microsoft.AspNetCore.Identity;

namespace Services.Authentication.UserService;

/// <summary>
/// Service for managing user operations, abstracting UserManager and SignInManager functionality.
/// </summary>
public partial class UserService : IUserService
{
	private readonly UserManager<IdentityUser> _userManager;
	private readonly SignInManager<IdentityUser> _signInManager;

	public UserService(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
	{
		_userManager   = userManager;
		_signInManager = signInManager;
	}
}