using System.Security.Claims;
using Api.Services.IdentityProviderService;
using Api.Services.TokenService;
using Microsoft.AspNetCore.Mvc;
using Services.ResponseService;

namespace Api.Services.Authentication;

/// <summary>
/// 
/// </summary>
public class AuthenticationOrchestrator : IAuthenticationOrchestrator
{
	private readonly IIdentityProviderService _identityProviderService;
	private readonly ITokenService _tokenService;
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly IResponseService _responseService;

	public AuthenticationOrchestrator(IIdentityProviderService identityProviderService,
									  ITokenService tokenService,
									  IHttpContextAccessor httpContextAccessor,
									  IResponseService responseService)
	{
		_identityProviderService = identityProviderService;
		_tokenService            = tokenService;
		_httpContextAccessor     = httpContextAccessor;
		_responseService		 = responseService;
	}
	
	public async Task<IActionResult> HandleEntraLoginAsync(CancellationToken ctx = default)
	{
		ClaimsPrincipal principal     = _httpContextAccessor.HttpContext?.User ?? throw new ArgumentNullException(nameof(principal));
		ServiceResult<TokenResponse> entraResponse = await _identityProviderService.ExchangeMicrosoftTokenAsync(principal);

		return await _responseService.HandleResultAsync(entraResponse);
	}
	
	public async Task<IActionResult> HandleEntraGoogleAsync(CancellationToken ctx = default)
	{
		ClaimsPrincipal              principal     = _httpContextAccessor.HttpContext?.User ?? throw new ArgumentNullException(nameof(principal));
		ServiceResult<TokenResponse> entraResponse = await _identityProviderService.ExchangeGoogleTokenAsync(principal);

		return await _responseService.HandleResultAsync(entraResponse);
	}
}