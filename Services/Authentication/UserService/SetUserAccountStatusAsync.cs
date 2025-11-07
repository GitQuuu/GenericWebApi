using System.Net;
using Microsoft.AspNetCore.Identity;

namespace Services.Authentication.UserService;

public partial class UserService
{
	/// <inheritdoc />
	public async Task<ServiceResult<IdentityUser>> SetUserAccountStatusAsync(string userId, bool isActive, CancellationToken ct)
	{
		// Find user by ID
		var user = await _userManager.FindByIdAsync(userId);

		if (user is null)
		{
			return new ServiceResult<IdentityUser>(false, HttpStatusCode.NotFound, "User not found.");
		}

		// Check if already in the desired state
		if (user.EmailConfirmed == isActive)
		{
			var statusMessage = isActive ? "activated" : "deactivated";
			return new ServiceResult<IdentityUser>(false, HttpStatusCode.BadRequest, $"User account is already {statusMessage}.");
		}

		IdentityResult result;

		if (isActive)
		{
			// Generate and confirm email token to activate the account
			var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
			result = await _userManager.ConfirmEmailAsync(user, token);
		}
		else
		{
			// Revoke email confirmation to deactivate the account
			user.EmailConfirmed = false;
			result = await _userManager.UpdateAsync(user);
		}

		if (!result.Succeeded)
		{
			var errors = string.Join(", ", result.Errors.Select(e => e.Description));
			var action = isActive ? "activate" : "deactivate";
			return new ServiceResult<IdentityUser>(false, HttpStatusCode.InternalServerError, $"Failed to {action} user: {errors}");
		}

		var successMessage = isActive ? "activated" : "deactivated";
		return new ServiceResult<IdentityUser>(true, HttpStatusCode.OK, $"User account has been successfully {successMessage}.", user);
	}
}
