using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Services.Authentication;

public partial class AuthenticationOrchestrator
{
	/// <inheritdoc />
	public async Task<IActionResult> HandleGetAllUsersAsync(CancellationToken ct = default)
	{
		ServiceResult<IEnumerable<IdentityUser>> result = await _userService.GetAllUsersAsync(ct);
		
		if (!result.Success)
		{
			return await _responseService.HandleResultAsync(result);
		}

		List<GetAllUsersResponse> users = result.Data.Adapt<List<GetAllUsersResponse>>();
		
		return await _responseService.HandleResultAsync(
			new ServiceResult<List<GetAllUsersResponse>>(true, result.HttpResponse, result.Message, users)
		);
	}
}
