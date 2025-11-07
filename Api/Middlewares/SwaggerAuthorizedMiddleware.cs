using System.Text;
using Microsoft.AspNetCore.Authentication;

namespace Api.Middlewares;

/// <summary>
/// Middleware to enforce authentication on Swagger endpoints with basic authorization.
/// </summary>
public class SwaggerAuthorizedMiddleware
{
	private readonly RequestDelegate _next;
	private readonly IConfiguration _configuration;

	public SwaggerAuthorizedMiddleware(RequestDelegate next, IConfiguration configuration)
	{
		_next = next;
		_configuration = configuration;
	}

	public async Task Invoke(HttpContext context)
	{
		if (context.Request.Path.StartsWithSegments("/swagger")
		 && !context.User.Identity.IsAuthenticated)
		{
			string authHeader = context.Request.Headers["Authorization"];
			if (authHeader != null && authHeader.StartsWith("Basic "))
			{
				// Get the encoded username and password
				var encodedUsernamePassword = authHeader.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries)[1]?.Trim();

				// Decode from Base64 to string
				var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));

				// Split username and password
				var username = decodedUsernamePassword.Split(':', 2)[0];
				var password = decodedUsernamePassword.Split(':', 2)[1];

				// Check if login is correct
				if (IsAuthorized(username, password))
				{
					await _next.Invoke(context);
					return;
				}
			}
			// context.Response.StatusCode = StatusCodes.Status401Unauthorized;
			await context.ChallengeAsync();
			// Return authentication type (causes browser to show login dialog)
			context.Response.Headers["WWW-Authenticate"] = "Basic";
			return;
		}

		await _next.Invoke(context);
	}

	/// <summary>
	/// Checks if the provided username and password are authorized.
	/// </summary>
	/// <param name="username">The username to check.</param>
	/// <param name="password">The password to check.</param>
	/// <returns>True if the username and password are authorized, false otherwise.</returns>
	public bool IsAuthorized(string username, string password)
	{
		// Get credentials from configuration
		var configUsername = _configuration["SwaggerAuth:Username"];
		var configPassword = _configuration["SwaggerAuth:Password"];
		
		// Check that username and password are correct
		return username.Equals(configUsername, StringComparison.InvariantCultureIgnoreCase)
			&& password.Equals(configPassword);
	}
}