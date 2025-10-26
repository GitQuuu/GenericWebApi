using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Services.Authentication;

public partial class AuthenticationOrchestrator
{
	public async Task<IActionResult> HandleForgotPasswordAsync(string email, CancellationToken ct)
	{
		ServiceResult<Tuple<IdentityUser, string>> forgotPasswordResult = await _userService.ForgotPasswordAsync(email, ct);
		if (forgotPasswordResult.Success is false)
		{
			return await _responseService.HandleResultAsync(forgotPasswordResult);
		}
		
		// Build the callback URL
		var request     = _httpContextAccessor.HttpContext?.Request;
		
		// Create reset link (adjust the URL based on your frontend)
		var callbackUrl = $"{request?.Scheme}://{request?.Host}/api/Auth/ResetPassword?userId={forgotPasswordResult.Data.Item1.Id}&token={Uri.EscapeDataString(forgotPasswordResult.Data.Item2)}";
		
		// Send confirmation email
		var emailSubject = "Confirm your email";
		var emailBody = $@"
					<h2>Click link to reset password!</h2>
					<p>Please reset your password by clicking the link below:</p>
					<p><a href='{callbackUrl}'>Activate Your Account</a></p>
					<p>If you didn't request this, please change your password.</p>
				";
		
		// Send email with reset link
		var sendEmailResult = await _emailService.SendEmailAsync(forgotPasswordResult.Data.Item1.Email,emailSubject ,emailBody, true);
		
		if (sendEmailResult is false)
		{
			return await _responseService.HandleResultAsync(
				new ServiceResult<string>(false, HttpStatusCode.InternalServerError, "Failed to send reset link email."));
		}
		
		return new OkResult();
	}
    
}


