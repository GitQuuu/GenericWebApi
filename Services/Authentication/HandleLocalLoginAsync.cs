using Microsoft.AspNetCore.Mvc;
using System.Net;
using Services.Authentication.TokenService;

namespace Services.Authentication;

public partial class AuthenticationOrchestrator
{
	/// <inheritdoc />
	public async Task<IActionResult> HandleLocalLoginAsync(string email, string password, CancellationToken ctx = default)
	{
		// Validate credentials using UserService
		var isValid = await _userService.PasswordSignInAsync(email, password);
		
		if (isValid is false)
		{
			return await _responseService.HandleResultAsync(
				new ServiceResult<string>(false, HttpStatusCode.Unauthorized, "Invalid email or password"));
		}

		// Find the user by email
		var user = await _userService.FindByEmailAsync(email);
		
		if (user is null)
		{
			return await _responseService.HandleResultAsync(
				new ServiceResult<string>(false, HttpStatusCode.Unauthorized, "User not found"));
		}

		// Generate token for the authenticated user
		ServiceResult<TokenResponse> tokenResponse = await _tokenService.CreateForUserAsync(user);

		return await _responseService.HandleResultAsync(tokenResponse);
	}
}
