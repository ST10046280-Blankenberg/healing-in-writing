using HealingInWriting.Domain.Books;
using HealingInWriting.Interfaces.Services;

namespace HealingInWriting.Services.Books;

/// <summary>
/// Temporary in-memory implementation used until real persistence is introduced.
/// </summary>
public class BookService : IBookService
{
    private static readonly IReadOnlyCollection<Book> FeaturedBooks = new List<Book>
    {
        new()
        {
            BookId = 1,
            Title = "The Way of Kings",
            Authors = new List<string> { "Brandon Sanderson" },
            Publisher = "Tor Books",
            PublishedDate = "2010",
            Description = "From #1 New York Times bestselling author Brandon Sanderson, The Way of Kings, Book One of the Stormlight Archive begins an incredible new saga of epic proportion. Roshar is a world of stone and storms. Uncanny tempests of incredible power sweep across the rocky terrain...",
            PageCount = 1007,
            Categories = new List<string> { "Fantasy", "Epic Fantasy", "Fiction" },
            Language = "en",
            ImageLinks = new ImageLinks
            {
                SmallThumbnail = "http://books.google.com/books/content?id=X_dJAAAACAAJ&printsec=frontcover&img=1&zoom=5&source=gbs_api",
                Thumbnail = "http://books.google.com/books/content?id=ezeekgAACAAJ&printsec=frontcover&img=1&zoom=1&source=gbs_api"
            },
            IndustryIdentifiers = new List<IndustryIdentifier>
            {
                new() { Type = "ISBN_13", Identifier = "9780765326355" }
            }
        },
        new()
        {
            BookId = 2,
            Title = "Educated: A Memoir",
            Authors = new List<string> { "Tara Westover" },
            Publisher = "Random House",
            PublishedDate = "2018",
            Description = "An unforgettable memoir about a young girl who, kept out of school, leaves her survivalist family and goes on to earn a PhD from Cambridge University. Born to survivalists in the mountains of Idaho, Tara Westover was seventeen the first time she set foot in a classroom...",
            PageCount = 334,
            Categories = new List<string> { "Biography & Autobiography", "Personal Memoirs", "Education" },
            Language = "en",
            ImageLinks = new ImageLinks
            {
                SmallThumbnail = "http://books.google.com/books/content?id=2ObWDgAAQBAJ&printsec=frontcover&img=1&zoom=5&edge=curl&source=gbs_api",
                Thumbnail = "http://books.google.com/books/content?id=2ObWDgAAQBAJ&printsec=frontcover&img=1&zoom=1&edge=curl&source=gbs_api"
            },
            IndustryIdentifiers = new List<IndustryIdentifier>
            {
                new() { Type = "ISBN_13", Identifier = "9780399590504" }
            }
        },
        new()
        {
            BookId = 3,
            Title = "Atomic Habits",
            Authors = new List<string> { "James Clear" },
            Publisher = "Avery",
            PublishedDate = "2018",
            Description = "The #1 New York Times bestseller. Over 4 million copies sold! Tiny Changes, Remarkable Results. No matter your goals, Atomic Habits offers a proven framework for improving--every day. James Clear, one of the world's leading experts on habit formation, reveals practical strategies...",
            PageCount = 320,
            Categories = new List<string> { "Self-Help", "Personal Growth", "Success" },
            Language = "en",
            ImageLinks = new ImageLinks
            {
                SmallThumbnail = "http://books.google.com/books/content?id=XfFvDwAAQBAJ&printsec=frontcover&img=1&zoom=5&edge=curl&source=gbs_api",
                Thumbnail = "http://books.google.com/books/content?id=XfFvDwAAQBAJ&printsec=frontcover&img=1&zoom=1&edge=curl&source=gbs_api"
            },
            IndustryIdentifiers = new List<IndustryIdentifier>
            {
                new() { Type = "ISBN_13", Identifier = "9780735211292" }
            }
        }
    };

    public Task<IReadOnlyCollection<Book>> GetFeaturedAsync()
    {
        return Task.FromResult(FeaturedBooks);
    }

    public async Task<IReadOnlyCollection<Book>> GetFeaturedFilteredAsync(string searchTerm, string selectedAuthor, string selectedCategory, string selectedTag)
    {
        var books = await GetFeaturedAsync();

        if (!string.IsNullOrWhiteSpace(selectedAuthor))
            books = books.Where(b => b.Authors.Contains(selectedAuthor)).ToList();

        if (!string.IsNullOrWhiteSpace(selectedCategory))
            books = books.Where(b => b.Categories.Contains(selectedCategory)).ToList();

        if (!string.IsNullOrWhiteSpace(selectedTag))
            books = books.Where(b => b.Categories.Contains(selectedTag)).ToList();

        if (!string.IsNullOrWhiteSpace(searchTerm))
            books = books.Where(b => b.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                     b.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();

        return books.ToList();
    }

    public async Task<Book?> ImportBookByIsbnAsync(string isbn)
    {
        if (string.IsNullOrWhiteSpace(isbn))
            return null;

        using var httpClient = new HttpClient();
        var url = $"https://www.googleapis.com/books/v1/volumes?q=isbn:{isbn}";
        var response = await httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();
        var googleResult = System.Text.Json.JsonDocument.Parse(json);

        var item = googleResult.RootElement.GetProperty("items").EnumerateArray().FirstOrDefault();
        if (item.ValueKind == System.Text.Json.JsonValueKind.Undefined)
            return null;

        var volumeInfo = item.GetProperty("volumeInfo");

        var book =
            
            new Book
        {
            Title = volumeInfo.GetProperty("title").GetString() ?? "",
            Authors = volumeInfo.TryGetProperty("authors", out var authors) ? authors.EnumerateArray().Select(a => a.GetString() ?? "").ToList() : new List<string>(),
            Publisher = volumeInfo.TryGetProperty("publisher", out var publisher) ? publisher.GetString() ?? "" : "",
            PublishedDate = volumeInfo.TryGetProperty("publishedDate", out var pubDate) ? pubDate.GetString() ?? "" : "",
            Description = volumeInfo.TryGetProperty("description", out var desc) ? desc.GetString() ?? "" : "",
            PageCount = volumeInfo.TryGetProperty("pageCount", out var pageCount) ? pageCount.GetInt32() : 0,
            Categories = volumeInfo.TryGetProperty("categories", out var cats) ? cats.EnumerateArray().Select(c => c.GetString() ?? "").ToList() : new List<string>(),
            Language = volumeInfo.TryGetProperty("language", out var lang) ? lang.GetString() ?? "" : "",
            ImageLinks = volumeInfo.TryGetProperty("imageLinks", out var imgLinks)
                ? new ImageLinks
                {
                    Thumbnail = imgLinks.TryGetProperty("thumbnail", out var thumb) ? thumb.GetString() ?? "" : "",
                    SmallThumbnail = imgLinks.TryGetProperty("smallThumbnail", out var smallThumb) ? smallThumb.GetString() ?? "" : ""
                }
                : new ImageLinks(),
            IndustryIdentifiers = volumeInfo.TryGetProperty("industryIdentifiers", out var ids)
                ? ids.EnumerateArray().Select(id =>
                    new IndustryIdentifier
                    {
                        Type = id.TryGetProperty("type", out var type) ? type.GetString() ?? "" : "",
                        Identifier = id.TryGetProperty("identifier", out var ident) ? ident.GetString() ?? "" : ""
                    }).ToList()
                : new List<IndustryIdentifier>()
        };


        return book;
    }
}
