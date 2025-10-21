using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services.Authentication.TokenService;

namespace Services.Authentication;

public partial class AuthenticationOrchestrator
{
	/// <inheritdoc />
	public async Task<IActionResult> HandleEntraGoogleAsync(CancellationToken ctx = default)
	{
		ClaimsPrincipal              principal     = _httpContextAccessor.HttpContext?.User ?? throw new ArgumentNullException(nameof(principal));
		ServiceResult<IdentityUser>  entraResponse = await _identityProviderService.ExchangeGoogleTokenAsync(principal, ctx);
		
		if (entraResponse.Data is null)
		{
			return await _responseService.HandleResultAsync(entraResponse); 
		}
		
		ServiceResult<TokenResponse> tokenResponse = await _tokenService.CreateForUserAsync(entraResponse.Data);

		return await _responseService.HandleResultAsync(tokenResponse);
	}
}