using System.Collections.Generic;
using System.Linq;
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
        // Get or create UserProfile
        var userProfile = await _context.UserProfiles
            .FirstOrDefaultAsync(up => up.UserId == userId);

        if (userProfile == null)
        {
            // Create a basic UserProfile for the admin user if it doesn't exist
            var applicationUser = await _context.Users.FindAsync(userId);
            if (applicationUser == null)
                throw new InvalidOperationException("User not found");

            userProfile = new Domain.Users.UserProfile
            {
                UserId = userId,
                Bio = string.Empty,
                City = string.Empty,
                User = applicationUser
            };
            
            _context.UserProfiles.Add(userProfile);
            await _context.SaveChangesAsync();
        }

        // Create Address
        var address = new Address
        {
            StreetAddress = model.StreetAddress,
            Suburb = model.Suburb,
            City = model.City,
            Province = model.Province,
            PostalCode = model.PostalCode,
            Country = "South Africa",
            Latitude = model.Latitude ?? 0,
            Longitude = model.Longitude ?? 0
        };

        // Process tags from comma-separated string
        var eventTags = new List<Tag>();
        if (!string.IsNullOrWhiteSpace(model.Tags))
        {
            var tagNames = model.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim())
                .Where(t => !string.IsNullOrEmpty(t))
                .Distinct()
                .ToList();

            foreach (var tagName in tagNames)
            {
                // Find existing tag or create new one
                var existingTag = await _context.Tags
                    .FirstOrDefaultAsync(t => t.Name.ToLower() == tagName.ToLower());

                if (existingTag != null)
                {
                    eventTags.Add(existingTag);
                }
                else
                {
                    var newTag = new Tag { Name = tagName };
                    _context.Tags.Add(newTag);
                    await _context.SaveChangesAsync(); // Save to get TagId
                    eventTags.Add(newTag);
                }
            }
        }

        // Create Event
        var newEvent = new Event
        {
            Title = model.Title,
            Description = model.Description,
            EventType = model.EventType,
            StartDateTime = model.EventDate.Add(model.StartTime),
            EndDateTime = model.EventDate.Add(model.EndTime),
            Capacity = model.Capacity,
            EventStatus = model.EventStatus,
            UserId = userProfile.ProfileId,
            User = userProfile,
            Address = address,
            EventTags = eventTags
        };

        await _eventRepository.AddAsync(newEvent);
        return newEvent.EventId;
    }

    public async Task UpdateEventAsync(CreateEventViewModel model, string userId)
    {
        // Get the existing event
        var existingEvent = await _eventRepository.GetByIdAsync(model.Id);
        if (existingEvent == null)
        {
            throw new InvalidOperationException($"Event with ID {model.Id} not found");
        }

        // Update event properties
        existingEvent.Title = model.Title;
        existingEvent.Description = model.Description;
        existingEvent.EventType = model.EventType;
        existingEvent.EventStatus = model.EventStatus;
        existingEvent.StartDateTime = model.EventDate.Add(model.StartTime);
        existingEvent.EndDateTime = model.EventDate.Add(model.EndTime);
        existingEvent.Capacity = model.Capacity;

        // Update address properties
        if (existingEvent.Address != null)
        {
            existingEvent.Address.StreetAddress = model.StreetAddress;
            existingEvent.Address.Suburb = model.Suburb;
            existingEvent.Address.City = model.City;
            existingEvent.Address.Province = model.Province;
            existingEvent.Address.PostalCode = model.PostalCode;
            existingEvent.Address.Latitude = model.Latitude ?? 0;
            existingEvent.Address.Longitude = model.Longitude ?? 0;
        }

        // Update tags
        existingEvent.EventTags.Clear();
        if (!string.IsNullOrWhiteSpace(model.Tags))
        {
            var tagNames = model.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim())
                .Where(t => !string.IsNullOrEmpty(t))
                .Distinct()
                .ToList();

            foreach (var tagName in tagNames)
            {
                // Find existing tag or create new one
                var existingTag = await _context.Tags
                    .FirstOrDefaultAsync(t => t.Name.ToLower() == tagName.ToLower());

                if (existingTag != null)
                {
                    existingEvent.EventTags.Add(existingTag);
                }
                else
                {
                    var newTag = new Tag { Name = tagName };
                    _context.Tags.Add(newTag);
                    await _context.SaveChangesAsync(); // Save to get TagId
                    existingEvent.EventTags.Add(newTag);
                }
            }
        }

        await _eventRepository.UpdateAsync(existingEvent);
    }

    public double CalculateDistanceKm(double lat1, double lon1, double lat2, double lon2)
    {
        throw new NotImplementedException();
    }
    
    public async Task<Event?> GetEventByIdAsync(int eventId)
    {
        return await _eventRepository.GetByIdAsync(eventId);
    }

    public async Task<IReadOnlyCollection<Event>> GetAllEventsAsync()
    {
        var events = await _eventRepository.GetAllAsync();
        return events
            .OrderByDescending(e => e.StartDateTime)
            .ToList();
    }

    public async Task<bool> DeleteEventAsync(int eventId)
    {
        var existingEvent = await _eventRepository.GetByIdAsync(eventId);
        if (existingEvent == null)
        {
            return false;
        }

        await _eventRepository.DeleteAsync(eventId);
        return true;
    }
}
