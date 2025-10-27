using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Services.Authentication;

public partial class AuthenticationOrchestrator
{
	/// <inheritdoc />
	public async Task<IActionResult> HandleResendActivationMailAsync(string email, CancellationToken ctx = default)
	{
		// Find the user by email
		var user = await _userService.FindByEmailAsync(email);
		if (user is null)
		{
			return await _responseService.HandleResultAsync(
				new ServiceResult<string>(false, HttpStatusCode.BadRequest, "If account exist a activation email will be sent. Please check your email."));
		}

		// Check if email is already confirmed
		var isEmailConfirmed = await _userService.IsEmailConfirmedAsync(user);
		if (isEmailConfirmed is false)
		{
			return await _responseService.HandleResultAsync(
				new ServiceResult<string>(false, HttpStatusCode.BadRequest, "If account exist a activation email will be sent. Please check your email."));
		}

		// Generate email confirmation token
		var confirmationToken = await _userService.GenerateEmailConfirmationTokenAsync(user);

		// Encode the token for URL
		var encodedToken = WebUtility.UrlEncode(confirmationToken);

		// Build the callback URL
		var request = _httpContextAccessor.HttpContext?.Request;
		var callbackUrl = $"{request?.Scheme}://{request?.Host}/api/Auth/ActivateUser?userId={user.Id}&token={encodedToken}";

		// Send confirmation email
		var emailSubject = "Confirm your email - Resend";
		var emailBody = $@"
			<h2>Email Confirmation</h2>
			<p>Please confirm your email address by clicking the link below:</p>
			<p><a href='{callbackUrl}'>Activate Your Account</a></p>
			<p>If you didn't request this email, please ignore it.</p>
		";

		var emailSent = await _emailService.SendEmailAsync(email, emailSubject, emailBody, true);

		if (!emailSent)
		{
			return await _responseService.HandleResultAsync(
				new ServiceResult<string>(false, HttpStatusCode.InternalServerError, "Failed to send activation email. Please try again later."));
		}

		return await _responseService.HandleResultAsync(
			new ServiceResult<string>(true, HttpStatusCode.OK, "If account exist a activation email will be sent. Please check your email."));
	}
}
