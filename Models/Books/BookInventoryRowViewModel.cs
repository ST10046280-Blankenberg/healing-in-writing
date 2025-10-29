namespace HealingInWriting.Models.Books
{
    /// <summary>
    /// Represents a single book row in the admin inventory table.
    /// </summary>
    public class BookInventoryRowViewModel
    {
        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Authors { get; set; } = string.Empty;
        public List<string> Categories { get; set; } = new();
        public string ThumbnailUrl { get; set; } = string.Empty;
        public int Quantity { get; set; } = 1;
        public string Condition { get; set; } = string.Empty;
        public bool IsVisible { get; set; } = true;
    }
}