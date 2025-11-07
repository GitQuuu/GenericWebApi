using Microsoft.AspNetCore.Identity;

namespace Services.Authentication.UserService;

public partial class UserService
{
	/// <inheritdoc />
	public async Task<IdentityUser?> FindByLoginAsync(string loginProvider, string providerKey)
	{
		return await _userManager.FindByLoginAsync(loginProvider, providerKey);
	}
}
