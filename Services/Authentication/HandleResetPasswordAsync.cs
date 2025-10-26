using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Services.Authentication;

public partial class AuthenticationOrchestrator
{
	public async Task<IActionResult> HandleResetPasswordAsync(string userId, string token, ResetPasswordRequest request, CancellationToken ct)
	{
		// Validate input parameters
		if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
		{
			return await _responseService.HandleResultAsync(
															new ServiceResult<string>(false, HttpStatusCode.BadRequest, "User ID and reset token are required."));
		}

		// Decode the token (it comes URL-encoded from the email link)
		string decodedToken = Uri.UnescapeDataString(token);

		// Call the service to reset the password
		var resetPasswordResult = await _userService.ResetPasswordAsync(userId, decodedToken, request.NewPassword, ct);

		if (resetPasswordResult.Success is false)
		{
			return await _responseService.HandleResultAsync(resetPasswordResult);
		}

		return new OkResult();
	}
}