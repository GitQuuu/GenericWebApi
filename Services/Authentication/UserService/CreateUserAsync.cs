using Microsoft.AspNetCore.Identity;

namespace Services.Authentication.UserService;

public partial class UserService
{
	/// <inheritdoc />
	public async Task<IdentityResult> CreateUserAsync(string email, string password)
	{
		var user = new IdentityUser
		{
			UserName = email,
			Email = email
		};
		return await _userManager.CreateAsync(user, password);
	}
}
