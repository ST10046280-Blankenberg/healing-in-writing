using HealingInWriting.Domain.Volunteers;

namespace HealingInWriting.Interfaces.Repository;

// TODO: Outline volunteer persistence operations separate from domain logic.
public interface IVolunteerRepository
{
    void AddVolunteerHour(VolunteerHour hour);
    void DeleteVolunteerHour(VolunteerHour hour);
    Task<List<VolunteerHour>> GetAllVolunteerHoursWithVolunteerAsync();
    Volunteer? GetVolunteerByUserId(string userId);
    Task<VolunteerHour?> GetVolunteerHourByIdAsync(Guid id);
    Task SaveChangesAsync();
}
