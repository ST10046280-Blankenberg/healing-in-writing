namespace HealingInWriting.Models.Dashboard;

public class DashboardViewModel
{
    public string UserName { get; set; } = string.Empty;
    public int MyStoriesCount { get; set; }
    public int MyEventsCount { get; set; }
    public int? MyHoursCount { get; set; } // Nullable for non-volunteers
    public List<DashboardNotificationViewModel> Notifications { get; set; } = new();
}

public class DashboardNotificationViewModel
{
    public string Message { get; set; } = string.Empty;
    public string TimeAgo { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
}

public enum NotificationType
{
    Success,
    Info,
    Warning
}
