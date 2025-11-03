using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Admin;

public partial class AdminUserController
{
	/// <summary>
	/// Gets a list of all users in the system (Admin only).
	/// </summary>
	/// <param name="ct">A CancellationToken to observe while waiting for the operation to complete.</param>
	/// <remarks>
	/// <para># Get All Users</para>
	/// <para>This endpoint allows administrators to retrieve a list of all registered users in the system.</para>
	/// <para>The response includes:</para>
	/// <para>- User ID</para>
	/// <para>- Email address</para>
	/// <para>- Username</para>
	/// <para>- Email confirmation status</para>
	/// <para>- Other user properties</para>
	/// </remarks>
	/// <returns>An IActionResult containing the list of all users.</returns>
	[HttpGet]
	[ProducesResponseType(typeof(IdentityUser),StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> GetAllUsers(CancellationToken ct)
	{
		return await _orchestrator.HandleGetAllUsersAsync(ct);
	}
}
