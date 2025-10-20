namespace HealingInWriting.Domain.Security;

/// <summary>
/// Defines the role-based access levels for the platform.
/// </summary>
public enum Role
{
    /// <summary>
    /// Guest users with limited read-only access
    /// </summary>
    Guest = 0,

    /// <summary>
    /// Regular authenticated users with standard access
    /// </summary>
    User = 1,

    /// <summary>
    /// Administrators with full system access
    /// </summary>
    Admin = 2
}
