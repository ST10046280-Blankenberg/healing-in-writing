namespace HealingInWriting.Models.Filters
{
    public class ReportsFilterViewModel : BaseFilterViewModel
    {
        // Event Attendance Filters
        public string? EventTitle { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? MinAttendance { get; set; }
        public int? MaxAttendance { get; set; }

        // Book Inventory Filters
        public string? BookCategory { get; set; }
        public string? StockStatus { get; set; } // "critical", "low", "good"
        public int? MinStock { get; set; }
        public int? MaxStock { get; set; }

        // General Report Filters
        public string? ReportType { get; set; } // "events", "inventory", "all"
        public string? SortBy { get; set; } = "attendance-desc"; // "attendance-desc", "attendance-asc", "stock-desc", "stock-asc"
    }
}