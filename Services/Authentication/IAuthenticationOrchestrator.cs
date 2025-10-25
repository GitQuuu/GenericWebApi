using Microsoft.AspNetCore.Mvc;

namespace Services.Authentication;

/// <summary>
/// Defines methods to orchestrate different authentication flows, including handling login processes
/// for external identity providers such as Microsoft Entra ID and Google.
/// </summary>
public interface IAuthenticationOrchestrator
{
	/// <summary>
	/// Handles the login flow for Microsoft Entra ID by exchanging a Microsoft token for a local authentication token.
	/// </summary>
	/// <param name="ctx">An optional CancellationToken to observe while waiting for the task to complete.</param>
	/// <returns>A Task representing the asynchronous operation, which contains an IActionResult indicating the result of the login process.</returns>
	Task<IActionResult> HandleEntraLoginAsync(CancellationToken ctx = default);

	/// <summary>
	/// Handles the Google login flow by exchanging a Google token for a local authentication token.
	/// </summary>
	/// <param name="ctx">An optional CancellationToken to observe while waiting for the task to complete.</param>
	/// <returns>A Task representing the asynchronous operation, which contains an IActionResult indicating the result of the login process.</returns>
	Task<IActionResult> HandleGoogleLoginAsync(CancellationToken ctx = default);

	/// <summary>
	/// Handles the local login flow by validating email and password credentials.
	/// </summary>
	/// <param name="email">The user's email address.</param>
	/// <param name="password">The user's password.</param>
	/// <param name="ctx">An optional CancellationToken to observe while waiting for the task to complete.</param>
	/// <returns>A JWT on success</returns>
	Task<IActionResult> HandleLocalLoginAsync(string email, string password, CancellationToken ctx = default);

	/// <summary>
	/// Handles user registration by creating a new account and sending an email confirmation.
	/// </summary>
	/// <param name="email">The user's email address.</param>
	/// <param name="password">The user's password.</param>
	/// <param name="ctx">An optional CancellationToken to observe while waiting for the task to complete.</param>
	/// <returns>A Task representing the asynchronous operation, which contains an IActionResult indicating the result of the registration process.</returns>
	Task<IActionResult> HandleRegisterAsync(string email, string password, CancellationToken ctx = default);

	/// <summary>
	/// Handles user account activation by confirming the email address.
	/// </summary>
	/// <param name="userId">The user's unique identifier.</param>
	/// <param name="token">The email confirmation token.</param>
	/// <param name="ctx">An optional CancellationToken to observe while waiting for the task to complete.</param>
	/// <returns>A Task representing the asynchronous operation, which contains an IActionResult indicating the result of the activation process.</returns>
	Task<IActionResult> HandleActivateUserAsync(string userId, string token, CancellationToken ctx = default);
}