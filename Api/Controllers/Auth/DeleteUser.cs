using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Authentication;

namespace Api.Controllers.Auth;

public partial class AuthController
{
	
	public class DeleteUserRequest
	{
		public string Password { get; set; }
	}
	
    /// <summary>
    /// Deletes a user account.
    /// </summary>
    /// <param name="request">User password for confirmation.</param>
    /// <param name="ctx">The cancellation token.</param>
    /// <returns>An IActionResult indicating the result of the operation.</returns>
    [HttpDelete]
    [Authorize] 
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteUser([FromBody] DeleteUserRequest request, CancellationToken ctx = default)
    {
		return await _orchestrator.HandleDeleteUserAsync(request.Adapt<AuthenticationOrchestrator.DeleteUserRequestDto>(), ctx);
    }


}
