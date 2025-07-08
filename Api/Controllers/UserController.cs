using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
	[HttpGet]
	[Authorize]
	public async Task<IActionResult> Get()
	{
		return Ok("Get works");
	}
}