using HealingInWriting.Data;
using HealingInWriting.Domain.Events;
using HealingInWriting.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace HealingInWriting.Repositories.Events;

public class EventRepository : IEventRepository
{
    private readonly ApplicationDbContext _context;

    public EventRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Event?> GetByIdAsync(int eventId)
    {
        return await _context.Events
            .Include(e => e.Address)
            .Include(e => e.EventTags)
            .Include(e => e.User)
            .FirstOrDefaultAsync(e => e.EventId == eventId);
    }

    public async Task<IEnumerable<Event>> GetAllAsync()
    {
        return await _context.Events
            .Include(e => e.Address)
            .Include(e => e.EventTags)
            .Include(e => e.User)
            .OrderByDescending(e => e.StartDateTime)
            .ToListAsync();
    }

    public async Task AddAsync(Event @event)
    {
        // Handle tags - attach existing ones to avoid duplication
        if (@event.EventTags?.Any() == true)
        {
            var tagIds = @event.EventTags.Select(t => t.TagId).ToList();
            var existingTags = await _context.Tags
                .Where(t => tagIds.Contains(t.TagId))
                .ToListAsync();
            @event.EventTags = existingTags;
        }

        await _context.Events.AddAsync(@event);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Event @event)
    {
        var existing = await _context.Events
            .Include(e => e.EventTags)
            .Include(e => e.Address)
            .FirstOrDefaultAsync(e => e.EventId == @event.EventId);

        if (existing == null) return;

        // Update scalar properties
        existing.Title = @event.Title;
        existing.Description = @event.Description;
        existing.EventType = @event.EventType;
        existing.StartDateTime = @event.StartDateTime;
        existing.EndDateTime = @event.EndDateTime;
        existing.EventStatus = @event.EventStatus;
        existing.Capacity = @event.Capacity;

        // Update address reference if changed
        if (existing.AddressId != @event.AddressId)
        {
            existing.AddressId = @event.AddressId;
        }

        // Update address properties if same address
        if (existing.Address != null && @event.Address != null)
        {
            existing.Address.StreetAddress = @event.Address.StreetAddress;
            existing.Address.Suburb = @event.Address.Suburb;
            existing.Address.City = @event.Address.City;
            existing.Address.Province = @event.Address.Province;
            existing.Address.PostalCode = @event.Address.PostalCode;
            existing.Address.Country = @event.Address.Country;
            existing.Address.Latitude = @event.Address.Latitude;
            existing.Address.Longitude = @event.Address.Longitude;
        }

        // Update tags
        existing.EventTags.Clear();
        if (@event.EventTags?.Any() == true)
        {
            var tagIds = @event.EventTags.Select(t => t.TagId).ToList();
            var tags = await _context.Tags.Where(t => tagIds.Contains(t.TagId)).ToListAsync();
            existing.EventTags = tags;
        }

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int eventId)
    {
        var @event = await _context.Events.FindAsync(eventId);
        if (@event != null)
        {
            _context.Events.Remove(@event);
            await _context.SaveChangesAsync();
        }
    }
}
