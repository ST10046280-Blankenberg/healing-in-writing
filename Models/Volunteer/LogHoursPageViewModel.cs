namespace HealingInWriting.Models.Volunteer
{
    public class LogHoursPageViewModel
    {
        public LogHoursViewModel LogForm { get; set; } = new();
        public List<VolunteerHourApprovalViewModel> RecentEntries { get; set; } = new();

        public double TotalHours { get; set; }
        public double ValidatedHours { get; set; }
        public double PendingHours { get; set; }
        public double NeedsInfoHours { get; set; }
    }
}
