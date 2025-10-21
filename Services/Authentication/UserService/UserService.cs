using Microsoft.AspNetCore.Identity;

namespace Services.Authentication.UserService;

/// <summary>
/// Service for managing user operations, abstracting UserManager functionality.
/// </summary>
public class UserService : IUserService
{
	private readonly UserManager<IdentityUser> _userManager;

	public UserService(UserManager<IdentityUser> userManager)
	{
		_userManager = userManager;
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
}
