using HealingInWriting.Data;
using HealingInWriting.Domain.Events;
using HealingInWriting.Domain.Shared;
using HealingInWriting.Interfaces.Repository;
using HealingInWriting.Interfaces.Services;
using HealingInWriting.Models.Events;
using HealingInWriting.Mapping;
using Microsoft.EntityFrameworkCore;

namespace HealingInWriting.Services.Events;

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;
    private readonly ApplicationDbContext _context;
    private readonly IRegistrationService _registrationService;

    public EventService(
        IEventRepository eventRepository,
        ApplicationDbContext context,
        IRegistrationService registrationService)
    {
        _eventRepository = eventRepository;
        _context = context;
        _registrationService = registrationService;
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
            CoverImageUrl = model.CoverImageUrl,
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
        existingEvent.CoverImageUrl = model.CoverImageUrl;
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

    public async Task<bool> UpdateEventStatusAsync(int eventId, EventStatus newStatus)
    {
        var existingEvent = await _eventRepository.GetByIdAsync(eventId);
        if (existingEvent == null)
        {
            return false;
        }

        // Validate status transitions (optional business rules)
        // For now, allow any transition - can add rules later if needed

        existingEvent.EventStatus = newStatus;
        await _eventRepository.UpdateAsync(existingEvent);
        return true;
    }

    public async Task<int> GetUserUpcomingEventsCountAsync(string userId)
    {
        // Registration.UserId is int?, but Event.UserId is int (creator)
        // We need to find registrations by the ApplicationUser string ID
        // First find the UserProfile
        var userProfile = await _context.UserProfiles
            .FirstOrDefaultAsync(up => up.UserId == userId);

        if (userProfile == null)
        {
            return 0;
        }

        var registrations = await _context.Registrations
            .Include(r => r.Event)
            .Where(r => r.UserId == userProfile.ProfileId && r.Event.StartDateTime >= DateTime.UtcNow)
            .CountAsync();

        return registrations;
    }

    public async Task<IReadOnlyCollection<Registration>> GetUserRegistrationsAsync(string userId)
    {
        // Find user profile first
        var userProfile = await _context.UserProfiles
            .FirstOrDefaultAsync(up => up.UserId == userId);

        if (userProfile == null)
        {
            return new List<Registration>();
        }

        var registrations = await _context.Registrations
            .Include(r => r.Event)
                .ThenInclude(e => e.Address)
            .Include(r => r.Event)
                .ThenInclude(e => e.EventTags)
            .Where(r => r.UserId == userProfile.ProfileId)
            .OrderByDescending(r => r.Event.StartDateTime)
            .ToListAsync();

        return registrations;
    }

    public async Task<EventsIndexViewModel> GetFilteredEventsAsync(
        string? searchText,
        EventType? selectedEventType,
        DateTime? startDate,
        DateTime? endDate)
    {
        var events = await _eventRepository.GetFilteredAsync(
            searchText,
            selectedEventType,
            startDate,
            endDate);

        return new EventsIndexViewModel
        {
            Events = events
                .Select(e => e.ToEventCardViewModel())
                .ToList()
        };
    }

    public async Task<(IEnumerable<Event> Events, int TotalCount)> GetFilteredEventsForAdminAsync(
        string? searchTerm,
        EventStatus? status,
        string? dateRange,
        string sortOrder,
        int page,
        int pageSize)
    {
        var query = _context.Events
            .Include(e => e.Address)
            .Include(e => e.EventTags)
            .AsQueryable();

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(e =>
                e.Title.ToLower().Contains(term) ||
                (e.Description != null && e.Description.ToLower().Contains(term)));
        }

        // Apply status filter
        if (status.HasValue)
        {
            query = query.Where(e => e.EventStatus == status.Value);
        }

        // Apply date range filter
        var dateFilter = ParseEventDateRange(dateRange);
        if (dateFilter.HasValue)
        {
            var (startDateTime, endDateTime) = dateFilter.Value;
            query = query.Where(e =>
                e.StartDateTime >= startDateTime &&
                (endDateTime == null || e.StartDateTime <= endDateTime));
        }

        // Apply sorting
        query = sortOrder?.ToLower() switch
        {
            "oldest" => query.OrderBy(e => e.EventId),
            "newest" => query.OrderByDescending(e => e.EventId),
            "date-desc" => query.OrderByDescending(e => e.StartDateTime),
            "date-asc" => query.OrderBy(e => e.StartDateTime),
            _ => query.OrderBy(e => e.StartDateTime)
        };

        // Get total count before pagination
        var totalCount = await query.CountAsync();

        // Apply pagination
        var events = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (events, totalCount);
    }

    /// <summary>
    /// Seeds the database with sample event data and registrations for testing purposes.
    /// This method generates multiple events with randomised registrations.
    /// Should only be used in development or staging environments.
    /// </summary>
    /// <param name="userId">The user ID to associate as the event creator</param>
    /// <param name="eventCount">Number of events to seed (default: 10)</param>
    /// <returns>Null on success, or an error message string on failure</returns>
    public async Task<string?> SeedEventsAsync(string userId, int eventCount = 10)
    {
        try
        {
            var random = new Random();

            for (int i = 1; i <= eventCount; i++)
            {
                // Create view model for event creation
                var model = new CreateEventViewModel
                {
                    Title = $"Seeded Event {i}",
                    Description = $"This is a seeded event number {i}.",
                    EventType = (i % 2 == 0) ? EventType.Workshop : EventType.CommunityEvent,
                    EventStatus = EventStatus.Published,
                    EventDate = DateTime.Today.AddDays(i),
                    StartTime = new TimeSpan(9 + i % 5, 0, 0),
                    EndTime = new TimeSpan(10 + i % 5, 0, 0),
                    Capacity = 20 + i,
                    StreetAddress = $"123{i} Main St",
                    Suburb = $"Suburb {i}",
                    City = "Cape Town",
                    Province = "Western Cape",
                    PostalCode = "8000",
                    Latitude = -33.9 + i * 0.01,
                    Longitude = 18.4 + i * 0.01,
                    Tags = "seed,auto"
                };

                // Create the event using existing service method
                var eventId = await CreateEventAsync(model, userId);

                // Seed a random number of guest registrations (between 1 and event capacity)
                int guestCount = random.Next(1, model.Capacity + 1);
                for (int r = 1; r <= guestCount; r++)
                {
                    var guestName = $"Guest {r} for Event {i}";
                    var guestEmail = $"guest{r}_event{i}@example.com";

                    // Register guest using registration service with admin override
                    await _registrationService.RegisterGuestAsync(
                        eventId,
                        guestName,
                        guestEmail,
                        ipAddress: null,
                        guestPhone: null,
                        isAdminOverride: true
                    );
                }
            }

            return null; // Success - no error message
        }
        catch (Exception ex)
        {
            return $"Seeding failed: {ex.Message}";
        }
    }

    private static (DateTime StartDate, DateTime? EndDate)? ParseEventDateRange(string? dateRange)
    {
        if (string.IsNullOrWhiteSpace(dateRange) ||
            dateRange.Equals("any", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var now = DateTime.UtcNow;

        // Calculate start of week inline (Monday as start of week)
        var diff = (7 + (now.DayOfWeek - DayOfWeek.Monday)) % 7;
        var startOfWeek = now.AddDays(-1 * diff).Date;

        return dateRange.ToLowerInvariant() switch
        {
            "upcoming" => (now, null),
            "past" => (DateTime.MinValue, now.AddDays(-1)),
            "this-week" => (startOfWeek, startOfWeek.AddDays(7)),
            "this-month" => (new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc),
                            new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(1).AddDays(-1)),
            "next-30" => (now, now.AddDays(30)),
            _ => null
        };
    }
}
