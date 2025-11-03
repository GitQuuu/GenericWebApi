using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Authentication;

namespace Api.Controllers.Admin;

/// <summary>
/// Endpoints for admin user management operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public partial class AdminUserController : ControllerBase
{
	private readonly IAuthenticationOrchestrator _orchestrator;

	/// <inheritdoc />
	public AdminUserController(IAuthenticationOrchestrator orchestrator)
	{
		_orchestrator = orchestrator;
	}
}
