namespace Services.Authentication;

public class GetAllUsersResponse
{
	public required List<UserDto> Users { get; init; }
	public required int TotalCount { get; init; }

	public class UserDto
	{
		public required string Email { get; init; }
		public required string UserName { get; init; }
		public bool EmailConfirmed { get; init; }
		public string? PhoneNumber { get; init; }
		public bool PhoneNumberConfirmed { get; init; }
		public bool TwoFactorEnabled { get; init; }
		public DateTimeOffset? LockoutEnd { get; init; }
		public bool LockoutEnabled { get; init; }
		public int AccessFailedCount { get; init; }
	}
}