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
		var resetLink = $"{request?.Scheme}://{request?.Host}/api/Auth/reset-password?userId={forgotPasswordResult.Data.Item1.Id}&token={Uri.EscapeDataString(forgotPasswordResult.Data.Item2)}";
	
		// Send email with reset link
		var sendEmailResult = await _emailService.SendEmailAsync(forgotPasswordResult.Data.Item1.Email,"Activation link" ,resetLink, true);
		
		if (sendEmailResult is false)
		{
			return await _responseService.HandleResultAsync(
				new ServiceResult<string>(false, HttpStatusCode.InternalServerError, "Failed to send reset link email."));
		}
		
		return new OkResult();
	}
    
}


