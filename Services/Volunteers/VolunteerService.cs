using HealingInWriting.Domain.Volunteers;
using HealingInWriting.Interfaces.Repository;
using HealingInWriting.Interfaces.Services;
using HealingInWriting.Mapping;
using HealingInWriting.Models.Volunteer;
using HealingInWriting.Repositories.Volunteers;

namespace HealingInWriting.Services.Volunteers;

// TODO: Implement volunteer management logic while enforcing domain invariants.
public class VolunteerService : IVolunteerService
{
    private readonly IVolunteerRepository _repository;

    public VolunteerService(IVolunteerRepository repository)
    {
        _repository = repository;
    }

    public async Task<(bool Success, string? Error)> LogHoursAsync(string userId, LogHoursViewModel model, string? attachmentUrl)
    {
        var volunteer = _repository.GetVolunteerByUserId(userId);
        if (volunteer == null)
            return (false, "Volunteer profile not found.");

        var hour = ViewModelMappers.ToVolunteerHour(model, volunteer.VolunteerId, attachmentUrl);

        _repository.AddVolunteerHour(hour);
        await _repository.SaveChangesAsync();
        return (true, null);
    }

    public async Task<List<VolunteerHourApprovalViewModel>> GetAllVolunteerHourApprovalsAsync()
    {
        var hours = await _repository.GetAllVolunteerHoursWithVolunteerAsync();
        return hours.Select(ViewModelMappers.ToVolunteerHourApprovalViewModel).ToList();
    }

    public async Task<(bool Success, string? Error)> UpdateHourStatusAsync(Guid hourId, string status, string? reviewedBy)
    {
        var hour = await _repository.GetVolunteerHourByIdAsync(hourId);
        if (hour == null)
            return (false, "Volunteer hour not found.");

        if (!Enum.TryParse<VolunteerHourStatus>(status, true, out var newStatus))
            return (false, "Invalid status.");

        hour.Status = newStatus;
        hour.ReviewedAt = DateTime.UtcNow;
        hour.ReviewedBy = reviewedBy;
        await _repository.SaveChangesAsync();
        return (true, null);
    }
}
