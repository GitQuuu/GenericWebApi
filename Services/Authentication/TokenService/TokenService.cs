using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Services.Authentication.TokenService;

/// <summary>
/// The TokenService class provides functionality to generate JWT tokens for authenticated users.
/// </summary>
public class TokenService : ITokenService
{
	private readonly IConfiguration _cfg;
	private readonly TimeProvider _timeProvider;
	private readonly UserManager<IdentityUser> _userManager;

	public TokenService(IConfiguration cfg, TimeProvider timeProvider, UserManager<IdentityUser> userManager)
	{
		_cfg             = cfg;
		_timeProvider    = timeProvider;
		_userManager = userManager;
	}

	public async Task<ServiceResult<TokenResponse>> CreateForUserAsync(IdentityUser user)
	{
		var now      = _timeProvider.GetUtcNow();
		var issuer   = _cfg["Auth:Local:Issuer"];
		var audience = _cfg["Auth:Local:Audience"];
		var key      = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_cfg["Auth:Local:SigningKey"]!));
		var creds    = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

		var ttl     = TimeSpan.FromMinutes(int.Parse(_cfg["Auth:Local:AccessTokenMinutes"] ?? "60"));
		var expires = now.Add(ttl);

		var claims = new List<Claim>
		{
			new (ClaimTypes.NameIdentifier, user.Id), 
			new (ClaimTypes.Name, user.UserName ?? user.Email ?? string.Empty), 
			new (ClaimTypes.Email, user.Email ?? string.Empty),
		};

		var identityUser  = await _userManager.FindByIdAsync(user.Id);
		var roles = await _userManager.GetRolesAsync(identityUser);
		claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

		var jwt = new JwtSecurityToken(
									   issuer : issuer,
									   audience : audience,
									   claims : claims,
									   notBefore : now.DateTime,
									   expires : expires.UtcDateTime,
									   signingCredentials : creds);

		var token = new JwtSecurityTokenHandler().WriteToken(jwt);
		return new ServiceResult<TokenResponse>(true,HttpStatusCode.OK, token);
	}
}