using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HomeController : ControllerBase
{
	private readonly ILogger<HomeController> _logger;

	public HomeController(ILogger<HomeController> logger)
	{
		_logger = logger;
	}

	[HttpGet]
	public IActionResult Index()
	{
		return Ok("Get works");
	}
	
	/// <summary>
	/// Protected endpoint
	/// </summary>
	/// <returns></returns>
	[HttpGet("Protected")]
	[Authorize]
	public IActionResult Protected()
	{
		return Ok("Protected endpoint authorized");
	}


}