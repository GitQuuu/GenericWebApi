using Microsoft.AspNetCore.Identity;

namespace Api.Services.TokenService;

public interface ITokenService 
{
	Task<(string Token, DateTimeOffset ExpiresAt)> CreateForUserAsync(IdentityUser user);
}