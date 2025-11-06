namespace HealingInWriting.Models.Filters
{
    public class BooksFilterViewModel : BaseFilterViewModel
    {
        public List<string> CategoryOptions { get; set; } = new();
        public List<string> AuthorOptions { get; set; } = new();

        // Selected values
        public string? SelectedCategory { get; set; }
        public string? SelectedAuthor { get; set; }
    }
}
