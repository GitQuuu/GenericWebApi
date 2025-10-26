using Microsoft.AspNetCore.Mvc;

namespace Services.Authentication;

public partial class AuthenticationOrchestrator
{
	public async Task<IActionResult> HandleForgotPasswordAsync(string email, CancellationToken ct)
	{
		return await _userService.ForgotPasswordAsync(email, ct);
	}
    
}


