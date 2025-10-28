namespace HealingInWriting.Models.Books
{
    public class BookSummaryViewModel
    {
        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Authors { get; set; } = string.Empty;
        public string PublishedDate { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public int PageCount { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<string> Categories { get; set; } = new();
        public string ThumbnailUrl { get; set; } = string.Empty;
    }
}
