using Microsoft.AspNetCore.Identity;

namespace Services.Authentication.UserService;

public partial class UserService
{
	/// <inheritdoc />
	public async Task<bool> PasswordSignInAsync(IdentityUser user, string password)
	{
		var result = await _signInManager.PasswordSignInAsync(user, password, false, lockoutOnFailure : false);
		return result.Succeeded;
	}

	/// <inheritdoc />
	public async Task<bool> PasswordSignInAsync(string email, string password)
	{
		var result = await _signInManager.PasswordSignInAsync(email, password, isPersistent : false, lockoutOnFailure : false);
		return result.Succeeded;
	}
}
