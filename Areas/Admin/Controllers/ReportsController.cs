using HealingInWriting.Data;
using HealingInWriting.Domain.Stories;
using HealingInWriting.Interfaces.Repository;
using HealingInWriting.Models.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealingInWriting.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ReportsController : Controller
    {
        //TODO: Implement Once Stories is complete
        private readonly IEventRepository _eventRepository;
        private readonly IBookRepository _bookRepository;
        //private readonly IStoryRepository _storyRepository;
        private readonly ApplicationDbContext _db;

        public ReportsController(
            IEventRepository eventRepository,
            IBookRepository bookRepository,
            //IStoryRepository storyRepository,
            ApplicationDbContext db)
        {
            _eventRepository = eventRepository;
            _bookRepository = bookRepository;
           //_storyRepository = storyRepository;
            _db = db;
        }

        // GET: Admin/Reports/Index
        public async Task<IActionResult> Index()
        {
            // Event Attendance: For each event, count registrations
            var events = (await _eventRepository.GetAllAsync()).ToList();
            var eventAttendance = events.Select(e => new EventAttendanceReportItem
            {
                EventTitle = e.Title,
                AttendanceCount = _db.Registrations.Count(r => r.EventId == e.EventId)
            }).OrderByDescending(e => e.AttendanceCount).Take(4).ToList();

            //TODO: Implement Once Stories is complete
            // Story Submissions
            //var stories = (await _storyRepository.GetAllAsync()).ToList();
            //var storyPending = stories.Count(s => s.Status == StoryStatus.Submitted);
            //var storyPublished = stories.Count(s => s.Status == StoryStatus.Published);
            //var storyDraft = stories.Count(s => s.Status == StoryStatus.Draft);

            //var recentStories = stories
            //    .OrderByDescending(s => s.CreatedAt)
            //    .Take(8)
            //    .Select(s => new StoryTableRow
            //    {
            //        StoryId = s.StoryId,
            //        Title = s.Title,
            //        AuthorName = s.Author?.User != null
            //            ? $"{s.Author.User.FirstName} {s.Author.User.LastName}"
            //            : "Unknown",
            //        CreatedAt = s.CreatedAt,
            //        Status = s.Status
            //    }).ToList();

            // Book Inventory by Category
            var books = (await _bookRepository.GetAllAsync()).ToList();
            var bookInventory = books
                .SelectMany(b => b.Categories.DefaultIfEmpty("Uncategorized"), (b, c) => new { Book = b, Category = c })
                .GroupBy(x => x.Category)
                .Select(g => new BookInventoryCategoryRow
                {
                    Category = g.Key,
                    StockCount = g.Count()
                })
                .OrderByDescending(c => c.StockCount)
                .ToList();

            var vm = new ReportsDashboardViewModel
            {
                EventAttendance = eventAttendance,
                //TODO: Implement Once Stories is complete
                //StoryPendingCount = storyPending,
                //StoryPublishedCount = storyPublished,
                //StoryDraftCount = storyDraft,
                //RecentStories = recentStories,
                BookInventoryByCategory = bookInventory
            };

            return View(vm);
        }
    }
}
