using System.Security.Claims;
using HealingInWriting.Data;
using HealingInWriting.Domain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace HealingInWriting.Services.Auth;

/// <summary>
/// Custom claims principal factory to add FirstName, LastName, and ProfileId to user claims.
/// This makes them accessible in views via User.Identity.
/// </summary>
public class ApplicationUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
{
    private readonly ApplicationDbContext _context;

    public ApplicationUserClaimsPrincipalFactory(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IOptions<IdentityOptions> optionsAccessor,
        ApplicationDbContext context)
        : base(userManager, roleManager, optionsAccessor)
    {
        _context = context;
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        var identity = await base.GenerateClaimsAsync(user); // This adds role claims!

        identity.AddClaim(new Claim(ClaimTypes.GivenName, user.FirstName ?? string.Empty));
        identity.AddClaim(new Claim("FirstName", user.FirstName ?? string.Empty));
        identity.AddClaim(new Claim("LastName", user.LastName ?? string.Empty));
        identity.AddClaim(new Claim("FullName", $"{user.FirstName} {user.LastName}"));

        // Add ProfileId claim if UserProfile exists
        var userProfile = await _context.UserProfiles
            .FirstOrDefaultAsync(up => up.UserId == user.Id);

        if (userProfile != null)
        {
            identity.AddClaim(new Claim("ProfileId", userProfile.ProfileId.ToString()));
        }

        return identity;
    }
}
