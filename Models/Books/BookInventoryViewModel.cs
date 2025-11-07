namespace HealingInWriting.Models.Books
{
    /// <summary>
    /// ViewModel for presenting a list of books in the admin inventory management view.
    /// Only includes fields relevant for inventory display.
    /// </summary>
    public class BookInventoryListViewModel
    {
        public List<BookInventoryRowViewModel> Books { get; set; } = new();

        //Filtering Metadata
        public List<string> AvailableCategories { get; set; } = new();
        public List<string> AvailableAuthors { get; set; } = new();
        public string SelectedCategory { get; set; } = string.Empty;
        public string SelectedAuthor { get; set; } = string.Empty;
        public string? SearchTerm { get; set; }
    }
}
