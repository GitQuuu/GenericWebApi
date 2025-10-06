using DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class IdentitySeeder
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<IdentitySeeder> _logger;

    private const string DefaultAdminEmail = "admin@example.com";
    private const string DefaultAdminPassword = "Admin@1234";
    private static readonly string[] Roles = { "Admin", "User" };
    private static readonly string[] AdminUserRoles = { "Admin" };

    public IdentitySeeder(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration,
        ApplicationDbContext dbContext,
        ILogger<IdentitySeeder> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        _logger.LogInformation("Starting identity seeding...");
        await SeedRolesAsync();
        var adminUser = await GetOrCreateAdminUserAsync();
        await AssignRolesToAdminUserAsync(adminUser);
        await CreateUserProfileIfMissingAsync(adminUser);
        _logger.LogInformation("Identity seeding complete.");
    }

    private async Task SeedRolesAsync()
    {
        foreach (var role in Roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                _logger.LogInformation("Creating role: {Role}", role);
                await _roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    private async Task<IdentityUser> GetOrCreateAdminUserAsync()
    {
        var email = _configuration["Seed:Admin:Email"] ?? DefaultAdminEmail;
        var password = _configuration["Seed:Admin:Password"] ?? DefaultAdminPassword;

        var user = await _userManager.FindByEmailAsync(email);
        if (user != null)
        {
            _logger.LogInformation("Admin user already exists: {Email}", email);
            return user;
        }

        _logger.LogInformation("Creating admin user: {Email}", email);
        user = new IdentityUser
        {
            Email = email,
            UserName = email,
            EmailConfirmed = true,
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new Exception($"Failed to create admin user: {errors}");
        }

        return user;
    }

    private async Task AssignRolesToAdminUserAsync(IdentityUser user)
    {
        foreach (var role in AdminUserRoles)
        {
            if (!await _userManager.IsInRoleAsync(user, role))
            {
                _logger.LogInformation("Assigning role {Role} to user {Email}", role, user.Email);
                await _userManager.AddToRoleAsync(user, role);
            }
        }
    }

    private async Task CreateUserProfileIfMissingAsync(IdentityUser aspNetUser)
    {
        var exists = await _dbContext.Users
            .AnyAsync(p => p.IdentityUserId == aspNetUser.Id);

        if (exists)
        {
            _logger.LogInformation("UserProfile already exists for {Email}", aspNetUser.Email);
            return;
        }

        _logger.LogInformation("Creating UserProfile for {Email}", aspNetUser.Email);
        User user = new ()
        {
            IdentityUserId = aspNetUser.Id,
            FirstName      = "Admin",
            LastName       = "Example",
            IdentityUser   = aspNetUser,
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
    }
}
