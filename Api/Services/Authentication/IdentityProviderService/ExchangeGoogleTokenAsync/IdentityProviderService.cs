using System.Net;
using System.Security.Claims;
using Api.Services.TokenService;
using Microsoft.AspNetCore.Identity;

namespace Api.Services.IdentityProviderService;

public partial class IdentityProviderService
{
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