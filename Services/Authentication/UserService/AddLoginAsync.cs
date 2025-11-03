using Microsoft.AspNetCore.Identity;

namespace Services.Authentication.UserService;

public partial class UserService
{
	/// <inheritdoc />
	public async Task<IdentityResult> AddLoginAsync(IdentityUser user, UserLoginInfo login)
	{
		return await _userManager.AddLoginAsync(user, login);
	}
}
