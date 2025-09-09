using System.Security.Claims;
using Api.Services.TokenService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Endpoints for authentication and authorization.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
	private readonly UserManager<IdentityUser> _userManager;
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly IConfiguration _configuration;
	private readonly ITokenService _tokenService;

	public AuthController(UserManager<
							  IdentityUser> userManager,
						  IHttpContextAccessor httpContextAccessor,
						  IConfiguration configuration,
						  ITokenService tokenService)
	{
		_userManager         = userManager;
		_httpContextAccessor = httpContextAccessor;
		_configuration       = configuration;
		_tokenService        = tokenService;
	}

	[HttpPost]
	[Authorize]
	[Route("ExchangeMicrosoft")]
	private async Task<IResult> ExchangeMicrosoft(CancellationToken ct)
	{
		// Entra token already validated by the "Entra" JwtBearer scheme.
		var principal = _httpContextAccessor.HttpContext?.User;
		if (principal?.Identity?.IsAuthenticated != true)
			return Results.Unauthorized();

		// Optional multi-tenant allowlist guard
		var allowAll = bool.TryParse(_configuration["Auth:Entra:AllowAllTenants"], out var a) && a;
		if (!allowAll)
		{
			var tid     = principal.FindFirst("tid")?.Value;
			var allowed = _configuration.GetSection("Auth:Entra:AllowedTenantIds").Get<string[]>() ?? Array.Empty<string>();
			if (string.IsNullOrEmpty(tid) || !allowed.Contains(tid, StringComparer.OrdinalIgnoreCase))
				return Results.Forbid();
		}

		// Extract stable identifiers from the Microsoft token
		var providerKey = principal.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier")
					   ?? principal.FindFirstValue("oid")
					   ?? principal.FindFirstValue("sub");
		var email = principal.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value
				 ?? principal.FindFirstValue("preferred_username")
				 ?? principal.FindFirstValue(ClaimTypes.Email);

		if (string.IsNullOrWhiteSpace(providerKey) || string.IsNullOrWhiteSpace(email))
			return Results.BadRequest(new { error = "missing_claims" });

		// Upsert/link local user
		var user = await _userManager.FindByLoginAsync("Microsoft", providerKey)
				?? await _userManager.FindByEmailAsync(email);

		if (user is null)
		{
			user = new IdentityUser() { UserName = email, Email = email, EmailConfirmed = false };
			var create = await _userManager.CreateAsync(user);
			if (!create.Succeeded)
				return Results.BadRequest(new { error = "user_create_failed", details = create.Errors.Select(e => e.Description) });
		}

		if (user.EmailConfirmed is false)
		{
			return Results.Forbid();
		}


		// Ensure external login mapping exists
		var logins = await _userManager.GetLoginsAsync(user);
		if (!logins.Any(l => l.LoginProvider == "Microsoft" && l.ProviderKey == providerKey))
		{
			var result = await _userManager.AddLoginAsync(user, new UserLoginInfo("Microsoft", providerKey, "Microsoft"));
		}

		// (Optional) load your domain roles/claims here and pass to token service
		var (accessToken, expiresAt) = await _tokenService.CreateForUserAsync(user /*, roles*/);

		return Results.Ok(new TokenResponse(accessToken, expiresAt));
	}

	[HttpPost]
	[Authorize]
	[Route("ExchangeGoogle")]
	private async Task<IResult> ExchangeGoogle(CancellationToken ct)
	{
		// The Google ID token was validated by JwtBearer("Google")
		var principal = _httpContextAccessor.HttpContext?.User;
		if (principal?.Identity?.IsAuthenticated != true)
			return Results.Unauthorized();

		// Claims from Google ID token
		var sub = principal.FindFirst("sub")?.Value
			   ?? principal.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
		var email = principal.FindFirst("email")?.Value
				 ?? principal.FindFirst(ClaimTypes.Email)?.Value;
		var emailVerified =
			string.Equals(principal.FindFirst("email_verified")?.Value, "true", StringComparison.OrdinalIgnoreCase);

		if (string.IsNullOrWhiteSpace(sub) || string.IsNullOrWhiteSpace(email))
			return Results.BadRequest(new { error = "missing_claims" });

		// Find or create user
		var user = await _userManager.FindByLoginAsync("Google", sub)
				?? await _userManager.FindByEmailAsync(email);

		if (user is null)
		{
			user = new IdentityUser()
			{
				UserName       = email,
				Email          = email,
				EmailConfirmed = false,

				// FirstName = principal.FindFirst("given_name")?.Value,
				// LastName  = principal.FindFirst("family_name")?.Value
			};
			var create = await _userManager.CreateAsync(user);
			if (!create.Succeeded)
				return Results.BadRequest(new { error = "user_create_failed", details = create.Errors.Select(e => e.Description) });
		}

		// Link external login if missing
		var logins = await _userManager.GetLoginsAsync(user);
		if (!logins.Any(l => l.LoginProvider == "Google" && l.ProviderKey == sub))
			await _userManager.AddLoginAsync(user, new UserLoginInfo("Google", sub, "Google"));

		// enforce confirmed email
		if (!user.EmailConfirmed)
		{
			return Results.Forbid();
		}

		var (accessToken, expiresAt) = await _tokenService.CreateForUserAsync(user);
		return Results.Ok(new { accessToken, expiresAt });
	}
}