using System.ComponentModel.DataAnnotations;

namespace HealingInWriting.Domain.Shared;

/// <summary>
/// Represents a physical address for users and events.
/// </summary>
public class Address
{
    [Key]
    public int AddressId { get; set; }

    [StringLength(160)]
    public string StreetAddress { get; set; }

    [StringLength(100)]
    public string Suburb { get; set; }

    [Required]
    [StringLength(120)]
    public string City { get; set; }

    [StringLength(100)]
    public string Province { get; set; }

    [Required]
    [StringLength(4)]
    [RegularExpression(@"^\d{4}$", ErrorMessage = "Postal code must be a 4-digit number.")]
    public string PostalCode { get; set; }

    [Required]
    [StringLength(120)]
    public string Country { get; set; } = "South Africa";

    [Required]
    public double Latitude { get; set; }
    [Required]
    public double Longitude { get; set; }

    // Navigation properties
    // public ICollection<UserProfile> UserProfiles { get; set; }
    // public ICollection<Event> Events { get; set; }
}
