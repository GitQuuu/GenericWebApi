using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Services.Authentication.HandleEntraLoginAsync;

public partial class AuthenticationOrchestrator
{
	/// <inheritdoc />
	public async Task<IActionResult> HandleEntraLoginAsync(CancellationToken ctx = default)
	{
		ClaimsPrincipal             principal     = _httpContextAccessor.HttpContext?.User ?? throw new ArgumentNullException(nameof(principal));
		ServiceResult<IdentityUser> entraResponse = await _identityProviderService.ExchangeMicrosoftTokenAsync(principal, ctx);
		var                         tokenResponse = await _tokenService.CreateForUserAsync(entraResponse.Data);

		return await _responseService.HandleResultAsync(tokenResponse);
	}
}