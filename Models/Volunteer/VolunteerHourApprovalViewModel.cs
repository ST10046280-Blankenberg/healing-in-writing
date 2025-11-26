namespace HealingInWriting.Models.Volunteer
{
    public class VolunteerHourApprovalViewModel
    {
        public Guid Id { get; set; }
        public string VolunteerName { get; set; } = string.Empty;
        public string? VolunteerAvatarUrl { get; set; }
        public DateOnly Date { get; set; }
        public string Activity { get; set; } = string.Empty;
        public double Hours { get; set; }
        public string? Notes { get; set; }
        public string? AttachmentUrl { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; }
    }
}
