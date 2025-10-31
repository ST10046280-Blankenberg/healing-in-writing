namespace HealingInWriting.Models.Volunteer
{
    public class VolunteerHourFilterViewModel
    {
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string? Status { get; set; }
        public string? OrderBy { get; set; }
        public string? Search { get; set; }

        public List<VolunteerHourApprovalViewModel> Results { get; set; } = new();
    }
}
