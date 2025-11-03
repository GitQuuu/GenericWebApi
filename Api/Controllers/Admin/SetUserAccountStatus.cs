using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Admin;

public partial class AdminUserController
{
	/// <summary>
	/// Sets the activation status of a user account (Admin only).
	/// </summary>
	/// <param name="userId">The unique identifier of the user.</param>
	/// <param name="isActive">True to activate the account, false to deactivate it.</param>
	/// <param name="ct">A CancellationToken to observe while waiting for the operation to complete.</param>
	/// <remarks>
	/// <para># Set User Account Status</para>
	/// <para>This endpoint allows administrators to activate or deactivate user accounts.</para>
	/// <para>When activating (isActive=true):</para>
	/// <para>- Validates the user exists</para>
	/// <para>- Confirms the user's email</para>
	/// <para>- Enables the user to log in</para>
	/// <para>When deactivating (isActive=false):</para>
	/// <para>- Validates the user exists</para>
	/// <para>- Revokes the user's email confirmation</para>
	/// <para>- Prevents the user from logging in</para>
	/// <para>This is typically used when UserActivationMode is set to "AdminApproval".</para>
	/// </remarks>
	/// <returns>An IActionResult indicating success or failure of the operation.</returns>
	[HttpPut("SetStatus/{userId}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> SetUserAccountStatus([FromRoute] string userId, [FromQuery] bool isActive, CancellationToken ct)
	{
		return await _orchestrator.HandleSetUserAccountStatusAsync(userId, isActive, ct);
	}
}
