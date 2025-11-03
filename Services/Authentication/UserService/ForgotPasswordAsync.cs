using System.Net;
using Microsoft.AspNetCore.Identity;

namespace Services.Authentication.UserService;

public partial class UserService
{
	/// <inheritdoc />
	public async Task<ServiceResult<Tuple<IdentityUser, string>>> ForgotPasswordAsync(string email, CancellationToken ct)
	{
		// Find user by email
		var user = await _userManager.FindByEmailAsync(email);
	
		// For security, return success regardless of whether user exists
		if (user == null)
		{
			return new ServiceResult<Tuple<IdentityUser, string>>(false, HttpStatusCode.NoContent, "If the email exists, a password reset link has been sent.");
		}
	
		// Generate password reset token
		var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

		if (resetToken is null)
		{
			return new ServiceResult<Tuple<IdentityUser, string>>(false, HttpStatusCode.InternalServerError, "Failed to generate password reset token.");
		}
	
		return new ServiceResult<Tuple<IdentityUser, string>>(true, HttpStatusCode.OK, "If the email exists, a password reset link has been sent.", new Tuple<IdentityUser, string>(user, resetToken));
	}
}
