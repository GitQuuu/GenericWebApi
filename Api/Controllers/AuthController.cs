using Api.Services.IdentityProviderService;
using Api.Services.TokenService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

	public AuthController(IHttpContextAccessor httpContextAccessor, 
						  IIdentityProviderService identityProviderService)
	{
		_httpContextAccessor          = httpContextAccessor;
		_identityProviderService = identityProviderService;
	}

	[HttpPost("ExchangeMicrosoft")]
	[Authorize(AuthenticationSchemes = "Entra")]
	public async Task<IResult> ExchangeMicrosoft(CancellationToken ct)
	{
		// Entra token already validated by the "Entra" JwtBearer scheme.
		var principal = _httpContextAccessor.HttpContext?.User;
		if (principal?.Identity?.IsAuthenticated != true)
			return Results.Unauthorized();
		
		var response = await _identityProviderService.ExchangeMicrosoftTokenAsync(principal, ct);

		return Results.Ok(new TokenResponse(response.Data.AccessToken, response.Data.ExpiresAt));
	}

	[HttpPost("ExchangeGoogle")]
	[Authorize(AuthenticationSchemes = "Google")]
	public async Task<IResult> ExchangeGoogle(CancellationToken ct)
	{
		// The Google ID token was validated by JwtBearer("Google")
		var principal = _httpContextAccessor.HttpContext?.User;
		if (principal?.Identity?.IsAuthenticated != true)
			return Results.Unauthorized();
		
		var response = await _identityProviderService.ExchangeGoogleTokenAsync(principal, ct);
	
		return Results.Ok(new { response.Data.AccessToken, response.Data.ExpiresAt });
	}
}