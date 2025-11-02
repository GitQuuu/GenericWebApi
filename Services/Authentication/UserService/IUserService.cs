using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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

	/// <summary>
	/// Deletes a user from the system.
	/// </summary>
	/// <param name="user">The user to delete.</param>
	/// <returns>An IdentityResult indicating success or failure.</returns>
	Task<IdentityResult> DeleteAsync(IdentityUser user);

	/// <summary>
	/// Handles the process of initiating a password reset for a user.
	/// </summary>
	/// <param name="email">The email address of the user requesting the password reset.</param>
	/// <param name="ct">The cancellation token to monitor for cancellation requests.</param>
	/// <returns>An IActionResult indicating the outcome of the password reset initiation.</returns>
	Task<ServiceResult<Tuple<IdentityUser, string>>> ForgotPasswordAsync(string email, CancellationToken ct);

	/// <summary>
	/// Resets a user's password using the provided reset token and new password.
	/// </summary>
	/// <typeparam name="T">The type of result expected from the operation.</typeparam>
	/// <param name="userId">The unique identifier of the user whose password is to be reset.</param>
	/// <param name="decodedToken">The decoded reset token for the password reset process.</param>
	/// <param name="requestNewPassword">The new password to be set for the user.</param>
	/// <param name="ct">A cancellation token for canceling the operation, if needed.</param>
	/// <returns>A ServiceResult object that contains the result of the password reset operation.</returns>
	Task<ServiceResult<IdentityResult>> ResetPasswordAsync(string userId, string decodedToken, string requestNewPassword, CancellationToken ct);

	/// <summary>
	/// Checks if a user's email is confirmed.
	/// </summary>
	/// <param name="user">The user to check.</param>
	/// <returns>True if the email is confirmed; otherwise, false.</returns>
	Task<bool> IsEmailConfirmedAsync(IdentityUser user);

	/// <summary>
	/// Changes a user's password after verifying the current password.
	/// </summary>
	/// <param name="userId">The unique identifier of the user whose password is to be changed.</param>
	/// <param name="currentPassword">The user's current password for verification.</param>
	/// <param name="newPassword">The new password to be set for the user.</param>
	/// <param name="ct">A cancellation token for canceling the operation, if needed.</param>
	/// <returns>A ServiceResult object that contains the result of the password change operation.</returns>
	Task<ServiceResult<IdentityResult>> ChangePasswordAsync(string userId, string currentPassword, string newPassword, CancellationToken ct);

	/// <summary>
	/// Sets the activation status of a user account.
	/// </summary>
	/// <param name="userId">The unique identifier of the user.</param>
	/// <param name="isActive">True to activate the account, false to deactivate it.</param>
	/// <param name="ct">A cancellation token for canceling the operation, if needed.</param>
	/// <returns>A ServiceResult indicating success or failure of the operation.</returns>
	Task<ServiceResult<IdentityUser>> SetUserAccountStatusAsync(string userId, bool isActive, CancellationToken ct);
}
