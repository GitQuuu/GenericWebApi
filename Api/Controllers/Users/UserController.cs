using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Users;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
	[HttpGet]
	[Authorize]
	public async Task<IActionResult> GetProfile()
	{
		var claims = User.Claims.Select(c => new { c.Type, c.Value });
		return Ok(claims);
	}
}