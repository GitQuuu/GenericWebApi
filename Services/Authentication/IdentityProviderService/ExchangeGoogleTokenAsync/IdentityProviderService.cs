using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace Services.Authentication.IdentityProviderService;

public partial class IdentityProviderService
{
	public async Task<ServiceResult<IdentityUser>> ExchangeGoogleTokenAsync(ClaimsPrincipal? principal, CancellationToken ct = default)
	{
		if (principal?.Identity?.IsAuthenticated != true)
		{
			return new ServiceResult<IdentityUser>(false,
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
			return new ServiceResult<IdentityUser>(false,
												   HttpStatusCode.BadRequest,
												   "Missing required claims (sub or email)");
		}

		// Find or create user
		var user = await _userService.FindByLoginAsync("Google", sub)
				?? await _userService.FindByEmailAsync(email);

		if (user is null)
		{
			user = new IdentityUser()
			{
				UserName = email,
				Email = email,
				EmailConfirmed = false,
			};
			var create = await _userService.CreateAsync(user);
			if (!create.Succeeded)
			{
				var errors = string.Join(", ", Enumerable.Select<IdentityError, string>(create.Errors, e => e.Description));
				return new ServiceResult<IdentityUser>(false,
													   HttpStatusCode.BadRequest,
													   "User creation failed: " + errors);
				
			}
		}

		// Link external login if missing
		var logins = await _userService.GetLoginsAsync(user);
		if (!Enumerable.Any<UserLoginInfo>(logins, l => l.LoginProvider == "Google" && l.ProviderKey == sub))
			await _userService.AddLoginAsync(user, new UserLoginInfo("Google", sub, "Google"));

		// Enforce confirmed email
		if (!user.EmailConfirmed)
		{
			return new ServiceResult<IdentityUser>(false,
													HttpStatusCode.Forbidden,
													"Email address is not confirmed");
		}

		
		return new ServiceResult<IdentityUser>(true,
											   HttpStatusCode.OK,
											   "Token exchanged successfully",
											   user);
	}
}