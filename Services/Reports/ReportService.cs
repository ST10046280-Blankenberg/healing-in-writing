using HealingInWriting.Data;
using HealingInWriting.Interfaces.Services;
using HealingInWriting.Models.Admin;
using HealingInWriting.Models.Filters;
using Microsoft.EntityFrameworkCore;

namespace HealingInWriting.Services.Reports;

public class ReportService : IReportService
{
    private readonly ApplicationDbContext _context;

    public ReportService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ReportsDashboardViewModel> GetDashboardDataAsync(ReportsFilterViewModel filters)
    {
        var model = new ReportsDashboardViewModel
        {
            Filters = filters
        };

        // Get filtered Event Attendance data
        model.EventAttendance = await GetEventAttendanceAsync(filters);

        // Get filtered Book Inventory data
        model.BookInventoryByCategory = await GetBookInventoryAsync(filters);

        // TODO: Uncomment when Stories is complete
        // await PopulateStoryStatsAsync(model, filters);

        return model;
    }

    private async Task<List<EventAttendanceReportItem>> GetEventAttendanceAsync(ReportsFilterViewModel filters)
    {
        var query = _context.Events.AsQueryable();

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(filters.SearchText))
        {
            query = query.Where(e => e.Title.Contains(filters.SearchText));
        }

        // Apply event title filter
        if (!string.IsNullOrWhiteSpace(filters.EventTitle))
        {
            query = query.Where(e => e.Title.Contains(filters.EventTitle));
        }

        // Apply date range filters
        if (filters.StartDate.HasValue)
        {
            query = query.Where(e => e.StartDateTime >= filters.StartDate.Value);
        }

        if (filters.EndDate.HasValue)
        {
            query = query.Where(e => e.StartDateTime <= filters.EndDate.Value);
        }

        // Project to report items with registration count
        var eventAttendance = await query
            .Select(e => new EventAttendanceReportItem
            {
                EventTitle = e.Title,
                AttendanceCount = _context.Registrations.Count(r => r.EventId == e.EventId),
                EventDate = e.StartDateTime
            })
            .ToListAsync();

        // Apply attendance filters (post-query since it's calculated)
        if (filters.MinAttendance.HasValue)
        {
            eventAttendance = eventAttendance
                .Where(e => e.AttendanceCount >= filters.MinAttendance.Value)
                .ToList();
        }

        if (filters.MaxAttendance.HasValue)
        {
            eventAttendance = eventAttendance
                .Where(e => e.AttendanceCount <= filters.MaxAttendance.Value)
                .ToList();
        }

        // Apply sorting
        eventAttendance = ApplyEventSorting(eventAttendance, filters.SortBy);

        // Take top 4 for dashboard display (matching existing behavior)
        return eventAttendance.Take(4).ToList();
    }

    private async Task<List<BookInventoryCategoryRow>> GetBookInventoryAsync(ReportsFilterViewModel filters)
    {
        var query = _context.Books.AsQueryable();

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(filters.SearchText))
        {
            query = query.Where(b => b.Categories.Any(c => c.Contains(filters.SearchText)));
        }

        // Apply book category filter
        if (!string.IsNullOrWhiteSpace(filters.BookCategory))
        {
            query = query.Where(b => b.Categories.Any(c => c.Contains(filters.BookCategory)));
        }

        // Get all books and group by category
        var books = await query.ToListAsync();
        
        // Group by category and calculate stock count (matching existing logic)
        var bookInventory = books
            .SelectMany(b => b.Categories.DefaultIfEmpty("Uncategorized"), (b, c) => new { Book = b, Category = c })
            .GroupBy(x => x.Category ?? "Uncategorized")
            .Select(g => new BookInventoryCategoryRow
            {
                Category = g.Key,
                StockCount = g.Count() // Using Count() to match existing behavior
            })
            .ToList();

        // Apply min/max stock filters
        if (filters.MinStock.HasValue)
        {
            bookInventory = bookInventory
                .Where(b => b.StockCount >= filters.MinStock.Value)
                .ToList();
        }

        if (filters.MaxStock.HasValue)
        {
            bookInventory = bookInventory
                .Where(b => b.StockCount <= filters.MaxStock.Value)
                .ToList();
        }

        // Apply stock status filter
        if (!string.IsNullOrWhiteSpace(filters.StockStatus))
        {
            bookInventory = filters.StockStatus switch
            {
                "critical" => bookInventory.Where(b => b.StockCount < 50).ToList(),
                "low" => bookInventory.Where(b => b.StockCount >= 50 && b.StockCount < 100).ToList(),
                "good" => bookInventory.Where(b => b.StockCount >= 100).ToList(),
                _ => bookInventory
            };
        }

        // Apply sorting
        bookInventory = ApplyBookSorting(bookInventory, filters.SortBy);

        return bookInventory;
    }

    private List<EventAttendanceReportItem> ApplyEventSorting(
        List<EventAttendanceReportItem> events,
        string? sortBy)
    {
        return sortBy switch
        {
            "attendance-asc" => events.OrderBy(e => e.AttendanceCount).ToList(),
            "attendance-desc" => events.OrderByDescending(e => e.AttendanceCount).ToList(),
            "date-asc" => events.OrderBy(e => e.EventDate).ToList(),
            "date-desc" => events.OrderByDescending(e => e.EventDate).ToList(),
            _ => events.OrderByDescending(e => e.AttendanceCount).ToList() // Default matches existing behavior
        };
    }

    private List<BookInventoryCategoryRow> ApplyBookSorting(
        List<BookInventoryCategoryRow> books,
        string? sortBy)
    {
        return sortBy switch
        {
            "stock-asc" => books.OrderBy(b => b.StockCount).ToList(),
            "stock-desc" => books.OrderByDescending(b => b.StockCount).ToList(),
            "category-asc" => books.OrderBy(b => b.Category).ToList(),
            "category-desc" => books.OrderByDescending(b => b.Category).ToList(),
            _ => books.OrderByDescending(b => b.StockCount).ToList() // Default matches existing behavior
        };
    }

    // TODO: Implement when Stories is complete
    /*
    private async Task PopulateStoryStatsAsync(ReportsDashboardViewModel model, ReportsFilterViewModel filters)
    {
        var storiesQuery = _context.Stories.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filters.SearchText))
        {
            storiesQuery = storiesQuery.Where(s => s.Title.Contains(filters.SearchText));
        }

        var stories = await storiesQuery
            .Include(s => s.Author)
            .ThenInclude(a => a.User)
            .ToListAsync();

        model.StoryPendingCount = stories.Count(s => s.Status == StoryStatus.Submitted);
        model.StoryPublishedCount = stories.Count(s => s.Status == StoryStatus.Published);
        model.StoryDraftCount = stories.Count(s => s.Status == StoryStatus.Draft);

        model.RecentStories = stories
            .OrderByDescending(s => s.CreatedAt)
            .Take(8)
            .Select(s => new StoryTableRow
            {
                StoryId = s.StoryId,
                Title = s.Title,
                AuthorName = s.Author?.User != null
                    ? $"{s.Author.User.FirstName} {s.Author.User.LastName}"
                    : "Unknown",
                CreatedAt = s.CreatedAt,
                Status = s.Status
            })
            .ToList();
    }
    */
}
