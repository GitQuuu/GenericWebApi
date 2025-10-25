using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Auth;

public partial class AuthController
{
	/// <summary>
	/// Request model for local login.
	/// </summary>
	public record LoginRequest(string Email, string Password);
	
	/// <summary>
	/// Authenticates a user with email and password credentials.
	/// </summary>
	/// <param name="request">The login request containing email and password.</param>
	/// <param name="ct">A CancellationToken to observe while waiting for the operation to complete.</param>
	/// <remarks>
	/// <para># Local Authentication</para>
	/// <para>This endpoint validates user credentials against the local identity store using ASP.NET Core Identity.</para>
	/// <para>The authentication process includes:</para>
	/// <para>- Validation of email and password credentials</para>
	/// <para>- Enforcement of account lockout policies</para>
	/// <para>- Tracking of failed login attempts</para>
	/// <para>Upon successful authentication, a JWT token is issued for subsequent API requests.</para>
	/// </remarks>
	/// <returns>An IActionResult containing either a successful response with the JWT token or an error if authentication fails.</returns>
	[HttpPost("Login")]
	public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
	{
		return await _orchestrator.HandleLocalLoginAsync(request.Email, request.Password, ct);
	}
}


