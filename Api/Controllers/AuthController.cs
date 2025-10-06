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