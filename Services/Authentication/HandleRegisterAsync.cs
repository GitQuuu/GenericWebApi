using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Services.Authentication;

public partial class AuthenticationOrchestrator
{
	/// <inheritdoc />
	public async Task<IActionResult> HandleRegisterAsync(string email, string password, CancellationToken ctx = default)
	{
		// Check if user already exists
		var existingUser = await _userService.FindByEmailAsync(email);
		if (existingUser is not null)
		{
			return await _responseService.HandleResultAsync(
				new ServiceResult<string>(false, HttpStatusCode.Conflict, "User with this email already exists"));
		}

		// Create the user
		var createResult = await _userService.CreateUserAsync(email, password);
		if (!createResult.Succeeded)
		{
			var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
			return await _responseService.HandleResultAsync(
				new ServiceResult<string>(false, HttpStatusCode.BadRequest, $"Failed to create user: {errors}"));
		}

		// Find the newly created user
		var user = await _userService.FindByEmailAsync(email);
		if (user is null)
		{
			return await _responseService.HandleResultAsync(
				new ServiceResult<string>(false, HttpStatusCode.InternalServerError, "User created but could not be retrieved"));
		}

		// Generate email confirmation token
		var token = await _userService.GenerateEmailConfirmationTokenAsync(user);
		
		// Encode the token for URL
		var encodedToken = WebUtility.UrlEncode(token);

		// Build the callback URL
		var request = _httpContextAccessor.HttpContext?.Request;
		var callbackUrl = $"{request?.Scheme}://{request?.Host}/api/Auth/ActivateUser?userId={user.Id}&token={encodedToken}";

		// Send confirmation email
		var emailSubject = "Confirm your email";
		var emailBody = $@"
			<h2>Welcome to our platform!</h2>
			<p>Please confirm your email address by clicking the link below:</p>
			<p><a href='{callbackUrl}'>Activate Your Account</a></p>
			<p>If you didn't create this account, please ignore this email.</p>
		";

		var emailSent = await _emailService.SendEmailAsync(email, emailSubject, emailBody,true);
		
		if (!emailSent)
		{
			return await _responseService.HandleResultAsync(
				new ServiceResult<string>(false, HttpStatusCode.InternalServerError, "User created but confirmation email could not be sent"));
		}

		return await _responseService.HandleResultAsync(
			new ServiceResult<string>(true, HttpStatusCode.Created, "Registration successful. Please check your email to activate your account."));
	}
}
