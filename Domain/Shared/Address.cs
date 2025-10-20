using System.ComponentModel.DataAnnotations;

namespace HealingInWriting.Domain.Shared;

/// <summary>
/// Represents a physical address for users and events, including geolocation data.
/// </summary>
public class Address
{
    /// <summary>
    /// Unique identifier for the address.
    /// </summary>
    [Key]
    public int AddressId { get; set; }

    /// <summary>
    /// Street address or building number and name.
    /// </summary>
    [StringLength(160)]
    public string StreetAddress { get; set; }

    /// <summary>
    /// Suburb or neighborhood of the address.
    /// </summary>
    [StringLength(100)]
    public string Suburb { get; set; }

    /// <summary>
    /// City or town of the address.
    /// </summary>
    [Required]
    [StringLength(120)]
    public string City { get; set; }

    /// <summary>
    /// Province, state, or region of the address.
    /// </summary>
    [StringLength(100)]
    public string Province { get; set; }

    /// <summary>
    /// Postal code (must be a 4-digit number).
    /// </summary>
    [Required]
    [StringLength(4)]
    [RegularExpression(@"^\d{4}$", ErrorMessage = "Postal code must be a 4-digit number.")]
    public string PostalCode { get; set; }

    /// <summary>
    /// Country of the address. Defaults to "South Africa".
    /// </summary>
    [Required]
    [StringLength(120)]
    public string Country { get; set; } = "South Africa";

    /// <summary>
    /// Latitude coordinate for geolocation.
    /// </summary>
    [Required]
    public double Latitude { get; set; }

    /// <summary>
    /// Longitude coordinate for geolocation.
    /// </summary>
    [Required]
    public double Longitude { get; set; }

    // Navigation properties
    // public ICollection<UserProfile> UserProfiles { get; set; }
    // public ICollection<Event> Events { get; set; }
}
