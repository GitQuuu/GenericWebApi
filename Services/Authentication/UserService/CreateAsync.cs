using Microsoft.AspNetCore.Identity;

namespace Services.Authentication.UserService;

public partial class UserService
{
	/// <inheritdoc />
	public async Task<IdentityResult> CreateAsync(IdentityUser user)
	{
		return await _userManager.CreateAsync(user);
	}
}
