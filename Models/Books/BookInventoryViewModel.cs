namespace HealingInWriting.Models.Books
{
    /// <summary>
    /// ViewModel for presenting a list of books in the admin inventory management view.
    /// </summary>
    public class BookInventoryViewModel
    {
        /// <summary>
        /// List of books to display in the inventory table.
        /// </summary>
        public List<BookSummaryViewModel> Books { get; set; } = new();

        // Optionally, you can add inventory-specific metadata here in the future,
        // such as filtering, sorting, or status fields.
    }
}
