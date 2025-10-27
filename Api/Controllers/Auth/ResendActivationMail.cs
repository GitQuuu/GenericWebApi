using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Auth;

public partial class AuthController
{
	/// <summary>
	/// Resends the activation email to a user's registered email address.
	/// </summary>
	/// <param name="email">The email address of the user.</param>
	/// <param name="ct">Cancellation token.</param>
	/// <returns>A response indicating success or failure of the resend operation.</returns>
	[HttpPost("ResendActivationMail")]
	public async Task<IActionResult> ResendActivation([FromQuery] string email, CancellationToken ct)
	{
		if (string.IsNullOrWhiteSpace(email))
		{
			return BadRequest("Email is required.");
		}

		return await _orchestrator.HandleResendActivationMailAsync(email, ct);
	}
}