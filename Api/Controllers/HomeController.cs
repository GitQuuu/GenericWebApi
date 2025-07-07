using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Api.Controllers;

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

	/// <summary>
	/// Login endpoint to authenticate users and return a JWT token.
	/// </summary>
	/// <param name="loginRequest">The login request containing username and password.</param>
	/// <returns>A JWT token if authentication is successful.</returns>
	[HttpPost("Login")]
	public IActionResult Login([FromBody] LoginRequest loginRequest)
	{
		// Replace this with your actual user authentication logic
		if (loginRequest.Email == "TestUser" && loginRequest.Password == "Password123")
		{
			var token = GenerateJwtToken(loginRequest.Email);
			return Ok(new { Token = token });
		}

		return Unauthorized("Invalid username or password");
	}

	private string GenerateJwtToken(string username)
	{
		var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
		var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

		var claims = new[]
		{
			new Claim(ClaimTypes.Name, username),
			new Claim(ClaimTypes.Role, "User")
		};

		var token = new JwtSecurityToken(
										 _configuration["Jwt:Issuer"],
										 _configuration["Jwt:Audience"],
										 claims,
										 expires: DateTime.Now.AddHours(1),
										 signingCredentials: credentials);

		return new JwtSecurityTokenHandler().WriteToken(token);
	}

	
}