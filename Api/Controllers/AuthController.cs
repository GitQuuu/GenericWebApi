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

	[HttpPost]
	[Authorize]
	[Route("ExchangeMicrosoft")]
	public async Task<IResult> ExchangeMicrosoft(CancellationToken ct)
	{
		// Entra token already validated by the "Entra" JwtBearer scheme.
		var principal = _httpContextAccessor.HttpContext?.User;
		if (principal?.Identity?.IsAuthenticated != true)
			return Results.Unauthorized();
		
		var response = await _identityProviderService.ExchangeMicrosoftTokenAsync(principal, ct);

		return Results.Ok(new TokenResponse(response.Data.AccessToken, response.Data.ExpiresAt));
	}

	// [HttpPost]
	// [Authorize]
	// [Route("ExchangeGoogle")]
	// public async Task<IResult> ExchangeGoogle(CancellationToken ct)
	// {
	// 	// The Google ID token was validated by JwtBearer("Google")
	// 	var principal = _httpContextAccessor.HttpContext?.User;
	// 	if (principal?.Identity?.IsAuthenticated != true)
	// 		return Results.Unauthorized();
	//
	// 	// Claims from Google ID token
	// 	var sub = principal.FindFirst("sub")?.Value
	// 		   ?? principal.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
	// 	var email = principal.FindFirst("email")?.Value
	// 			 ?? principal.FindFirst(ClaimTypes.Email)?.Value;
	// 	var emailVerified =
	// 		string.Equals(principal.FindFirst("email_verified")?.Value, "true", StringComparison.OrdinalIgnoreCase);
	//
	// 	if (string.IsNullOrWhiteSpace(sub) || string.IsNullOrWhiteSpace(email))
	// 		return Results.BadRequest(new { error = "missing_claims" });
	//
	// 	// Find or create user
	// 	var user = await _userManager.FindByLoginAsync("Google", sub)
	// 			?? await _userManager.FindByEmailAsync(email);
	//
	// 	if (user is null)
	// 	{
	// 		user = new IdentityUser()
	// 		{
	// 			UserName       = email,
	// 			Email          = email,
	// 			EmailConfirmed = false,
	//
	// 			// FirstName = principal.FindFirst("given_name")?.Value,
	// 			// LastName  = principal.FindFirst("family_name")?.Value
	// 		};
	// 		var create = await _userManager.CreateAsync(user);
	// 		if (!create.Succeeded)
	// 			return Results.BadRequest(new { error = "user_create_failed", details = create.Errors.Select(e => e.Description) });
	// 	}
	//
	// 	// Link external login if missing
	// 	var logins = await _userManager.GetLoginsAsync(user);
	// 	if (!logins.Any(l => l.LoginProvider == "Google" && l.ProviderKey == sub))
	// 		await _userManager.AddLoginAsync(user, new UserLoginInfo("Google", sub, "Google"));
	//
	// 	// enforce confirmed email
	// 	if (!user.EmailConfirmed)
	// 	{
	// 		return Results.Forbid();
	// 	}
	//
	// 	var (accessToken, expiresAt) = await _tokenService.CreateForUserAsync(user);
	// 	return Results.Ok(new { accessToken, expiresAt });
	// }
}