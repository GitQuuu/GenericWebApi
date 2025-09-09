using Microsoft.AspNetCore.Identity;

namespace Api.Services.TokenService;

public class TokenService : ITokenService
{
	private readonly IConfiguration _cfg;
	private readonly TimeProvider _timeProvider;
	private readonly IIdentityService _identityService;

	public TokenService(IConfiguration cfg, TimeProvider timeProvider, IIdentityService identityService)
	{
		_cfg             = cfg;
		_timeProvider    = timeProvider;
		_identityService = identityService;
	}

	public async Task<(string Token, DateTimeOffset ExpiresAt)> CreateForUserAsync(ApplicationUser user)
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

		var roles = await _identityService.GetRolesAsync(user.Id);
		claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

		var jwt = new JwtSecurityToken(
									   issuer : issuer,
									   audience : audience,
									   claims : claims,
									   notBefore : now.DateTime,
									   expires : expires.UtcDateTime,
									   signingCredentials : creds);

		var token = new JwtSecurityTokenHandler().WriteToken(jwt);
		return (token, expires);
	}
}