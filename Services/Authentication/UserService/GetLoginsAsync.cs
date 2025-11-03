using Microsoft.AspNetCore.Identity;

namespace Services.Authentication.UserService;

public partial class UserService
{
	/// <inheritdoc />
	public async Task<IList<UserLoginInfo>> GetLoginsAsync(IdentityUser user)
	{
		return await _userManager.GetLoginsAsync(user);
	}
}
