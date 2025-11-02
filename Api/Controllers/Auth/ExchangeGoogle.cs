using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Auth;

public partial class AuthController
{
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
		return await _orchestrator.HandleGoogleLoginAsync(ct);
	}
}
