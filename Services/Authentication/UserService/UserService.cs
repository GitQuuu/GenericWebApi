using Microsoft.AspNetCore.Identity;

namespace Services.Authentication.UserService;

/// <summary>
/// Service for managing user operations, abstracting UserManager and SignInManager functionality.
/// </summary>
public class UserService : IUserService
{
	private readonly UserManager<IdentityUser> _userManager;
	private readonly SignInManager<IdentityUser> _signInManager;

	public UserService(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
	{
		_userManager   = userManager;
		_signInManager = signInManager;
	}

	/// <inheritdoc />
	public async Task<IdentityUser?> FindByIdAsync(string userId)
	{
		return await _userManager.FindByIdAsync(userId);
	}

	/// <inheritdoc />
	public async Task<IList<string>> GetRolesAsync(IdentityUser user)
	{
		return await _userManager.GetRolesAsync(user);
	}

	/// <inheritdoc />
	public async Task<IdentityUser?> FindByLoginAsync(string loginProvider, string providerKey)
	{
		return await _userManager.FindByLoginAsync(loginProvider, providerKey);
	}

	/// <inheritdoc />
	public async Task<IdentityUser?> FindByEmailAsync(string email)
	{
		return await _userManager.FindByEmailAsync(email);
	}

	/// <inheritdoc />
	public async Task<IdentityResult> CreateAsync(IdentityUser user)
	{
		return await _userManager.CreateAsync(user);
	}

	/// <inheritdoc />
	public async Task<IList<UserLoginInfo>> GetLoginsAsync(IdentityUser user)
	{
		return await _userManager.GetLoginsAsync(user);
	}

	/// <inheritdoc />
	public async Task<IdentityResult> AddLoginAsync(IdentityUser user, UserLoginInfo login)
	{
		return await _userManager.AddLoginAsync(user, login);
	}

	/// <inheritdoc />
	public async Task<bool> PasswordSignInAsync(IdentityUser user, string password)
	{
		var result = await _signInManager.PasswordSignInAsync(user, password, false, lockoutOnFailure : false);
		return result.Succeeded;
	}

	/// <inheritdoc />
	public async Task<bool> PasswordSignInAsync(string email, string password)
	{
		var result = await _signInManager.PasswordSignInAsync(email, password, isPersistent : false, lockoutOnFailure : false);
		return result.Succeeded;
	}

	/// <inheritdoc />
	public async Task<IdentityResult> CreateUserAsync(string email, string password)
	{
		var user = new IdentityUser
		{
			UserName = email,
			Email = email
		};
		return await _userManager.CreateAsync(user, password);
	}

	/// <inheritdoc />
	public async Task<string> GenerateEmailConfirmationTokenAsync(IdentityUser user)
	{
		return await _userManager.GenerateEmailConfirmationTokenAsync(user);
	}

	/// <inheritdoc />
	public async Task<IdentityResult> ConfirmEmailAsync(IdentityUser user, string token)
	{
		return await _userManager.ConfirmEmailAsync(user, token);
	}
}