using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Services.Authentication;

public partial class AuthenticationOrchestrator
{
	/// <inheritdoc />
	public async Task<IActionResult> HandleGetAllUsersAsync(CancellationToken ct = default)
	{
		var                       result = await _userService.GetAllUsersAsync(ct);
		return await _responseService.HandleResultAsync(result);
	}
}
