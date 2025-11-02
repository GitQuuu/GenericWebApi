using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Auth;

public partial class AuthController
{
	/// <summary>
	/// Request model for forgot password.
	/// </summary>
	public record ForgotPasswordRequest
	{
		[Required]
		[EmailAddress]
		public required string Email { get; init; }
	}

	/// <summary>
	/// Initiates a password reset process for a user account.
	/// </summary>
	/// <param name="request">The forgot password request containing the user's email.</param>
	/// <param name="ct">A CancellationToken to observe while waiting for the operation to complete.</param>
	/// <remarks>
	/// <para># Password Reset Flow</para>
	/// <para>This endpoint initiates the password reset process:</para>
	/// <para>- Validates that the email exists in the system</para>
	/// <para>- Generates a password reset token</para>
	/// <para>- Sends a password reset email with a reset link</para>
	/// <para>The user must click the reset link in the email to proceed with resetting their password.</para>
	/// <para>The reset link will direct to the ResetPassword endpoint with the necessary token.</para>
	/// <para>For security purposes, this endpoint returns success regardless of whether the email exists.</para>
	/// </remarks>
	/// <returns>An IActionResult indicating success of the password reset initiation.</returns>
	[HttpPost("ForgotPassword")]
	public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request, CancellationToken ct)
	{
		return await _orchestrator.HandleForgotPasswordAsync(request.Email, ct);
	}
}
