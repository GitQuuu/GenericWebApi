using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Authentication;

namespace Api.Controllers.Auth;

public partial class AuthController
{
	
	/// <summary>
	/// Request model for password change.
	/// </summary>
	public record ChangePasswordRequest
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
	
	/// <summary>
	/// Changes the password for the authenticated user.
	/// </summary>
	/// <param name="request">The change password request containing the current and new password.</param>
	/// <param name="ct">A CancellationToken to observe while waiting for the operation to complete.</param>
	/// <remarks>
	/// <para># Password Change Flow</para>
	/// <para>This endpoint allows an authenticated user to change their password:</para>
	/// <para>- Validates the user is authenticated</para>
	/// <para>- Verifies the current password is correct</para>
	/// <para>- Updates the user's password to the new password</para>
	/// <para>The user must be authenticated (logged in) to use this endpoint.</para>
	/// </remarks>
	/// <returns>An IActionResult indicating success or failure of the password change.</returns>
	[HttpPost("ChangePassword")]
	[Authorize]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> ChangePassword(
		[FromBody] ChangePasswordRequest request,
		CancellationToken ct)
	{
		// Get the user ID from the authenticated user's claims
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		
		if (string.IsNullOrWhiteSpace(userId))
		{
			return Unauthorized("User is not authenticated.");
		}

		return await _orchestrator.HandleChangePasswordAsync(
			userId, 
			request.Adapt<AuthenticationOrchestrator.ChangePasswordRequestDto>(),
			ct);
	}
}
