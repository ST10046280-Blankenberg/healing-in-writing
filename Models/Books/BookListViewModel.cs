namespace HealingInWriting.Models.Books;

// TODO: Shape book catalogue data for presentation in views.
public class BookListViewModel
{
    public List<BookSummaryViewModel> Books { get; set; } = new();

    //Filtering Metadata
    public List<string> AvailableCategories { get; set; } = new();
    public List<string> AvailableAuthors { get; set; } = new();
    public string SelectedCategory { get; set; } = string.Empty;
    public string SelectedAuthor { get; set; } = string.Empty;

}

public class BookSummaryViewModel
{
    public int BookId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Authors { get; set; } = string.Empty;
    public string PublishedDate { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Categories { get; set; } = new();
    public string ThumbnailUrl { get; set; } = string.Empty;
}