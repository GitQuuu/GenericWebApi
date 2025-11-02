using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Home;

[ApiController]
[Route("api/[controller]")]
public class HomeController : ControllerBase
{
	private readonly ILogger<HomeController> _logger;
	private readonly IConfiguration _configuration;

	public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
	{
		_logger             = logger;
		_configuration = configuration;
	}

	/// <summary>
	/// A endpoint to test it works
	/// </summary>
	/// <returns></returns>
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

/// <summary>
/// Request model for test login endpoint.
/// </summary>
public record TestLoginRequest(string Email, string Password);