using Microsoft.AspNetCore.Identity;

namespace Services.Authentication.UserService;

public partial class UserService
{
	/// <inheritdoc />
	public async Task<bool> IsEmailConfirmedAsync(IdentityUser user)
	{
		return user.EmailConfirmed;
	}
}
