namespace HealingInWriting.Models.Volunteer
{
    public class LogHoursPageViewModel
    {
        public LogHoursViewModel LogForm { get; set; } = new();
        public List<VolunteerHourApprovalViewModel> RecentEntries { get; set; } = new();
    }
}
