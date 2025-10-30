using HealingInWriting.Domain.Events;
using HealingInWriting.Interfaces.Repository;
using HealingInWriting.Interfaces.Services;

namespace HealingInWriting.Services.Events;

public class RegistrationService : IRegistrationService
{
    private readonly IRegistrationRepository _registrationRepository;
    private readonly IEventRepository _eventRepository;

    public RegistrationService(IRegistrationRepository registrationRepository, IEventRepository eventRepository)
    {
        _registrationRepository = registrationRepository;
        _eventRepository = eventRepository;
    }

    public async Task<RegistrationResult> RegisterUserAsync(int eventId, int userId, bool isAdminOverride = false)
    {
        // Get event
        var @event = await _eventRepository.GetByIdAsync(eventId);
        if (@event == null)
        {
            return new RegistrationResult { Success = false, Message = "Event not found." };
        }

        // Check if user already registered
        var existingRegistration = await _registrationRepository.GetByEventAndUserAsync(eventId, userId);
        if (existingRegistration != null)
        {
            return new RegistrationResult { Success = false, Message = "You are already registered for this event." };
        }

        // Validate registration rules (unless admin override)
        if (!isAdminOverride)
        {
            var validationResult = await ValidateRegistrationRulesAsync(@event);
            if (!validationResult.Success)
            {
                return validationResult;
            }
        }

        // Create registration
        var registration = new Registration
        {
            EventId = eventId,
            UserId = userId,
            RegistrationDate = DateTime.UtcNow,
            IsAdminOverride = isAdminOverride,
            Event = @event
        };

        await _registrationRepository.AddAsync(registration);

        return new RegistrationResult
        {
            Success = true,
            Message = "Successfully registered for event.",
            RegistrationId = registration.RegistrationId
        };
    }

    public async Task<RegistrationResult> RegisterGuestAsync(int eventId, string guestName, string guestEmail,
        string? ipAddress = null, string? guestPhone = null, bool isAdminOverride = false)
    {
        // Validate guest email
        if (string.IsNullOrWhiteSpace(guestEmail))
        {
            return new RegistrationResult { Success = false, Message = "Guest email is required." };
        }

        // Get event
        var @event = await _eventRepository.GetByIdAsync(eventId);
        if (@event == null)
        {
            return new RegistrationResult { Success = false, Message = "Event not found." };
        }

        // Check if guest email already registered
        var existingRegistration =
            await _registrationRepository.GetByEventAndGuestEmailAsync(eventId, guestEmail.ToLower());
        if (existingRegistration != null)
        {
            return new RegistrationResult
            {
                Success = false, Message = "This email is already registered for this event."
            };
        }

        // IP-based rate limiting (unless admin override)
        if (!isAdminOverride && !string.IsNullOrWhiteSpace(ipAddress))
        {
            var oneHourAgo = DateTime.UtcNow.AddHours(-1);
            var recentRegistrationsFromIp = await _registrationRepository.GetRegistrationCountByIpAsync(
                eventId, ipAddress, oneHourAgo);

            if (recentRegistrationsFromIp >= 3)
            {
                return new RegistrationResult
                {
                    Success = false,
                    Message = "Registration limit reached. Maximum 3 registrations per hour from your location."
                };
            }
        }

        // Validate registration rules (unless admin override)
        if (!isAdminOverride)
        {
            var validationResult = await ValidateRegistrationRulesAsync(@event);
            if (!validationResult.Success)
            {
                return validationResult;
            }
        }

        // Create guest registration
        var registration = new Registration
        {
            EventId = eventId,
            UserId = null,
            GuestName = guestName,
            GuestEmail = guestEmail.ToLower(),
            GuestPhone = guestPhone,
            IpAddress = ipAddress,
            RegistrationDate = DateTime.UtcNow,
            IsAdminOverride = isAdminOverride,
            Event = @event
        };

        await _registrationRepository.AddAsync(registration);

        return new RegistrationResult
        {
            Success = true,
            Message = "Successfully registered for event.",
            RegistrationId = registration.RegistrationId
        };
    }

    public async Task<RegistrationResult> CancelRegistrationAsync(int registrationId, int? requestingUserId = null,
        bool isAdminOverride = false)
    {
        var registration = await _registrationRepository.GetByIdAsync(registrationId);
        if (registration == null)
        {
            return new RegistrationResult { Success = false, Message = "Registration not found." };
        }

        // Check ownership (unless admin override)
        if (!isAdminOverride && requestingUserId.HasValue)
        {
            if (registration.UserId != requestingUserId.Value)
            {
                return new RegistrationResult
                {
                    Success = false, Message = "You can only cancel your own registrations."
                };
            }
        }

        // Check 48-hour cancellation policy (unless admin override)
        if (!isAdminOverride)
        {
            var hoursUntilEvent = (registration.Event.StartDateTime - DateTime.UtcNow).TotalHours;
            if (hoursUntilEvent < 48)
            {
                return new RegistrationResult
                {
                    Success = false,
                    Message =
                        "Cancellations must be made at least 48 hours before the event. Please contact an administrator for assistance."
                };
            }
        }

        await _registrationRepository.DeleteAsync(registrationId);

        return new RegistrationResult { Success = true, Message = "Registration cancelled successfully." };
    }

    public async Task<IEnumerable<Registration>> GetEventRegistrationsAsync(int eventId)
    {
        return await _registrationRepository.GetByEventIdAsync(eventId);
    }

    public async Task<IEnumerable<Registration>> GetUserRegistrationsAsync(int userId)
    {
        return await _registrationRepository.GetByUserIdAsync(userId);
    }

    public async Task<bool> IsUserRegisteredAsync(int eventId, int userId)
    {
        var registration = await _registrationRepository.GetByEventAndUserAsync(eventId, userId);
        return registration != null;
    }

    public async Task<bool> IsGuestRegisteredAsync(int eventId, string guestEmail)
    {
        var registration =
            await _registrationRepository.GetByEventAndGuestEmailAsync(eventId, guestEmail.ToLower());
        return registration != null;
    }

    public async Task<RegistrationCapacityInfo> GetCapacityInfoAsync(int eventId)
    {
        var @event = await _eventRepository.GetByIdAsync(eventId);
        if (@event == null)
        {
            return new RegistrationCapacityInfo
            {
                Capacity = 0, RegisteredCount = 0, CanRegister = false, StatusMessage = "Event not found."
            };
        }

        var registeredCount = await _registrationRepository.GetRegistrationCountAsync(eventId);

        var canRegister = @event.EventStatus == EventStatus.Published &&
                          registeredCount < @event.Capacity &&
                          @event.StartDateTime > DateTime.UtcNow;

        var statusMessage = GetStatusMessage(@event, registeredCount);

        return new RegistrationCapacityInfo
        {
            Capacity = @event.Capacity,
            RegisteredCount = registeredCount,
            CanRegister = canRegister,
            StatusMessage = statusMessage
        };
    }

    #region Private Helper Methods

    /// <summary>
    /// Validates business rules for event registration.
    /// </summary>
    private async Task<RegistrationResult> ValidateRegistrationRulesAsync(Event @event)
    {
        // Check event status
        if (@event.EventStatus != EventStatus.Published)
        {
            return new RegistrationResult
            {
                Success = false,
                Message = "Registration is not available for this event. Event status: " + @event.EventStatus
            };
        }

        // Check if event has started
        if (@event.StartDateTime <= DateTime.UtcNow)
        {
            return new RegistrationResult { Success = false, Message = "This event has already started." };
        }

        // Check capacity
        var registeredCount = await _registrationRepository.GetRegistrationCountAsync(@event.EventId);
        if (registeredCount >= @event.Capacity)
        {
            return new RegistrationResult { Success = false, Message = "This event is full." };
        }

        return new RegistrationResult { Success = true };
    }

    /// <summary>
    /// Generates status message for registration capacity display.
    /// </summary>
    private string GetStatusMessage(Event @event, int registeredCount)
    {
        if (@event.EventStatus != EventStatus.Published)
        {
            return "Registration not available";
        }

        if (@event.StartDateTime <= DateTime.UtcNow)
        {
            return "Event has started";
        }

        if (registeredCount >= @event.Capacity)
        {
            return "Event Full";
        }

        var spotsLeft = @event.Capacity - registeredCount;
        return spotsLeft == 1 ? "1 spot remaining" : $"{spotsLeft} spots remaining";
    }

    #endregion
}
