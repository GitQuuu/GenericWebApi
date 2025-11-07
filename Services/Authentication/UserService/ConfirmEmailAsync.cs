using Microsoft.AspNetCore.Identity;

namespace Services.Authentication.UserService;

public partial class UserService
{
	/// <inheritdoc />
	public async Task<IdentityResult> ConfirmEmailAsync(IdentityUser user, string token)
	{
		return await _userManager.ConfirmEmailAsync(user, token);
	}
}
