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

		var response = new GetAllUsersResponse
		{
			Users = result.Data.Adapt<List<GetAllUsersResponse.UserDto>>(),
			TotalCount = result.Data.Count()
		};
		
		return await _responseService.HandleResultAsync(
			new ServiceResult<GetAllUsersResponse>(true, result.HttpResponse, result.Message, response)
		);
	}
}
