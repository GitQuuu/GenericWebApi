using System.Net;
using Microsoft.AspNetCore.Identity;

namespace Services.Authentication.UserService;

public partial class UserService
{
	/// <inheritdoc />
	public async Task<ServiceResult<IdentityResult>> ChangePasswordAsync(string userId, string currentPassword, string newPassword, CancellationToken ct)
	{
		// Find user by ID
		var user = await _userManager.FindByIdAsync(userId);

		if (user is null)
		{
			return new ServiceResult<IdentityResult>(false, HttpStatusCode.NotFound, "User not found.");
		}

		// Attempt to change the password
		IdentityResult result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

		if (result.Succeeded is false)
		{
			// Extract error messages from IdentityResult
			var errors = string.Join(", ", result.Errors.Select(e => e.Description));
			return new ServiceResult<IdentityResult>(false, HttpStatusCode.BadRequest, $"Password change failed: {errors}");
		}

		return new ServiceResult<IdentityResult>(true, HttpStatusCode.OK, "Password has been successfully changed.", result);
	}
}
