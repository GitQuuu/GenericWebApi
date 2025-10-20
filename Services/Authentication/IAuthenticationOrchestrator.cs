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
	Task<IActionResult> HandleEntraGoogleAsync(CancellationToken ctx = default);
}