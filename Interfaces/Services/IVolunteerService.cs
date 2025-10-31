using HealingInWriting.Models.Volunteer;

namespace HealingInWriting.Interfaces.Services;

// TODO: Define volunteer workflows exposed to MVC controllers.
public interface IVolunteerService
{
    // TODO: Provide dashboards, assignments, and hour logging operations.
    Task<(bool Success, string? Error)> LogHoursAsync(string userId, LogHoursViewModel model, string? attachmentUrl);
}
