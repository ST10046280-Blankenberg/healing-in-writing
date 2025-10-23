using System.Security.Claims;
using HealingInWriting.Domain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace HealingInWriting.Services.Auth;

/// <summary>
/// Custom claims principal factory to add FirstName and LastName to user claims.
/// This makes them accessible in views via User.Identity.
/// </summary>
public class ApplicationUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser>
{
    public ApplicationUserClaimsPrincipalFactory(
        UserManager<ApplicationUser> userManager,
        IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, optionsAccessor)
    {
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);

        // Add custom claims for FirstName and LastName
        identity.AddClaim(new Claim("FirstName", user.FirstName ?? string.Empty));
        identity.AddClaim(new Claim("LastName", user.LastName ?? string.Empty));
        identity.AddClaim(new Claim("FullName", $"{user.FirstName} {user.LastName}"));

        return identity;
    }
}
