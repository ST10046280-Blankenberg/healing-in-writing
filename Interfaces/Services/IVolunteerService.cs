using HealingInWriting.Models.Volunteer;

namespace HealingInWriting.Interfaces.Services;

// TODO: Define volunteer workflows exposed to MVC controllers.
public interface IVolunteerService
{
    Task<List<VolunteerHourApprovalViewModel>> GetAllVolunteerHourApprovalsAsync();
    Task<List<VolunteerHourApprovalViewModel>> GetFilteredVolunteerHourApprovalsAsync(DateOnly? startDate, DateOnly? endDate, string? status, string? orderBy, string? search);

    // TODO: Provide dashboards, assignments, and hour logging operations.
    Task<(bool Success, string? Error)> LogHoursAsync(string userId, LogHoursViewModel model, string? attachmentUrl);
    Task<(bool Success, string? Error)> UpdateHourStatusAsync(Guid hourId, string status, string? reviewedBy);
}
