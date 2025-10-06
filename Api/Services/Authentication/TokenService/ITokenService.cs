using Microsoft.AspNetCore.Identity;

namespace Api.Services.TokenService;

public interface ITokenService 
{
	/// <summary>
	/// Generates a token for a specified user asynchronously.
	/// </summary>
	/// <param name="user">The IdentityUser for which the token is generated.</param>
	/// <returns>A task that represents the asynchronous operation.
	/// The task result contains a ServiceResult instance, which includes the token response details.</returns>
	Task<ServiceResult<TokenResponse>> CreateForUserAsync(IdentityUser user);
}