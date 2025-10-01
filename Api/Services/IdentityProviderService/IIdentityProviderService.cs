using Api.Services.TokenService;

namespace Api.Services.IdentityProviderService;

public interface IIdentityProviderService
{
	Task<ServiceResult<TokenResponse>> ExchangeMicrosoftTokenAsync(System.Security.Claims.ClaimsPrincipal? principal, CancellationToken ct = default);
	Task<ServiceResult<TokenResponse>> ExchangeGoogleTokenAsync(System.Security.Claims.ClaimsPrincipal? principal, CancellationToken ct = default);
}
