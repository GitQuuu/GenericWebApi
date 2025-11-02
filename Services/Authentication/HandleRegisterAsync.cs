using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.Extensions.Configuration;

namespace Services.Authentication;

public partial class AuthenticationOrchestrator
{
	
	/// <summary>
	/// Defines the modes in which a user account can be activated.
	/// </summary>
	public enum UserActivationModeEnum
	{
		/// <summary>
		/// Represents an activation mode where the user is responsible for activating
		/// their own account, typically through a confirmation process such as
		/// clicking a link sent via email.
		/// </summary>
		SelfActivation,

		/// <summary>
		/// Represents an activation mode where a user's account requires explicit
		/// approval by an administrator before it becomes active.
		/// </summary>
		AdminApproval,

		/// <summary>
		/// Represents an activation mode where user accounts are activated automatically
		/// without requiring any manual action from the user or an administrator.
		/// </summary>
		AutoActivation,
	}
	
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

		// Get user activation mode: "SelfActivation", "AdminApproval", or "AutoActivation"
		string activationModeString = _configuration.GetValue<string>("Auth:Local:UserActivationMode", "SelfActivation");
		UserActivationModeEnum activationMode = Enum.TryParse<UserActivationModeEnum>(activationModeString, true, out var parsedMode)
			? parsedMode
			: UserActivationModeEnum.AdminApproval;

		switch (activationMode)
		{
			case UserActivationModeEnum.AutoActivation:
				// Auto-confirm the user's email - no email sent, user can immediately log in
				var autoToken = await _userService.GenerateEmailConfirmationTokenAsync(user);
				await _userService.ConfirmEmailAsync(user, autoToken);
				
				return await _responseService.HandleResultAsync(
					new ServiceResult<string>(true, HttpStatusCode.Created, "Registration successful. You can now log in."));

			case UserActivationModeEnum.AdminApproval:
				// User created but not confirmed - admin must manually activate, no email sent
				return await _responseService.HandleResultAsync(
					new ServiceResult<string>(true, HttpStatusCode.Created, "Registration successful. Your account is pending admin approval."));

			case UserActivationModeEnum.SelfActivation:
			default:
				// User must confirm via email link
				// Generate email confirmation token
				var confirmationToken = await _userService.GenerateEmailConfirmationTokenAsync(user);
				
				// Encode the token for URL
				var encodedToken = WebUtility.UrlEncode(confirmationToken);

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

				var emailSent = await _emailService.SendEmailAsync(email, emailSubject, emailBody, true);
				
				if (!emailSent)
				{
					// Rollback: Delete the user since email couldn't be sent
					await _userService.DeleteAsync(user);
					
					return await _responseService.HandleResultAsync(
						new ServiceResult<string>(false, HttpStatusCode.InternalServerError, "Registration failed. Unable to send confirmation email. Please try again later."));
				}

				return await _responseService.HandleResultAsync(
					new ServiceResult<string>(true, HttpStatusCode.Created, "Registration successful. Please check your email to activate your account."));
		}
	}
}
