using HealingInWriting.Domain.Volunteers;

namespace HealingInWriting.Interfaces.Repository;

// TODO: Outline volunteer persistence operations separate from domain logic.
public interface IVolunteerRepository
{
    void AddVolunteerHour(VolunteerHour hour);
    Task<List<VolunteerHour>> GetAllVolunteerHoursWithVolunteerAsync();
    Volunteer? GetVolunteerByUserId(string userId);
    Task SaveChangesAsync();
}
