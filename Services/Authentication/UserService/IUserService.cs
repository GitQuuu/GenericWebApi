using Microsoft.AspNetCore.Identity;

namespace Services.Authentication.UserService;

/// <summary>
/// Service interface for user management operations.
/// </summary>
public interface IUserService
{
	/// <summary>
	/// Finds a user by their unique identifier.
	/// </summary>
	/// <param name="userId">The unique identifier of the user.</param>
	/// <returns>The IdentityUser if found; otherwise, null.</returns>
	Task<IdentityUser?> FindByIdAsync(string userId);

	/// <summary>
	/// Gets the roles assigned to a user.
	/// </summary>
	/// <param name="user">The user whose roles to retrieve.</param>
	/// <returns>A collection of role names assigned to the user.</returns>
	Task<IList<string>> GetRolesAsync(IdentityUser user);

	/// <summary>
	/// Finds a user by their external login provider information.
	/// </summary>
	/// <param name="loginProvider">The login provider (e.g., "Microsoft", "Google").</param>
	/// <param name="providerKey">The unique key from the provider.</param>
	/// <returns>The IdentityUser if found; otherwise, null.</returns>
	Task<IdentityUser?> FindByLoginAsync(string loginProvider, string providerKey);

	/// <summary>
	/// Finds a user by their email address.
	/// </summary>
	/// <param name="email">The email address to search for.</param>
	/// <returns>The IdentityUser if found; otherwise, null.</returns>
	Task<IdentityUser?> FindByEmailAsync(string email);

	/// <summary>
	/// Creates a new user.
	/// </summary>
	/// <param name="user">The user to create.</param>
	/// <returns>An IdentityResult indicating success or failure.</returns>
	Task<IdentityResult> CreateAsync(IdentityUser user);

	/// <summary>
	/// Gets the external login information for a user.
	/// </summary>
	/// <param name="user">The user whose logins to retrieve.</param>
	/// <returns>A collection of UserLoginInfo for the user.</returns>
	Task<IList<UserLoginInfo>> GetLoginsAsync(IdentityUser user);

	/// <summary>
	/// Adds an external login to a user.
	/// </summary>
	/// <param name="user">The user to add the login to.</param>
	/// <param name="login">The external login information.</param>
	/// <returns>An IdentityResult indicating success or failure.</returns>
	Task<IdentityResult> AddLoginAsync(IdentityUser user, UserLoginInfo login);
}
