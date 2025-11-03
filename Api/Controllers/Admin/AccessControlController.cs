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
public partial class AccessControlController : ControllerBase
{
	private readonly IAuthenticationOrchestrator _orchestrator;

	/// <inheritdoc />
	public AccessControlController(IAuthenticationOrchestrator orchestrator)
	{
		_orchestrator = orchestrator;
	}
}
