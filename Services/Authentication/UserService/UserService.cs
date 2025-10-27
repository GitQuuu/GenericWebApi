using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Services.Authentication.UserService;

/// <summary>
/// Service for managing user operations, abstracting UserManager and SignInManager functionality.
/// </summary>
public class UserService : IUserService
{
	private readonly UserManager<IdentityUser> _userManager;
	private readonly SignInManager<IdentityUser> _signInManager;

	public UserService(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
	{
		_userManager   = userManager;
		_signInManager = signInManager;
	}

	/// <inheritdoc />
	public async Task<IdentityUser?> FindByIdAsync(string userId)
	{
		return await _userManager.FindByIdAsync(userId);
	}

	/// <inheritdoc />
	public async Task<IList<string>> GetRolesAsync(IdentityUser user)
	{
		return await _userManager.GetRolesAsync(user);
	}

	/// <inheritdoc />
	public async Task<IdentityUser?> FindByLoginAsync(string loginProvider, string providerKey)
	{
		return await _userManager.FindByLoginAsync(loginProvider, providerKey);
	}

	/// <inheritdoc />
	public async Task<IdentityUser?> FindByEmailAsync(string email)
	{
		return await _userManager.FindByEmailAsync(email);
	}

	/// <inheritdoc />
	public async Task<IdentityResult> CreateAsync(IdentityUser user)
	{
		return await _userManager.CreateAsync(user);
	}

	/// <inheritdoc />
	public async Task<IList<UserLoginInfo>> GetLoginsAsync(IdentityUser user)
	{
		return await _userManager.GetLoginsAsync(user);
	}

	/// <inheritdoc />
	public async Task<IdentityResult> AddLoginAsync(IdentityUser user, UserLoginInfo login)
	{
		return await _userManager.AddLoginAsync(user, login);
	}

	/// <inheritdoc />
	public async Task<bool> PasswordSignInAsync(IdentityUser user, string password)
	{
		var result = await _signInManager.PasswordSignInAsync(user, password, false, lockoutOnFailure : false);
		return result.Succeeded;
	}

	/// <inheritdoc />
	public async Task<bool> PasswordSignInAsync(string email, string password)
	{
		var result = await _signInManager.PasswordSignInAsync(email, password, isPersistent : false, lockoutOnFailure : false);
		return result.Succeeded;
	}

	/// <inheritdoc />
	public async Task<IdentityResult> CreateUserAsync(string email, string password)
	{
		var user = new IdentityUser
		{
			UserName = email,
			Email = email
		};
		return await _userManager.CreateAsync(user, password);
	}

	/// <inheritdoc />
	public async Task<string> GenerateEmailConfirmationTokenAsync(IdentityUser user)
	{
		return await _userManager.GenerateEmailConfirmationTokenAsync(user);
	}

	/// <inheritdoc />
	public async Task<IdentityResult> ConfirmEmailAsync(IdentityUser user, string token)
	{
		return await _userManager.ConfirmEmailAsync(user, token);
	}

	/// <inheritdoc />
	public async Task<IdentityResult> DeleteAsync(IdentityUser user)
	{
		return await _userManager.DeleteAsync(user);
	}

	public async Task<ServiceResult<Tuple<IdentityUser, string>>> ForgotPasswordAsync(string email, CancellationToken ct)
	{
		// Find user by email
		var user = await _userManager.FindByEmailAsync(email);
	
		// For security, return success regardless of whether user exists
		if (user == null)
		{
			return new ServiceResult<Tuple<IdentityUser, string>>(false, HttpStatusCode.NoContent, "If the email exists, a password reset link has been sent.");
		}
	
		// Generate password reset token
		var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

		if (resetToken is null)
		{
			return new ServiceResult<Tuple<IdentityUser, string>>(false, HttpStatusCode.InternalServerError, "Failed to generate password reset token.");
		}
	
		return new ServiceResult<Tuple<IdentityUser, string>>(true, HttpStatusCode.OK, "If the email exists, a password reset link has been sent.", new Tuple<IdentityUser, string>(user, resetToken));
	
	}

	public async Task<ServiceResult<IdentityResult>> ResetPasswordAsync(string userId, string decodedToken, string requestNewPassword, CancellationToken ct)
	{
		// Find user by ID
		var user = await _userManager.FindByIdAsync(userId);

		if (user is null)
		{
			return new ServiceResult<IdentityResult>(false, HttpStatusCode.NotFound, "User not found.");
		}

		// Attempt to reset the password using the decoded token
		IdentityResult result = await _userManager.ResetPasswordAsync(user, decodedToken, requestNewPassword);

		if (result.Succeeded is false)
		{
			// Extract error messages from IdentityResult
			var errors = string.Join(", ", result.Errors.Select(e => e.Description));
			return new ServiceResult<IdentityResult>(false, HttpStatusCode.BadRequest, $"Password reset failed: {errors}");
		}

		return new ServiceResult<IdentityResult>(true, HttpStatusCode.OK, "Password has been successfully reset.", result);
	}
	
	
	/// <inheritdoc />
	public async Task<bool> IsEmailConfirmedAsync(IdentityUser user)
	{
		return user.EmailConfirmed;
	}
}