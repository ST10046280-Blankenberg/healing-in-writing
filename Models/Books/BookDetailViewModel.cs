namespace HealingInWriting.Models.Books
{
    public class BookDetailViewModel
    {
        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Authors { get; set; } = string.Empty;
        public string PublishedDate { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> Categories { get; set; } = new();
        public string ThumbnailUrl { get; set; } = string.Empty;
        public int PageCount { get; set; }
        public string Language { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public List<string> IndustryIdentifiers { get; set; } = new();
        public string Condition { get; set; } = string.Empty;
        public decimal Price { get; set; } = new();
    }
}
