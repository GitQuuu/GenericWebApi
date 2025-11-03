using Microsoft.AspNetCore.Identity;

namespace Services.Authentication.UserService;

public partial class UserService
{
	/// <inheritdoc />
	public async Task<IList<string>> GetRolesAsync(IdentityUser user)
	{
		return await _userManager.GetRolesAsync(user);
	}
}
