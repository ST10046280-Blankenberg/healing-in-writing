using System.ComponentModel.DataAnnotations;

namespace HealingInWriting.Models.Volunteer;

public class LogHoursViewModel
{
    [Required]
    [DataType(DataType.Date)]
    public DateOnly Date { get; set; }

    [Required]
    [Range(0.1, 24)]
    public double Hours { get; set; }

    [Required]
    [StringLength(200)]
    public string Activity { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Notes { get; set; }

    public IFormFile? Attachment { get; set; }
}