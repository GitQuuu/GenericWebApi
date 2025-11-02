using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Auth;

public partial class AuthController
{
	/// <summary>
	/// Represents a request to resend the activation email to a user's registered email address.
	/// </summary>
	public class ResendActivationRequest
	{
		/// <summary>
		/// Gets or sets the email address associated with the request.
		/// </summary>
		/// <remarks>
		/// This property represents the email of the user for which an activation email
		/// will be resent. It is a required field.
		/// </remarks>
		public required string Email { get; set; }
	}
	
	/// <summary>
	/// Resends the activation email to a user's registered email address.
	/// </summary>
	/// <param name="email">The email address of the user.</param>
	/// <param name="ct">Cancellation token.</param>
	/// <returns>A response indicating success or failure of the resend operation.</returns>
	[HttpPost("ResendActivationMail")]
	public async Task<IActionResult> ResendActivation([FromBody] ResendActivationRequest request, CancellationToken ct)
	{
		if (string.IsNullOrWhiteSpace(request?.Email))
		{
			return BadRequest("Email is required.");
		}

		return await _orchestrator.HandleResendActivationMailAsync(request.Email, ct);
	}
}