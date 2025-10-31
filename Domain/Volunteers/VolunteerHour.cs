using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealingInWriting.Domain.Volunteers;

public enum VolunteerHourStatus
{
    Pending,
    Approved,
    Rejected,
    NeedsInfo
}

public class VolunteerHour
{
    [Key]
    public Guid Id { get; set; }

    // Foreign key to Volunteer
    [Required]
    public int VolunteerId { get; set; }

    [ForeignKey(nameof(VolunteerId))]
    public Volunteer Volunteer { get; set; }

    // Redundant, but useful for display (denormalized)
    [NotMapped]
    public string VolunteerName =>
        Volunteer != null
            ? $"{Volunteer?.User?.FirstName} {Volunteer?.User?.LastName}"
            : string.Empty;

    [Required]
    public DateOnly Date { get; set; }

    [Required]
    [StringLength(200)]
    public string Activity { get; set; } = string.Empty;

    [Required]
    [Range(0.1, 24)]
    public double Hours { get; set; }

    public string? AttachmentUrl { get; set; }

    [Required]
    public VolunteerHourStatus Status { get; set; } = VolunteerHourStatus.Pending;

    [Required]
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ReviewedAt { get; set; }

    public string? ReviewedBy { get; set; }

    public string? Comments { get; set; }

    // Domain validation
    public void Validate()
    {
        if (Hours <= 0)
            throw new ArgumentException("Hours must be greater than zero.");
        if (string.IsNullOrWhiteSpace(Activity))
            throw new ArgumentException("Activity is required.");
        if (Date > DateOnly.FromDateTime(DateTime.UtcNow))
            throw new ArgumentException("Date cannot be in the future.");
    }
}
