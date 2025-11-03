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
        var roles = _configuration.GetSection("Seed:Roles").Get<string[]>();
        if (roles == null || roles.Length == 0)
        {
            _logger.LogWarning("No roles configured in Seed:Roles. Skipping role seeding.");
            return;
        }
        
        foreach (var role in roles)
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
        var email = _configuration["Seed:Admin:Email"];
        var password = _configuration["Seed:Admin:Password"];
        
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            throw new InvalidOperationException("Admin email and password must be configured in Seed:Admin section.");
        }

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
        var adminRoles = _configuration.GetSection("Seed:AdminRoles").Get<string[]>();
        if (adminRoles == null || adminRoles.Length == 0)
        {
            _logger.LogWarning("No admin roles configured in Seed:AdminRoles. Skipping role assignment.");
            return;
        }
        
        foreach (var role in adminRoles)
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
        
        var firstName = _configuration["Seed:Admin:FirstName"];
        var lastName = _configuration["Seed:Admin:LastName"];
        
        if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
        {
            throw new InvalidOperationException("Admin FirstName and LastName must be configured in Seed:Admin section.");
        }
        
        User user = new ()
        {
            IdentityUserId = aspNetUser.Id,
            FirstName      = firstName,
            LastName       = lastName,
            IdentityUser   = aspNetUser,
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
    }
}
