namespace HealingInWriting.Models.Admin;

/// <summary>
/// Aggregated data displayed on the admin dashboard.
/// </summary>
public class AdminDashboardViewModel
{
    public int PendingStoryCount { get; set; }

    public int EventsThisMonthCount { get; set; }

    public int PendingVolunteerHoursCount { get; set; }

    public IReadOnlyCollection<AdminDashboardActivityItem> RecentActivities { get; set; }
        = Array.Empty<AdminDashboardActivityItem>();
}

public class AdminDashboardActivityItem
{
    public AdminDashboardActivityItem(string description, string timeAgo, AdminDashboardActivityType type)
    {
        Description = description;
        TimeAgo = timeAgo;
        Type = type;
    }

    public string Description { get; }

    public string TimeAgo { get; }

    public AdminDashboardActivityType Type { get; }
}

public enum AdminDashboardActivityType
{
    Story,
    Event,
    Volunteer
}
