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
		await _emailService.SendPasswordResetEmailAsync(user.Email!, resetLink, ct);
	}
    
}


