namespace HealingInWriting.Models.Books
{
    /// <summary>
    /// ViewModel for presenting a list of books in the admin inventory management view.
    /// Only includes fields relevant for inventory display.
    /// </summary>
    public class BookInventoryViewModel
    {
        /// <summary>
        /// List of inventory book rows.
        /// </summary>
        public List<BookInventoryRowViewModel> Books { get; set; } = new();
    }

    /// <summary>
    /// Represents a single book row in the admin inventory table.
    /// </summary>
    public class BookInventoryRowViewModel
    {
        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public List<string> Categories { get; set; } = new();
        public string ThumbnailUrl { get; set; } = string.Empty;

        // Inventory-specific fields
        //TODO: add required fields to book domain model.
        public int Quantity { get; set; } = 0;
        public string Condition { get; set; } = "Good"; // e.g., "New", "Good", "Fair", etc.
        public bool IsVisible { get; set; } = true;
    }
}
