using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Auth;

public partial class AuthController
{
	/// <summary>
	/// Activates a user account by confirming their email address.
	/// </summary>
	/// <param name="userId">The unique identifier of the user to activate.</param>
	/// <param name="token">The email confirmation token sent to the user's email.</param>
	/// <param name="ct">A CancellationToken to observe while waiting for the operation to complete.</param>
	/// <remarks>
	/// <para># Email Confirmation</para>
	/// <para>This endpoint is typically called via a link sent to the user's email during registration.</para>
	/// <para>The activation process:</para>
	/// <para>- Validates the user exists</para>
	/// <para>- Verifies the confirmation token</para>
	/// <para>- Marks the email as confirmed</para>
	/// <para>Once activated, the user can log in using their credentials.</para>
	/// <para>If the email is already confirmed, this endpoint will return an error.</para>
	/// </remarks>
	/// <returns>An IActionResult indicating success or failure of the activation process.</returns>
	[HttpGet("ActivateUser")]
	public async Task<IActionResult> ActivateUser([FromQuery] string userId, [FromQuery] string token, CancellationToken ct)
	{
		return await _orchestrator.HandleActivateUserAsync(userId, token, ct);
	}
}
