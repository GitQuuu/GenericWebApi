using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Services.Authentication.UserService;

public partial class UserService
{
	/// <inheritdoc />
	public async Task<ServiceResult<IEnumerable<IdentityUser>>> GetAllUsersAsync(CancellationToken ct)
	{
		var users = await _userManager.Users.ToListAsync(ct);
		if (!users.Any())
		{
			return new ServiceResult<IEnumerable<IdentityUser>>(true, HttpStatusCode.NoContent, "No users found.", []);
		}
		
		return new ServiceResult<IEnumerable<IdentityUser>>(true, HttpStatusCode.OK, "Users retrieved successfully.", users);
	}
}
