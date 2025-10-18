using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealingInWriting.Domain.Users;

/// <summary>
/// Extended profile information for application users.
/// Maintains a 1-to-1 relationship with ApplicationUser.
/// </summary>
public class UserProfile
{
    [Key]
    public int ProfileId { get; set; }

    [Required]
    public string UserId { get; set; }

    public int? AddressId { get; set; }

    public string Bio { get; set; }

    public DateTime? DateOfBirth { get; set; }

    [StringLength(120)]
    public string City { get; set; }

    // Navigation properties
    [ForeignKey(nameof(UserId))]
    public ApplicationUser User { get; set; }

    [ForeignKey(nameof(AddressId))]
    public Shared.Address Address { get; set; }
}
