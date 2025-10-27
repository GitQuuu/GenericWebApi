using System.ComponentModel.DataAnnotations;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Services.Authentication;

namespace Api.Controllers.Auth;

public partial class AuthController
{
	
	/// <summary>
	/// Request model for password reset.
	/// </summary>
	public record ResetPasswordRequest
	{
		[Required]
		[StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 100 characters.")]
		public required string NewPassword { get; init; }

		[Required]
		[Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
		public required string ConfirmPassword { get; init; }
	}
	
	/// <summary>
	/// Completes the password reset process for a user.
	/// </summary>
	/// <param name="userId">The ID of the user (from email callback link query parameter).</param>
	/// <param name="token">The password reset token (from email callback link query parameter).</param>
	/// <param name="request">The reset password request containing the new password.</param>
	/// <param name="ct">A CancellationToken to observe while waiting for the operation to complete.</param>
	/// <remarks>
	/// <para># Password Reset Completion Flow</para>
	/// <para>This endpoint completes the password reset process:</para>
	/// <para>- Validates the user exists</para>
	/// <para>- Validates the reset token from the email callback</para>
	/// <para>- Updates the user's password</para>
	/// <para>The user is redirected to this endpoint via the reset link sent in the forgot password email.</para>
	/// <para>Query parameters (userId, token) come from the email callback URL.</para>
	/// </remarks>
	/// <returns>An IActionResult indicating success or failure of the password reset.</returns>
	[HttpGet("ResetPassword")]
	public async Task<IActionResult> ResetPassword(
		[FromQuery] string userId,
		[FromQuery] string token,
		[FromForm] ResetPasswordRequest request,
		CancellationToken ct)
	{
		return await _orchestrator.HandleResetPasswordAsync(
															userId, 
															token, 
															request.Adapt<AuthenticationOrchestrator.ResetPasswordRequestDto>(),
															ct);
	}
}


