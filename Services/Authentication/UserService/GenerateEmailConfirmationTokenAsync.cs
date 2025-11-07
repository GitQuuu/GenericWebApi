using Microsoft.AspNetCore.Identity;

namespace Services.Authentication.UserService;

public partial class UserService
{
	/// <inheritdoc />
	public async Task<string> GenerateEmailConfirmationTokenAsync(IdentityUser user)
	{
		return await _userManager.GenerateEmailConfirmationTokenAsync(user);
	}
}
