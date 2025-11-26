using HealingInWriting.Domain.Events;
using HealingInWriting.Interfaces.Services;
using HealingInWriting.Models.Events;
using HealingInWriting.Models.Filters;
using HealingInWriting.Models.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

namespace HealingInWriting.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class EventsController : Controller
    {
        private readonly IEventService _eventService;
        private readonly IRegistrationService _registrationService;
        private readonly IBlobStorageService _blobStorageService;

        public EventsController(
            IEventService eventService,
            IRegistrationService registrationService,
            IBlobStorageService blobStorageService)
        {
            _eventService = eventService;
            _registrationService = registrationService;
            _blobStorageService = blobStorageService;
        }

        // GET: Admin/Events/Manage
        public async Task<IActionResult> Manage(
            string? searchTerm,
            string? status,
            string? dateRange,
            string? sortOrder,
            int page = 1)
        {
            const int pageSize = 10;
            var currentPage = Math.Max(page, 1);
            var normalizedSortOrder = string.IsNullOrWhiteSpace(sortOrder) ? "date-asc" : sortOrder;

            // Parse status filter
            EventStatus? statusFilter = null;
            if (!string.IsNullOrWhiteSpace(status) &&
                Enum.TryParse<EventStatus>(status, true, out var parsed))
            {
                statusFilter = parsed;
            }

            // Get filtered events from service
            var (events, totalCount) = await _eventService.GetFilteredEventsForAdminAsync(
                searchTerm,
                statusFilter,
                dateRange,
                normalizedSortOrder,
                currentPage,
                pageSize);

            // Map to view models
            var eventViewModels = events.Select(@event =>
            {
                var locationParts = new[]
                {
                    @event.Address?.StreetAddress,
                    @event.Address?.City,
                    @event.Address?.Province
                }
                .Where(part => !string.IsNullOrWhiteSpace(part));

                var isRsvpOpen = @event.EventStatus == EventStatus.Published
                                 && @event.StartDateTime > DateTime.UtcNow;

                return new AdminEventSummaryViewModel
                {
                    Id = @event.EventId,
                    Title = @event.Title,
                    EventType = @event.EventType,
                    Status = @event.EventStatus,
                    StartDateTime = @event.StartDateTime,
                    EndDateTime = @event.EndDateTime,
                    LocationSummary = string.Join(", ", locationParts),
                    IsRsvpOpen = isRsvpOpen
                };
            }).ToList();

            var totalPages = Math.Max((int)Math.Ceiling(totalCount / (double)pageSize), 1);

            var viewModel = new AdminManageEventsViewModel
            {
                Events = eventViewModels,
                Filters = new AdminEventsFilterViewModel
                {
                    SearchText = searchTerm,
                    Status = status,
                    DateRange = dateRange,
                    SortOrder = normalizedSortOrder,
                    Page = currentPage
                },
                StatusOptions = BuildStatusOptions(status),
                DateOptions = BuildDateOptions(dateRange),
                SortOptions = BuildSortOptions(normalizedSortOrder),
                CurrentPage = currentPage,
                TotalPages = totalPages,
                PageSize = pageSize,
                TotalEvents = totalCount
            };

            return View(viewModel);
        }

        // [HttpGet]
        // // GET: Admin/Events/Details
        // public IActionResult Details()
        // {
        //     return View();
        // }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            var model = new CreateEventViewModel
            {
                Id = id ?? 0,
                EventDate = DateTime.Today,
                StartTime = new TimeSpan(9, 0, 0),
                EndTime = new TimeSpan(10, 0, 0)
            };

            if (id.HasValue)
            {
                var existingEvent = await _eventService.GetEventByIdAsync(id.Value);
                if (existingEvent != null)
                {
                    model.Title = existingEvent.Title;
                    model.Description = existingEvent.Description;
                    model.EventType = existingEvent.EventType;
                    model.EventStatus = existingEvent.EventStatus;
                    model.EventDate = existingEvent.StartDateTime.Date;
                    model.StartTime = existingEvent.StartDateTime.TimeOfDay;
                    model.EndTime = existingEvent.EndDateTime.TimeOfDay;
                    model.Capacity = existingEvent.Capacity;

                    if (existingEvent.Address != null)
                    {
                        model.StreetAddress = existingEvent.Address.StreetAddress;
                        model.Suburb = existingEvent.Address.Suburb;
                        model.City = existingEvent.Address.City;
                        model.Province = existingEvent.Address.Province;
                        model.PostalCode = existingEvent.Address.PostalCode;
                        model.Latitude = existingEvent.Address.Latitude;
                        model.Longitude = existingEvent.Address.Longitude;
                    }

                    // Load existing tags as comma-separated string
                    if (existingEvent.EventTags != null && existingEvent.EventTags.Any())
                    {
                        model.Tags = string.Join(",", existingEvent.EventTags.Select(t => t.Name));
                    }

                    // Load existing cover image
                    model.CoverImageUrl = existingEvent.CoverImageUrl;
                }
            }

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(CreateEventViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    ModelState.AddModelError("", "User not authenticated");
                    return View(model);
                }

                // Handle cover image upload
                if (model.CoverImage != null && model.CoverImage.Length > 0)
                {
                    // If updating and there's an existing image, delete it first
                    if (model.Id > 0)
                    {
                        var existingEvent = await _eventService.GetEventByIdAsync(model.Id);
                        if (existingEvent != null && !string.IsNullOrEmpty(existingEvent.CoverImageUrl))
                        {
                            await _blobStorageService.DeleteImageAsync(existingEvent.CoverImageUrl, isPublic: true);
                        }
                    }

                    // Upload new image
                    model.CoverImageUrl = await _blobStorageService.UploadImageAsync(
                        model.CoverImage,
                        "events",
                        isPublic: true);
                }
                // If editing without uploading a new image, preserve the existing URL from hidden field
                // (model.CoverImageUrl is already populated from the hidden field in the form)

                if (model.Id > 0)
                {
                    // Update existing event
                    await _eventService.UpdateEventAsync(model, userId);
                    TempData["SuccessMessage"] = "Event updated successfully!";
                }
                else
                {
                    // Create new event
                    var eventId = await _eventService.CreateEventAsync(model, userId);
                    TempData["SuccessMessage"] = "Event created successfully!";
                }

                return RedirectToAction(nameof(Manage));
            }
            catch (ArgumentException ex)
            {
                // Validation errors from BlobStorageService
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
            catch (Exception ex)
            {
                var action = model.Id > 0 ? "updating" : "creating";
                ModelState.AddModelError("", $"Error {action} event: {ex.Message}");
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _eventService.DeleteEventAsync(id);

                if (success)
                {
                    TempData["SuccessMessage"] = "Event deleted successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Event not found.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting event: {ex.Message}";
            }

            return RedirectToAction(nameof(Manage));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, EventStatus status)
        {
            try
            {
                var success = await _eventService.UpdateEventStatusAsync(id, status);

                if (success)
                {
                    TempData["SuccessMessage"] = $"Event status updated to {status}.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Event not found.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating event status: {ex.Message}";
            }

            return RedirectToAction(nameof(Manage));
        }

        [HttpGet]
        public async Task<IActionResult> Registrations(int id)
        {
            var @event = await _eventService.GetEventByIdAsync(id);
            if (@event == null)
            {
                return NotFound();
            }

            var registrations = await _registrationService.GetEventRegistrationsAsync(id);

            var viewModel = new EventRegistrationsViewModel
            {
                EventId = @event.EventId,
                EventTitle = @event.Title,
                EventStartDateTime = @event.StartDateTime,
                Capacity = @event.Capacity,
                RegisteredCount = registrations.Count(),
                Registrations = registrations.Select(r => new RegistrationItemViewModel
                {
                    RegistrationId = r.RegistrationId,
                    AttendeeName = r.UserId.HasValue
                        ? $"{r.User?.User?.FirstName} {r.User?.User?.LastName}".Trim()
                        : r.GuestName ?? "Unknown",
                    AttendeeEmail = r.UserId.HasValue
                        ? r.User?.User?.Email ?? "N/A"
                        : r.GuestEmail ?? "N/A",
                    AttendeePhone = r.UserId.HasValue
                        ? r.User?.User?.PhoneNumber
                        : r.GuestPhone,
                    RegistrationDate = r.RegistrationDate,
                    IsGuest = !r.UserId.HasValue,
                    IsAdminOverride = r.IsAdminOverride
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRegistration(int eventId, string guestName, string guestEmail, string? guestPhone)
        {
            var result = await _registrationService.RegisterGuestAsync(
                eventId,
                guestName,
                guestEmail,
                ipAddress: null,
                guestPhone,
                isAdminOverride: true);

            if (result.Success)
            {
                TempData["SuccessMessage"] = "Registration added successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction(nameof(Registrations), new { id = eventId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveRegistration(int registrationId, int eventId)
        {
            var result = await _registrationService.CancelRegistrationAsync(
                registrationId,
                requestingUserId: null,
                isAdminOverride: true);

            if (result.Success)
            {
                TempData["SuccessMessage"] = "Registration removed successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction(nameof(Registrations), new { id = eventId });
        }


        //TODO Remove in production
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SeedEvents()
        {
            try
            {
                // Get the admin user ID (current user)
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "admin-seed";
                var random = new Random();

                for (int i = 1; i <= 10; i++)
                {
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

                    var eventId = await _eventService.CreateEventAsync(model, userId);

                    // Seed a random number of guest registrations (between 1 and event capacity)
                    int guestCount = random.Next(1, model.Capacity + 1);
                    for (int r = 1; r <= guestCount; r++)
                    {
                        var guestName = $"Guest {r} for Event {i}";
                        var guestEmail = $"guest{r}_event{i}@example.com";
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

                TempData["SuccessMessage"] = "10 events with a random number of registrations have been seeded.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Seeding failed: {ex.Message}";
            }

            return RedirectToAction(nameof(Manage));
        }

        private static List<AdminDropdownOption> BuildStatusOptions(string? selectedStatus)
        {
            return Enum.GetValues<EventStatus>()
                .Select(s => new AdminDropdownOption
                {
                    Value = s.ToString(),
                    Text = s.ToString(),
                    Selected = s.ToString().Equals(selectedStatus, StringComparison.OrdinalIgnoreCase)
                })
                .ToList();
        }

        private static List<AdminDropdownOption> BuildDateOptions(string? selectedRange)
        {
            var options = new[]
            {
                ("upcoming", "Upcoming Events"),
                ("past", "Past Events"),
                ("this-week", "This Week"),
                ("this-month", "This Month"),
                ("next-30", "Next 30 Days")
            };

            return options.Select(o => new AdminDropdownOption
            {
                Value = o.Item1,
                Text = o.Item2,
                Selected = o.Item1.Equals(selectedRange, StringComparison.OrdinalIgnoreCase)
            }).ToList();
        }

        private static List<AdminDropdownOption> BuildSortOptions(string? selectedSort)
        {
            var options = new[]
            {
                ("date-asc", "Date (Upcoming First)"),
                ("date-desc", "Date (Latest First)"),
                ("oldest", "Oldest Created"),
                ("newest", "Newest Created")
            };

            return options.Select(o => new AdminDropdownOption
            {
                Value = o.Item1,
                Text = o.Item2,
                Selected = o.Item1.Equals(selectedSort, StringComparison.OrdinalIgnoreCase)
            }).ToList();
        }
    }
}
