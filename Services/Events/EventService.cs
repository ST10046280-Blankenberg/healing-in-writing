using HealingInWriting.Data;
using HealingInWriting.Domain.Events;
using HealingInWriting.Domain.Shared;
using HealingInWriting.Interfaces.Repository;
using HealingInWriting.Interfaces.Services;
using HealingInWriting.Models.Events;
using Microsoft.EntityFrameworkCore;

namespace HealingInWriting.Services.Events;

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;
    private readonly ApplicationDbContext _context;

    public EventService(IEventRepository eventRepository, ApplicationDbContext context)
    {
        _eventRepository = eventRepository;
        _context = context;
    }

    public async Task<int> CreateEventAsync(CreateEventViewModel model, string userId)
    {
        // Get UserProfile
        var userProfile = await _context.UserProfiles
            .FirstOrDefaultAsync(up => up.UserId == userId);

        if (userProfile == null)
            throw new InvalidOperationException("User profile not found");

        // Create Address
        var address = new Address
        {
            StreetAddress = model.StreetAddress,
            Suburb = model.Suburb,
            City = model.City,
            Province = model.Province,
            PostalCode = model.PostalCode,
            Country = "South Africa",
            Latitude = model.Latitude,
            Longitude = model.Longitude
        };

        // Create Event
        var newEvent = new Event
        {
            Title = model.Title,
            Description = model.Description,
            EventType = model.EventType,
            StartDateTime = model.EventDate.Add(model.StartTime),
            EndDateTime = model.EventDate.Add(model.EndTime),
            Capacity = model.Capacity,
            EventStatus = EventStatus.Draft,
            UserId = userProfile.ProfileId,
            User = userProfile,
            Address = address,
            EventTags = await _context.Tags
                .Where(t => model.SelectedTagIds.Contains(t.TagId))
                .ToListAsync()
        };

        await _eventRepository.AddAsync(newEvent);
        return newEvent.EventId;
    }
    public double CalculateDistanceKm(double lat1, double lon1, double lat2, double lon2)     
    {
        throw new NotImplementedException();
    }
    
    public async Task<Event?> GetEventByIdAsync(int eventId)
    {
        return await _eventRepository.GetByIdAsync(eventId);
    }
}