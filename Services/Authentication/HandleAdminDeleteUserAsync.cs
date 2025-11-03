using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Services.Authentication;

public partial class AuthenticationOrchestrator
{
    /// <inheritdoc />
    public async Task<IActionResult> HandleAdminDeleteUserAsync(string userId, string password, CancellationToken ctx = default)
    {
        try
        {
            // Get the current admin user ID from the HTTP context
            var adminUserId = _httpContextAccessor?.HttpContext?.User.FindFirst("nameIdentifier")?.Value;
            if (string.IsNullOrWhiteSpace(adminUserId))
            {
                return new UnauthorizedObjectResult(new { Message = "Admin user not found." });
            }

            // Verify admin's password
            if (string.IsNullOrWhiteSpace(password))
            {
                return new BadRequestObjectResult(new { Message = "Admin password is required." });
            }
            
            var passwordValid = await _userService.PasswordSignInAsync(adminUserId, password);
            if (!passwordValid)
            {
                return new UnauthorizedObjectResult(new { Message = "Invalid admin password." });
            }

            // Validate userId
            if (string.IsNullOrWhiteSpace(userId))
            {
                return new BadRequestObjectResult(new { Message = "User ID is required." });
            }

            // Prevent admin from deleting themselves
            if (adminUserId == userId)
            {
                return new BadRequestObjectResult(new { Message = "You cannot delete your own account." });
            }

            // Find the user to delete by ID
            var user = await _userService.FindByIdAsync(userId);
            if (user is null)
            {
                return new NotFoundObjectResult(new { Message = "User not found." });
            }

            // Delete the user
            var result = await _userService.DeleteAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new BadRequestObjectResult(new { Message = $"Failed to delete user: {errors}" });
            }

            return new OkObjectResult(new { Message = "User deleted successfully." });
        }
        catch (Exception ex)
        {
            // Log the exception (you might want to inject ILogger and log the actual exception)
            return new ObjectResult(new { Message = "An error occurred while deleting the user." })
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
    }
}
