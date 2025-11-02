using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Services.Authentication;

public partial class AuthenticationOrchestrator
{
	/// <inheritdoc />
	public async Task<IActionResult> HandleActivateUserAsync(string userId, string token, CancellationToken ctx = default)
	{
		// Find the user
		var user = await _userService.FindByIdAsync(userId);
		if (user is null)
		{
			return await _responseService.HandleResultAsync(
				new ServiceResult<string>(false, HttpStatusCode.NotFound, "User not found"));
		}

		// Check if email is already confirmed
		if (user.EmailConfirmed)
		{
			return await _responseService.HandleResultAsync(
				new ServiceResult<string>(false, HttpStatusCode.BadRequest, "Email is already confirmed"));
		}

		// Confirm the email
		var result = await _userService.ConfirmEmailAsync(user, token);
		
		if (!result.Succeeded)
		{
			var errors = string.Join(", ", result.Errors.Select(e => e.Description));
			return await _responseService.HandleResultAsync(
				new ServiceResult<string>(false, HttpStatusCode.BadRequest, $"Email confirmation failed: {errors}"));
		}

		return await _responseService.HandleResultAsync(
			new ServiceResult<string>(true, HttpStatusCode.OK, "Email confirmed successfully. You can now log in."));
	}
}
