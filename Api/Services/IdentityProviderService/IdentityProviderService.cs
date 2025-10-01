using System.Net;
using System.Security.Claims;
using Api.Services.TokenService;
using Microsoft.AspNetCore.Identity;

namespace Api.Services.IdentityProviderService;

/// <summary>
/// Service responsible for handling identity provider integrations and operations.
/// </summary>
public partial class IdentityProviderService : IIdentityProviderService
{
	private readonly UserManager<IdentityUser> _userManager;
	private readonly IConfiguration _configuration;
	private readonly ITokenService _tokenService;

	public IdentityProviderService(
		UserManager<IdentityUser> userManager,
		IConfiguration configuration,
		ITokenService tokenService)
	{
		_userManager = userManager;
		_configuration = configuration;
		_tokenService = tokenService;
	}

	public async Task<ServiceResult<TokenResponse>> ExchangeMicrosoftTokenAsync(ClaimsPrincipal? principal, CancellationToken ct = default)
	{
		if (principal?.Identity?.IsAuthenticated != true)
		{
			return new ServiceResult<TokenResponse>(
													false,
													HttpStatusCode.Unauthorized,
													"User is not authenticated");
		}

		// Optional multi-tenant allowlist guard
		var allowAll = bool.TryParse(_configuration["Auth:Entra:AllowAllTenants"], out var a) && a;
		if (!allowAll)
		{
			var tid = principal.FindFirst("tid")?.Value;
			var allowed = _configuration.GetSection("Auth:Entra:AllowedTenantIds").Get<string[]>() ?? Array.Empty<string>();
			if (string.IsNullOrEmpty(tid) || !allowed.Contains(tid, StringComparer.OrdinalIgnoreCase))
			{
				return new ServiceResult<TokenResponse>(
														false, 
														HttpStatusCode.Forbidden, 
														"Tenant is not allowed");
			}
		}

		// Extract stable identifiers from the Microsoft token
		var providerKey = principal.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier")
					   ?? principal.FindFirstValue("oid")
					   ?? principal.FindFirstValue("sub");
		var email = principal.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value
				 ?? principal.FindFirstValue("preferred_username")
				 ?? principal.FindFirstValue(ClaimTypes.Email);

		if (string.IsNullOrWhiteSpace(providerKey) || string.IsNullOrWhiteSpace(email))
		{
			return new ServiceResult<TokenResponse>(
													false, 
													HttpStatusCode.BadRequest, 
													"Missing required claims (oid/sub or email)"
													);
		}
		

		// Upsert/link local user
		var user = await _userManager.FindByLoginAsync("Microsoft", providerKey)
				?? await _userManager.FindByEmailAsync(email);

		if (user is null)
		{
			user = new IdentityUser() { UserName = email, Email = email, EmailConfirmed = false };
			var create = await _userManager.CreateAsync(user);
			if (!create.Succeeded)
			{
				var errors = string.Join(", ", create.Errors.Select(e => e.Description));
				return new ServiceResult<TokenResponse>(
														false,
														HttpStatusCode.BadRequest,
														"User creation failed: " + errors);
			}
		}

		if (user.EmailConfirmed is false)
		{
			return new ServiceResult<TokenResponse>(
													false,
													HttpStatusCode.Forbidden,
													"Email address is not confirmed");
		}

		// Ensure external login mapping exists
		var logins = await _userManager.GetLoginsAsync(user);
		if (!logins.Any(l => l.LoginProvider == "Microsoft" && l.ProviderKey == providerKey))
		{
			await _userManager.AddLoginAsync(user, new UserLoginInfo("Microsoft", providerKey, "Microsoft"));
		}

		// Generate token
		var (accessToken, expiresAt) = await _tokenService.CreateForUserAsync(user);
		var tokenResponse = new TokenResponse(accessToken, expiresAt);

		return new ServiceResult<TokenResponse>(true,
												HttpStatusCode.OK,
												"Token exchanged successfully",
												tokenResponse);
	}

	public async Task<ServiceResult<TokenResponse>> ExchangeGoogleTokenAsync(ClaimsPrincipal? principal, CancellationToken ct = default)
	{
		if (principal?.Identity?.IsAuthenticated != true)
		{
			return new ServiceResult<TokenResponse>(false,
													HttpStatusCode.Unauthorized,
													"User is not authenticated");
		}

		// Claims from Google ID token
		var sub = principal.FindFirst("sub")?.Value
			   ?? principal.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
		var email = principal.FindFirst("email")?.Value
				 ?? principal.FindFirst(ClaimTypes.Email)?.Value;
		var emailVerified =
			string.Equals(principal.FindFirst("email_verified")?.Value, "true", StringComparison.OrdinalIgnoreCase);

		if (string.IsNullOrWhiteSpace(sub) || string.IsNullOrWhiteSpace(email))
		{
			return new ServiceResult<TokenResponse>(false,
													HttpStatusCode.BadRequest,
													"Missing required claims (sub or email)");
		}

		// Find or create user
		var user = await _userManager.FindByLoginAsync("Google", sub)
				?? await _userManager.FindByEmailAsync(email);

		if (user is null)
		{
			user = new IdentityUser()
			{
				UserName = email,
				Email = email,
				EmailConfirmed = false,
			};
			var create = await _userManager.CreateAsync(user);
			if (!create.Succeeded)
			{
				var errors = string.Join(", ", create.Errors.Select(e => e.Description));
				return new ServiceResult<TokenResponse>(false,
														HttpStatusCode.BadRequest,
														"User creation failed: " + errors);
				
			}
		}

		// Link external login if missing
		var logins = await _userManager.GetLoginsAsync(user);
		if (!logins.Any(l => l.LoginProvider == "Google" && l.ProviderKey == sub))
			await _userManager.AddLoginAsync(user, new UserLoginInfo("Google", sub, "Google"));

		// Enforce confirmed email
		if (!user.EmailConfirmed)
		{
			return new ServiceResult<TokenResponse>(false,
													HttpStatusCode.Forbidden,
													"Email address is not confirmed");
		}

		// Generate token
		var (accessToken, expiresAt) = await _tokenService.CreateForUserAsync(user);
		var tokenResponse = new TokenResponse(accessToken, expiresAt);
		
		return new ServiceResult<TokenResponse>(true,
												HttpStatusCode.OK,
												"Token exchanged successfully",
												tokenResponse);
	}
}
