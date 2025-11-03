using Microsoft.AspNetCore.Identity;

namespace Services.Authentication.UserService;

public partial class UserService
{
	/// <inheritdoc />
	public async Task<IdentityUser?> FindByIdAsync(string userId)
	{
		return await _userManager.FindByIdAsync(userId);
	}
}
