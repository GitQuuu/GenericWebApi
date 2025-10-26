using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Services.Authentication;

public partial class AuthenticationOrchestrator
{
	
	public record ResetPasswordRequestDto
	{
		[StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 100 characters.")]
		public required string NewPassword { get; init; }
		
		[Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
		public required string ConfirmPassword { get; init; }
	}
	
	public async Task<IActionResult> HandleResetPasswordAsync(string userId, string token, ResetPasswordRequestDto request, CancellationToken ct)
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