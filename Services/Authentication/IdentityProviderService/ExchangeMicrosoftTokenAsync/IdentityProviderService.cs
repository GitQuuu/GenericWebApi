using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Services.Authentication.IdentityProviderService.ExchangeMicrosoftTokenAsync;

public partial class IdentityProviderService
{
	/// <inheritdoc />
	public async Task<ServiceResult<IdentityUser>> ExchangeMicrosoftTokenAsync(ClaimsPrincipal? principal, CancellationToken ct = default)
	{
		if (principal?.Identity?.IsAuthenticated != true)
		{
			return new ServiceResult<IdentityUser>(
												   false,
												   HttpStatusCode.Unauthorized,
												   "User is not authenticated");
		}

		// Optional multi-tenant allowlist guard
		var allowAll = bool.TryParse((string?) _configuration["Auth:Entra:AllowAllTenants"], out var a) && a;
		if (!allowAll)
		{
			var tid = principal.FindFirst("tid")?.Value;
			var allowed = ConfigurationBinder.Get<string[]>(_configuration.GetSection("Auth:Entra:AllowedTenantIds")) ?? Array.Empty<string>();
			if (string.IsNullOrEmpty(tid) || !allowed.Contains(tid, StringComparer.OrdinalIgnoreCase))
			{
				return new ServiceResult<IdentityUser>(
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
			return new ServiceResult<IdentityUser>(
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
				var errors = string.Join(", ", Enumerable.Select<IdentityError, string>(create.Errors, e => e.Description));
				return new ServiceResult<IdentityUser>(
													   false,
													   HttpStatusCode.BadRequest,
													   "User creation failed: " + errors);
			}
		}

		if (user.EmailConfirmed is false)
		{
			return new ServiceResult<IdentityUser>(
												   false,
												   HttpStatusCode.Forbidden,
												   "Email address is not confirmed");
		}

		// Ensure external login mapping exists
		var logins = await _userManager.GetLoginsAsync(user);
		if (!Enumerable.Any<UserLoginInfo>(logins, l => l.LoginProvider == "Microsoft" && l.ProviderKey == providerKey))
		{
			await _userManager.AddLoginAsync(user, new UserLoginInfo("Microsoft", providerKey, "Microsoft"));
		}

		return new ServiceResult<IdentityUser>(true,
											   HttpStatusCode.OK,
											   "Token exchanged successfully",
											   user);
	}
}