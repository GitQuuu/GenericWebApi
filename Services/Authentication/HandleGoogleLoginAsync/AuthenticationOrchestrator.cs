using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services.Authentication.TokenService;

namespace Services.Authentication;

public partial class AuthenticationOrchestrator
{
	/// <inheritdoc />
	public async Task<IActionResult> HandleGoogleLoginAsync(CancellationToken ctx = default)
	{
		ClaimsPrincipal              principal     = _httpContextAccessor.HttpContext?.User ?? throw new ArgumentNullException(nameof(principal));
		ServiceResult<IdentityUser>  googleResponse = await _identityProviderService.ExchangeGoogleTokenAsync(principal, ctx);
		
		if (googleResponse.Data is null)
		{
			return await _responseService.HandleResultAsync(googleResponse); 
		}
		
		ServiceResult<TokenResponse> tokenResponse = await _tokenService.CreateForUserAsync(googleResponse.Data);

		return await _responseService.HandleResultAsync(tokenResponse);
	}
}