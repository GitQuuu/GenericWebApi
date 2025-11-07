using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Authentication;

namespace Api.Controllers.Users;

/// <summary>
/// Endpoints for user management operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public partial class UserController : ControllerBase
{
	private readonly IAuthenticationOrchestrator _orchestrator;

	/// <inheritdoc />
	public UserController(IAuthenticationOrchestrator orchestrator)
	{
		_orchestrator = orchestrator;
	}

	[HttpGet]
	[Authorize]
	public async Task<IActionResult> GetProfile()
	{
		var claims = User.Claims.Select(c => new { c.Type, c.Value });
		return Ok(claims);
	}
}