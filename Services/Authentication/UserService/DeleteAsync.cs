using Microsoft.AspNetCore.Identity;

namespace Services.Authentication.UserService;

public partial class UserService
{
	/// <inheritdoc />
	public async Task<IdentityResult> DeleteAsync(IdentityUser user)
	{
		return await _userManager.DeleteAsync(user);
	}
}
