using Microsoft.AspNetCore.Mvc;
using Services.Authentication;

namespace Api.Controllers.Auth;

/// <summary>
/// Endpoints for authentication and authorization.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public partial class AuthController : ControllerBase
{
	private readonly IAuthenticationOrchestrator _orchestrator;

	/// <inheritdoc />
	public AuthController(IAuthenticationOrchestrator orchestrator)
	{
		_orchestrator = orchestrator;
	}
}