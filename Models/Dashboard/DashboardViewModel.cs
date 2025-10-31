namespace HealingInWriting.Models.Dashboard;

public class DashboardViewModel
{
    public string UserName { get; set; } = string.Empty;
    public int MyStoriesCount { get; set; }
    public int MyEventsCount { get; set; }
    public int? MyHoursCount { get; set; } // Nullable for non-volunteers
}
