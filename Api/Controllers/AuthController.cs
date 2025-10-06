using Api.Services.IdentityProviderService;
using Api.Services.TokenService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.ResponseService;

namespace Api.Controllers;

/// <summary>
/// Endpoints for authentication and authorization.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly IIdentityProviderService _identityProviderService;
	private readonly IResponseService _responseService;

	public AuthController(IHttpContextAccessor httpContextAccessor,
						  IIdentityProviderService identityProviderService,
						  IResponseService responseService)
	{
		_httpContextAccessor     = httpContextAccessor;
		_identityProviderService = identityProviderService;
		_responseService         = responseService;
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
	/// <para>During authentication, the SPA requests a token for the API scope. Azure AD issues a token with:</para>
	/// <para>- `aud`: The API’s client ID</para>
	/// <para>- `scp`: The requested permission (e.g., `access_as_user`)</para>
	/// <para>The backend validates this token and grants access accordingly.</para>
	/// </remarks>
	/// <returns>An IActionResult containing either a successful response with the locally authenticated token or an error if authentication fails.</returns>
	[HttpPost("ExchangeMicrosoft")]
	[Authorize(AuthenticationSchemes = "Entra")]
	public async Task<IActionResult> ExchangeMicrosoft(CancellationToken ct)
	{
		// Entra token already validated by the "Entra" JwtBearer scheme.
		var principal = _httpContextAccessor.HttpContext?.User;
		if (principal?.Identity?.IsAuthenticated != true)
		{
			return Unauthorized();
		}

		var response = await _identityProviderService.ExchangeMicrosoftTokenAsync(principal, ct);

		return await _responseService.HandleResultAsync(response);
	}

	[HttpPost("ExchangeGoogle")]
	[Authorize(AuthenticationSchemes = "Google")]
	public async Task<IActionResult> ExchangeGoogle(CancellationToken ct)
	{
		// The Google ID token was validated by JwtBearer("Google")
		var principal = _httpContextAccessor.HttpContext?.User;
		if (principal?.Identity?.IsAuthenticated != true)
		{
			return Unauthorized();
		}

		var response = await _identityProviderService.ExchangeGoogleTokenAsync(principal, ct);

		return await _responseService.HandleResultAsync(response);
	}
}