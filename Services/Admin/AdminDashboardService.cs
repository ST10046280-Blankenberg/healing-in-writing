using HealingInWriting.Data;
using HealingInWriting.Domain.Events;
using HealingInWriting.Domain.Stories;
using HealingInWriting.Interfaces.Services;
using HealingInWriting.Models.Admin;
using Microsoft.EntityFrameworkCore;

namespace HealingInWriting.Services.Admin;

public class AdminDashboardService : IAdminDashboardService
{
    private readonly ApplicationDbContext _context;

    public AdminDashboardService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AdminDashboardViewModel> GetDashboardAsync(DateTime utcNow)
    {
        var monthStart = new DateTime(utcNow.Year, utcNow.Month, 1);
        var monthEnd = monthStart.AddMonths(1);

        var pendingStories = await _context.Stories
            .AsNoTracking()
            .CountAsync(story => story.Status == StoryStatus.Submitted);

        var eventsThisMonth = await _context.Events
            .AsNoTracking()
            .CountAsync(evt => evt.StartDateTime >= monthStart
                               && evt.StartDateTime < monthEnd
                               && evt.EventStatus != EventStatus.Cancelled);

        var recentStories = await _context.Stories
            .AsNoTracking()
            .OrderByDescending(story => story.CreatedAt)
            .Select(story => new
            {
                story.Title,
                story.IsAnonymous,
                story.CreatedAt
            })
            .Take(6)
            .ToListAsync();

        var recentRegistrations = await _context.Registrations
            .AsNoTracking()
            .OrderByDescending(registration => registration.RegistrationDate)
            .Select(registration => new
            {
                EventTitle = registration.Event.Title,
                registration.RegistrationDate
            })
            .Take(6)
            .ToListAsync();

        var activities = new List<(DateTime timestamp, AdminDashboardActivityItem item)>();

        foreach (var story in recentStories)
        {
            var description = story.IsAnonymous
                ? "New story submitted by Anonymous"
                : $"New story submitted: \"{story.Title}\"";

            activities.Add((story.CreatedAt,
                new AdminDashboardActivityItem(description,
                    FormatRelativeTime(utcNow, story.CreatedAt),
                    AdminDashboardActivityType.Story)));
        }

        foreach (var registration in recentRegistrations)
        {
            var description = $"Registration received for {registration.EventTitle}";

            activities.Add((registration.RegistrationDate,
                new AdminDashboardActivityItem(description,
                    FormatRelativeTime(utcNow, registration.RegistrationDate),
                    AdminDashboardActivityType.Event)));
        }

        var orderedActivities = activities
            .OrderByDescending(entry => entry.timestamp)
            .Take(6)
            .Select(entry => entry.item)
            .ToList();

        return new AdminDashboardViewModel
        {
            PendingStoryCount = pendingStories,
            EventsThisMonthCount = eventsThisMonth,
            PendingVolunteerHoursCount = 0,
            RecentActivities = orderedActivities
        };
    }

    private static string FormatRelativeTime(DateTime referenceUtc, DateTime occurrenceUtc)
    {
        var delta = referenceUtc - occurrenceUtc;
        var future = delta.TotalSeconds < 0;
        var absoluteDelta = TimeSpan.FromSeconds(Math.Abs(delta.TotalSeconds));

        if (absoluteDelta < TimeSpan.FromMinutes(1))
        {
            return future ? "in under a minute" : "just now";
        }

        (double value, string unit) = absoluteDelta switch
        {
            { TotalDays: >= 1 } span => (Math.Floor(span.TotalDays), "day"),
            { TotalHours: >= 1 } span => (Math.Floor(span.TotalHours), "hour"),
            { TotalMinutes: >= 1 } span => (Math.Floor(span.TotalMinutes), "minute"),
            _ => (Math.Floor(absoluteDelta.TotalSeconds), "second")
        };

        var pluralSuffix = value == 1 ? string.Empty : "s";
        var phrase = $"{value:0} {unit}{pluralSuffix}";

        return future ? $"in {phrase}" : $"{phrase} ago";
    }
}
