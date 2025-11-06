using HealingInWriting.Data;
using HealingInWriting.Domain.Events;
using HealingInWriting.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace HealingInWriting.Repositories.Events;

public class RegistrationRepository : IRegistrationRepository
{
    private readonly ApplicationDbContext _context;

    public RegistrationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Registration>> GetByEventIdAsync(int eventId)
    {
        return await _context.Registrations
            .Include(r => r.User)
            .ThenInclude(u => u!.User)
            .Include(r => r.Event)
            .Where(r => r.EventId == eventId)
            .OrderBy(r => r.RegistrationDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Registration>> GetByUserIdAsync(int userId)
    {
        return await _context.Registrations
            .Include(r => r.Event)
            .ThenInclude(e => e.Address)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.Event.StartDateTime)
            .ToListAsync();
    }

    public async Task<Registration?> GetByIdAsync(int registrationId)
    {
        return await _context.Registrations
            .Include(r => r.Event)
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.RegistrationId == registrationId);
    }

    public async Task<Registration?> GetByEventAndUserAsync(int eventId, int userId)
    {
        return await _context.Registrations
            .FirstOrDefaultAsync(r => r.EventId == eventId && r.UserId == userId);
    }

    public async Task<Registration?> GetByEventAndGuestEmailAsync(int eventId, string guestEmail)
    {
        return await _context.Registrations
            .FirstOrDefaultAsync(r => r.EventId == eventId && r.GuestEmail == guestEmail);
    }

    public async Task<int> GetRegistrationCountAsync(int eventId)
    {
        return await _context.Registrations
            .CountAsync(r => r.EventId == eventId);
    }

    public async Task AddAsync(Registration registration)
    {
        await _context.Registrations.AddAsync(registration);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int registrationId)
    {
        var registration = await _context.Registrations.FindAsync(registrationId);
        if (registration != null)
        {
            _context.Registrations.Remove(registration);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> GetRegistrationCountByIpAsync(int eventId, string ipAddress, DateTime since)
    {
        return await _context.Registrations
            .CountAsync(r => r.EventId == eventId
                && r.IpAddress == ipAddress
                && r.RegistrationDate >= since);
    }

    public async Task<IEnumerable<Registration>> GetFilteredUserRegistrationsAsync(
        int userId,
        string? searchText,
        EventType? selectedEventType,
        DateTime? startDate,
        DateTime? endDate)
    {
        var query = _context.Registrations
            .Include(r => r.Event)
            .ThenInclude(e => e.Address)
            .Where(r => r.UserId == userId);

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            query = query.Where(r =>
                r.Event.Title.Contains(searchText) ||
                r.Event.Description.Contains(searchText));
        }

        if (selectedEventType.HasValue)
        {
            query = query.Where(r => r.Event.EventType == selectedEventType.Value);
        }

        if (startDate.HasValue)
        {
            query = query.Where(r => r.Event.StartDateTime >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(r => r.Event.EndDateTime <= endDate.Value);
        }

        return await query
            .OrderByDescending(r => r.Event.StartDateTime)
            .ToListAsync();
    }
}
