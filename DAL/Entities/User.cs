using Microsoft.AspNetCore.Identity;

namespace DAL.Entities;

public class User
{
	public int Id { get; set; }
	public string? FirstName { get; set; }
	public string? LastName { get; set; }
	public required IdentityUser IdentityUser { get; set; }
}