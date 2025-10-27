using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Services.Authentication;

public partial class AuthenticationOrchestrator
{
    
    public class DeleteUserRequestDto
    {
        public string Password { get; set; }
    }
    
    /// <inheritdoc />
    public async Task<IActionResult> HandleDeleteUserAsync(DeleteUserRequestDto request, CancellationToken ctx = default)
    {
        try
        {
            var userId = _httpContextAccessor?.HttpContext?.User.FindFirst("nameIdentifier")?.Value;
            // Find the user by ID
            if (string.IsNullOrWhiteSpace(userId))
            {
                return new BadRequestObjectResult(new { Message = "User not found." });
            }
            var user = await _userService.FindByIdAsync(userId);
            if (user is null)
            {
                return new NotFoundObjectResult(new { Message = "User not found." });
            }
            
            var promptCredentials = await _userService.PasswordSignInAsync(userId,request.Password);
            if (promptCredentials is false)
            {
                return new BadRequestObjectResult(new { Message = "Failed to prompt credentials." });
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
