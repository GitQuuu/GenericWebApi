using Microsoft.AspNetCore.Identity;

namespace Api.Services.TokenService;

public interface ITokenService 
{
	Task<ServiceResult<TokenResponse>> CreateForUserAsync(IdentityUser user);
}