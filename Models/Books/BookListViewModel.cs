namespace HealingInWriting.Models.Books;

public class BookListViewModel
{
    public List<BookSummaryViewModel> Books { get; set; } = new();

    //Filtering Metadata
    public List<string> AvailableCategories { get; set; } = new();
    public List<string> AvailableAuthors { get; set; } = new();
    public string SelectedCategory { get; set; } = string.Empty;
    public string SelectedAuthor { get; set; } = string.Empty;

}


