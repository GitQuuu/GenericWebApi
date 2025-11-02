using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Services.Authentication;

public partial class AuthenticationOrchestrator
{
	
	public record ChangePasswordRequestDto
	{
		[Required]
		[StringLength(100, MinimumLength = 8, ErrorMessage = "Current password is required.")]
		public required string CurrentPassword { get; init; }
		
		[Required]
		[StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 100 characters.")]
		public required string NewPassword { get; init; }
		
		[Required]
		[Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
		public required string ConfirmPassword { get; init; }
	}
	
	public async Task<IActionResult> HandleChangePasswordAsync(string userId, ChangePasswordRequestDto request, CancellationToken ct)
	{
		// Validate input parameters
		if (string.IsNullOrWhiteSpace(userId))
		{
			return await _responseService.HandleResultAsync(
				new ServiceResult<string>(false, HttpStatusCode.BadRequest, "User ID is required."));
		}

		// Call the service to change the password
		var changePasswordResult = await _userService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword, ct);

		if (changePasswordResult.Success is false)
		{
			return await _responseService.HandleResultAsync(changePasswordResult);
		}

		return await _responseService.HandleResultAsync(
			new ServiceResult<string>(true, HttpStatusCode.OK, "Password has been successfully changed."));
	}
}
