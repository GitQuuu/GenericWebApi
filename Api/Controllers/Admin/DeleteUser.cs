using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Authentication;

namespace Api.Controllers.Admin;

public partial class AccessControlController
{
	/// <summary>
	/// Deletes a user account (Admin only).
	/// </summary>
	/// <param name="userId">The unique identifier of the user to delete.</param>
	/// <param name="password">The admin's password for confirmation.</param>
	/// <param name="ct">A CancellationToken to observe while waiting for the operation to complete.</param>
	/// <remarks>
	/// <para># Delete User</para>
	/// <para>This endpoint allows administrators to permanently delete a user account from the system.</para>
	/// <para>The operation will:</para>
	/// <para>- Verify the admin's password for security</para>
	/// <para>- Validate the user exists</para>
	/// <para>- Permanently remove the user from the database</para>
	/// <para>- Remove all associated user data</para>
	/// <para>**Warning:** This action cannot be undone.</para>
	/// </remarks>
	/// <returns>An IActionResult indicating success or failure of the deletion operation.</returns>
	[HttpDelete("{userId}")]
	[ProducesResponseType(typeof(ServiceResult<bool>), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> DeleteUser([FromRoute] string userId, [FromQuery] string password, CancellationToken ct)
	{
		return await _orchestrator.HandleAdminDeleteUserAsync(userId, password, ct);
	}
}
