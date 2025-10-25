using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Auth;

public partial class AuthController
{
	/// <summary>
	/// Exchanges a Microsoft authentication token for a locally authenticated token.
	/// </summary>
	/// <param name="ct">A CancellationToken to observe while waiting for the operation to complete.</param>
	/// <remarks>
	/// <para># Azure Entra Configuration</para>
	/// <para>This setup uses a two-app registration model in Microsoft Entra ID:</para>
	/// <para>- `SPA_CLIENT_ID`: The client ID of the frontend SPA (e.g., React/Vite). This app initiates login and requests tokens.</para>
	/// <para>- `AAD_API_SCOPE`: The scope string that targets the backend API. It includes the API's client ID and the permission name (e.g., `access_as_user`).</para>
	/// <para>- `TENANT`: Typically set to `common` for multi-tenant apps, or a specific tenant ID for single-tenant scenarios.</para>
	/// <para>- `REDIRECT_URI`: The URI where Azure AD redirects after authentication. Must match the SPA app registration.</para>
	/// <para>During authentication, the SPA requests a token for the API scope. Azure Entra issues a token with:</para>
	/// <para>- `aud`: The API's client ID</para>
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
}
