using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Services.Authentication;

public partial class AuthenticationOrchestrator
{
	/// <inheritdoc />
	public async Task<IActionResult> HandleSetUserAccountStatusAsync(string userId, bool isActive, CancellationToken ct = default)
	{
		// Validate userId
		if (string.IsNullOrWhiteSpace(userId))
		{
			return await _responseService.HandleResultAsync(
				new ServiceResult<string>(false, HttpStatusCode.BadRequest, "User ID is required."));
		}

		// Set the user account status
		var result = await _userService.SetUserAccountStatusAsync(userId, isActive, ct);

		if (!result.Success)
		{
			return await _responseService.HandleResultAsync(result);
		}

		var statusMessage = isActive ? "activated" : "deactivated";
		return await _responseService.HandleResultAsync(
			new ServiceResult<string>(true, HttpStatusCode.OK, $"User account has been successfully {statusMessage}."));
	}
}
