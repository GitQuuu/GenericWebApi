using Microsoft.AspNetCore.Identity;

namespace Services.Authentication.UserService;

public partial class UserService
{
	/// <inheritdoc />
	public async Task<IdentityUser?> FindByEmailAsync(string email)
	{
		return await _userManager.FindByEmailAsync(email);
	}
}
