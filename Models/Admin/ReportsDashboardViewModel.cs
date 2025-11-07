using HealingInWriting.Domain.Stories;
using HealingInWriting.Models.Filters;

namespace HealingInWriting.Models.Admin
{
    public class ReportsDashboardViewModel
    {
        // Filter State
        public ReportsFilterViewModel Filters { get; set; } = new();

        
        // Event Attendance
        public List<EventAttendanceReportItem> EventAttendance { get; set; } = new();

        // Story Submission Stats
        public int StoryPendingCount { get; set; }
        public int StoryPublishedCount { get; set; }
        public int StoryDraftCount { get; set; }
        public List<StoryTableRow> RecentStories { get; set; } = new();

        // Book Inventory Snapshot
        public List<BookInventoryCategoryRow> BookInventoryByCategory { get; set; } = new();
    }

    public class EventAttendanceReportItem
    {
        public string EventTitle { get; set; }
        public int AttendanceCount { get; set; }
        public DateTime EventDate { get; set; } 

    }

    public class StoryTableRow
    {
        public int StoryId { get; set; }
        public string Title { get; set; }
        public string AuthorName { get; set; }
        public DateTime CreatedAt { get; set; }
        public StoryStatus Status { get; set; }
    }

    public class BookInventoryCategoryRow
    {
        public string Category { get; set; }
        public int StockCount { get; set; }
    }
}