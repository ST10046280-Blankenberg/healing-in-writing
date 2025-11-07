namespace HealingInWriting.Models.Filters
{
    /// <summary>
    /// Filter parameters for Admin Events management page
    /// </summary>
    public class AdminEventsFilterViewModel : BaseFilterViewModel
    {
        /// <summary>
        /// Filter by event status (Draft, Published, Cancelled, etc.)
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// Filter by date range (upcoming, past, this-month, etc.)
        /// </summary>
        public string? DateRange { get; set; }

        /// <summary>
        /// Sort order (newest, oldest, date-asc, date-desc)
        /// </summary>
        public string SortOrder { get; set; } = "date-asc";

        /// <summary>
        /// Current page number for pagination
        /// </summary>
        public int Page { get; set; } = 1;
    }
}

