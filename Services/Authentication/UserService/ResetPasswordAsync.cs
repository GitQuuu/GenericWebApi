using System.Net;
using Microsoft.AspNetCore.Identity;

namespace Services.Authentication.UserService;

public partial class UserService
{
	/// <inheritdoc />
	public async Task<ServiceResult<IdentityResult>> ResetPasswordAsync(string userId, string decodedToken, string requestNewPassword, CancellationToken ct)
	{
		// Find user by ID
		var user = await _userManager.FindByIdAsync(userId);

		if (user is null)
		{
			return new ServiceResult<IdentityResult>(false, HttpStatusCode.NotFound, "User not found.");
		}

		// Attempt to reset the password using the decoded token
		IdentityResult result = await _userManager.ResetPasswordAsync(user, decodedToken, requestNewPassword);

		if (result.Succeeded is false)
		{
			// Extract error messages from IdentityResult
			var errors = string.Join(", ", result.Errors.Select(e => e.Description));
			return new ServiceResult<IdentityResult>(false, HttpStatusCode.BadRequest, $"Password reset failed: {errors}");
		}

		return new ServiceResult<IdentityResult>(true, HttpStatusCode.OK, "Password has been successfully reset.", result);
	}
}
