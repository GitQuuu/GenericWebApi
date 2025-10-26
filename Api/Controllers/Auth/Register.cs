using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Auth;

public partial class AuthController
{
	/// <summary>
	/// Request model for user registration.
	/// </summary>
	public record RegisterRequest
	{
		[Required]
		[EmailAddress]
		public required string Email { get; init; }
		
		[Required]
		[MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
		public required string Password { get; init; }
		
		[Required]
		[Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
		public required string ConfirmPassword { get; init; }
	}

	/// <summary>
	/// Registers a new user account with email and password.
	/// </summary>
	/// <param name="request">The registration request containing email and password.</param>
	/// <param name="ct">A CancellationToken to observe while waiting for the operation to complete.</param>
	/// <remarks>
	/// <para># User Registration Flow</para>
	/// <para>This endpoint creates a new user account and initiates the email confirmation process:</para>
	/// <para>- Validates that the email is not already registered</para>
	/// <para>- Creates the user account with the provided credentials</para>
	/// <para>- Generates an email confirmation token</para>
	/// <para>- Sends a confirmation email with an activation link</para>
	/// <para>The user must click the activation link in the email to confirm their account before they can log in.</para>
	/// <para>The activation link will direct to the ActivateUser endpoint with the necessary token.</para>
	/// </remarks>
	/// <returns>An IActionResult indicating success or failure of the registration process.</returns>
	[HttpPost("Register")]
	public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
	{
		return await _orchestrator.HandleRegisterAsync(request.Email, request.Password, ct);
	}
}
