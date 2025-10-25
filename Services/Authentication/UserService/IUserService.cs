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

	/// <summary>
	/// Validates that the provided password matches the user's stored password.
	/// Uses SignInManager to enforce lockout policies and track failed attempts.
	/// </summary>
	/// <param name="user">The user whose password should be validated.</param>
	/// <param name="password">The password to validate.</param>
	/// <returns>True if the password is valid and the account is not locked out; otherwise, false.</returns>
	Task<bool> PasswordSignInAsync(IdentityUser user, string password);

	/// <summary>
	/// Validates credentials using email/username and password.
	/// Uses SignInManager to enforce lockout policies and track failed attempts.
	/// </summary>
	/// <param name="email">The email or username of the user.</param>
	/// <param name="password">The password to validate.</param>
	/// <returns>True if the credentials are valid and the account is not locked out; otherwise, false.</returns>
	Task<bool> PasswordSignInAsync(string email, string password);

	/// <summary>
	/// Creates a new user with the specified email and password.
	/// </summary>
	/// <param name="email">The email address for the new user.</param>
	/// <param name="password">The password for the new user.</param>
	/// <returns>An IdentityResult indicating success or failure.</returns>
	Task<IdentityResult> CreateUserAsync(string email, string password);

	/// <summary>
	/// Generates an email confirmation token for the specified user.
	/// </summary>
	/// <param name="user">The user to generate the token for.</param>
	/// <returns>The email confirmation token.</returns>
	Task<string> GenerateEmailConfirmationTokenAsync(IdentityUser user);

	/// <summary>
	/// Confirms a user's email address using the provided token.
	/// </summary>
	/// <param name="user">The user whose email should be confirmed.</param>
	/// <param name="token">The email confirmation token.</param>
	/// <returns>An IdentityResult indicating success or failure.</returns>
	Task<IdentityResult> ConfirmEmailAsync(IdentityUser user, string token);
}
