using HealingInWriting.Domain.Users;
using HealingInWriting.Domain.Volunteers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HealingInWriting.Data;

/// <summary>
/// Database initialiser for seeding test accounts and roles.
/// Creates 3 test accounts: User, Volunteer, and Admin.
/// </summary>
public static class DbInitialiser
{
    /// <summary>
    /// Initialises the database with roles and test accounts.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="userManager">The user manager for creating users.</param>
    /// <param name="roleManager">The role manager for creating roles.</param>
    public static async Task InitialiseAsync(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        // Ensure database is created
        await context.Database.MigrateAsync();

        // Check if roles already exist
        if (await roleManager.Roles.AnyAsync())
        {
            return; // Database already seeded
        }

        // Create roles
        var roles = new[] { "User", "Volunteer", "Admin" };
        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Create test accounts
        await CreateTestUserAsync(userManager, "user@test.com", "Test", "User", "User");
        await CreateTestUserAsync(userManager, "volunteer@test.com", "Test", "Volunteer", "Volunteer");
        await CreateTestUserAsync(userManager, "admin@test.com", "Test", "Admin", "Admin");

        //Seed Volunteer entity for volunteer user
        var volunteerUser = await userManager.FindByEmailAsync("volunteer@test.com");
        if (volunteerUser != null)
        {
            // Check if Volunteer already exists
            if (!context.Volunteers.Any(v => v.UserId == volunteerUser.Id))
            {
                context.Volunteers.Add(new Volunteer
                {
                    UserId = volunteerUser.Id,
                    EnrolledAt = DateTime.UtcNow,
                    IsActive = true
                });
                await context.SaveChangesAsync();
            }
        }
    }

    /// <summary>
    /// Creates a test user account with email verification pre-confirmed.
    /// </summary>
    /// <param name="userManager">The user manager.</param>
    /// <param name="email">The user's email address.</param>
    /// <param name="firstName">The user's first name.</param>
    /// <param name="lastName">The user's last name.</param>
    /// <param name="role">The role to assign to the user.</param>
    private static async Task CreateTestUserAsync(
        UserManager<ApplicationUser> userManager,
        string email,
        string firstName,
        string lastName,
        string role)
    {
        // Check if user already exists
        if (await userManager.FindByEmailAsync(email) != null)
        {
            return;
        }

        // Create user
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            EmailConfirmed = true, // Pre-confirm email for test accounts
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        // Password: Password123! (meets all requirements)
        var result = await userManager.CreateAsync(user, "Password123!");

        if (result.Succeeded)
        {
            // Assign role
            await userManager.AddToRoleAsync(user, role);
        }
    }
}
