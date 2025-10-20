using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Authentication;

namespace Api.Controllers;

/// <summary>
/// Endpoints for authentication and authorization.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
	private readonly IAuthenticationOrchestrator _orchestrator;

	/// <inheritdoc />
	public AuthController(IAuthenticationOrchestrator orchestrator)
	{
		_orchestrator       = orchestrator;
	}

	/// <summary>
	/// Exchanges a Microsoft authentication token for a locally authenticated token.
	/// </summary>
	/// <param name="ct">A CancellationToken to observe while waiting for the operation to complete.</param>
	/// <remarks>
	/// <para># Azure Entra Configuration</para>
	/// <para>This setup uses a two-app registration model in Microsoft Entra ID:</para>
	/// <para>- `SPA_CLIENT_ID`: The client ID of the frontend SPA (e.g., React/Vite). This app initiates login and requests tokens.</para>
	/// <para>- `AAD_API_SCOPE`: The scope string that targets the backend API. It includes the API’s client ID and the permission name (e.g., `access_as_user`).</para>
	/// <para>- `TENANT`: Typically set to `common` for multi-tenant apps, or a specific tenant ID for single-tenant scenarios.</para>
	/// <para>- `REDIRECT_URI`: The URI where Azure AD redirects after authentication. Must match the SPA app registration.</para>
	/// <para>During authentication, the SPA requests a token for the API scope. Azure Entra issues a token with:</para>
	/// <para>- `aud`: The API’s client ID</para>
	/// <para>- `scp`: The requested permission (e.g., `access_as_user`)</para>
	/// <para>The backend validates this token and grants access accordingly.</para>
	/// </remarks>
	/// <returns>An IActionResult containing either a successful response with the locally authenticated token or an error if authentication fails.</returns>
	[HttpPost("ExchangeMicrosoft")]
	[Authorize(AuthenticationSchemes = "Entra")]
	public async Task<IActionResult> ExchangeMicrosoft(CancellationToken ct)
	{
		return await _orchestrator.HandleEntraLoginAsync(ct);
	}

	/// <summary>
	/// Exchanges a Google authentication token for a locally authenticated token.
	/// </summary>
	/// <param name="ct">A CancellationToken to observe while waiting for the operation to complete.</param>
	/// <remarks>
	/// <para># Google OAuth Configuration</para>
	/// <para>This endpoint uses Google's JWT Bearer authentication. The configuration requires:</para>
	/// <para>- `ClientId`: The OAuth 2.0 client ID obtained from Google Cloud Console. This identifies your application to Google.</para>
	/// <para>- `ClientSecret`: The OAuth 2.0 client secret obtained from Google Cloud Console. Used for secure communication with Google's authentication servers.</para>
	/// <para>The Google ID token is automatically validated against the configured verification parameters. Upon</para>
	/// <para>successful validation, the user's claims are extracted, and the backend exchanges</para>
	/// <para>this token for a locally authenticated token to facilitate subsequent API interactions.</para>
	/// </remarks>
	/// <returns>An IActionResult containing either a successful response with the locally authenticated token or an error if authentication fails.</returns>
	[HttpPost("ExchangeGoogle")]
	[Authorize(AuthenticationSchemes = "Google")]
	public async Task<IActionResult> ExchangeGoogle(CancellationToken ct)
	{
		return await _orchestrator.HandleEntraGoogleAsync(ct);
	}
}