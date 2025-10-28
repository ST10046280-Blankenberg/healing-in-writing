using System.ComponentModel.DataAnnotations;
using HealingInWriting.Domain.Events;

namespace HealingInWriting.Models.Events;

public class CreateEventViewModel
{
    public int Id { get; set; } // Add this property

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public EventType EventType { get; set; }

    [Required]
    public DateTime EventDate { get; set; }

    [Required]
    public TimeSpan StartTime { get; set; }

    [Required]
    public TimeSpan EndTime { get; set; }

    [Range(1, int.MaxValue)]
    public int Capacity { get; set; } = 50;

    // Address fields
    [StringLength(160)]
    public string StreetAddress { get; set; } = string.Empty;

    [StringLength(100)]
    public string Suburb { get; set; } = string.Empty;

    [Required]
    [StringLength(120)]
    public string City { get; set; } = string.Empty;

    [StringLength(100)]
    public string Province { get; set; } = string.Empty;

    [Required]
    [StringLength(4)]
    [RegularExpression(@"^\d{4}$")]
    public string PostalCode { get; set; } = string.Empty;

    [Required]
    public double Latitude { get; set; }

    [Required]
    public double Longitude { get; set; }

    // Tags
    public List<int> SelectedTagIds { get; set; } = new();

    // Available options for dropdowns
    public List<Domain.Shared.Tag> AvailableTags { get; set; } = new();
}