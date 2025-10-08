using Api.Services.TokenService;
using Microsoft.AspNetCore.Identity;

namespace Api.Services.IdentityProviderService;

public interface IIdentityProviderService
{
	/// <summary>
	/// Exchanges the Microsoft authentication token for a locally authenticated token.
	/// </summary>
	/// <param name="principal">The user's claims-based identities represented as a ClaimsPrincipal.</param>
	/// <param name="ct">An optional CancellationToken to observe while waiting for the task to complete.</param>
	/// <returns>A ServiceResult containing either a TokenResponse with the access token and its expiration time, or an error message indicating the issue.</returns>
	Task<ServiceResult<IdentityUser>> ExchangeMicrosoftTokenAsync(System.Security.Claims.ClaimsPrincipal? principal, CancellationToken ct = default);

	/// <summary>
	/// Exchanges the Google authentication token for a locally authenticated token.
	/// </summary>
	/// <param name="principal">The user's claims-based identities represented as a ClaimsPrincipal.</param>
	/// <param name="ct">An optional CancellationToken to observe while waiting for the task to complete.</param>
	/// <returns>A ServiceResult containing either a TokenResponse with the access token and its expiration time, or an error message indicating the issue.</returns>
	Task<ServiceResult<IdentityUser>> ExchangeGoogleTokenAsync(System.Security.Claims.ClaimsPrincipal? principal, CancellationToken ct = default);
}
