using HealingInWriting.Domain.Volunteers;
using HealingInWriting.Domain.Users;
using Microsoft.EntityFrameworkCore;
using HealingInWriting.Interfaces.Repository;
using HealingInWriting.Data;

namespace HealingInWriting.Repositories.Volunteers;

// TODO: Implement volunteer persistence using EF Core mappings.
public class VolunteerRepository : IVolunteerRepository
{
    private readonly ApplicationDbContext _dbContext;

    public VolunteerRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Volunteer? GetVolunteerByUserId(string userId)
    {
        return _dbContext.Volunteers
            .Include(v => v.User)
            .FirstOrDefault(v => v.UserId == userId);
    }

    public void AddVolunteerHour(VolunteerHour hour)
    {
        _dbContext.VolunteerHours.Add(hour);
    }

    public Task SaveChangesAsync() => _dbContext.SaveChangesAsync();
}
