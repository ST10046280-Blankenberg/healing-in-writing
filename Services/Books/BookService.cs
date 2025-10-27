using HealingInWriting.Domain.Books;
using HealingInWriting.Interfaces.Repository;
using HealingInWriting.Interfaces.Services;
using HealingInWriting.Models.Books;

namespace HealingInWriting.Services.Books;
/// <summary>
/// Book service with persistent storage integration.
/// </summary>
public class BookService : IBookService
{
    private readonly IConfiguration _configuration;
    private readonly IBookRepository _bookRepository;

    // 1. List of 10 ISBNs to seed
    private static readonly List<string> SeedIsbns = new()
    {
        "9780765326355", // The Way of Kings
        "9780399590504", // Educated
        "9780735211292", // Atomic Habits
        "9780062316097", // Sapiens
        "9780143127741", // The Martian
        "9781501124020", // The Power of Habit
        "9780062457738", // The Subtle Art of Not Giving a F*ck
        "9780307271037", // The Road
        "9780385472579", // Zen and the Art of Motorcycle Maintenance
        "9780553380163"  // A Short History of Nearly Everything
    };

    public BookService(IBookRepository bookRepository, IConfiguration configuration)
    {
        _bookRepository = bookRepository;
        _configuration = configuration;
    }

    // Seed books into the database if not already present
    public async Task SeedBooksAsync()
    {
        var existingBooks = (await _bookRepository.GetAllAsync()).ToList();
        var existingIsbns = existingBooks
            .SelectMany(b => b.IndustryIdentifiers ?? new List<IndustryIdentifier>())
            .Select(id => id.Identifier)
            .ToHashSet();

        foreach (var isbn in SeedIsbns)
        {
            if (existingIsbns.Contains(isbn))
            {
                continue;
            }

            var book = await ImportBookByIsbnAsync(isbn);
            if (book != null)
            {
                await _bookRepository.AddAsync(book);
            }
        }
    }

    public async Task<IReadOnlyCollection<Book>> GetFeaturedAsync()
    {
        var books = await _bookRepository.GetAllAsync();
        return books.ToList();
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

        var apiKey = _configuration["ApiKeys:GoogleBooks"];
        if (string.IsNullOrWhiteSpace(apiKey))
            return null;

        using var httpClient = new HttpClient();
        var url = $"https://www.googleapis.com/books/v1/volumes?q=isbn:{isbn}&key={apiKey}";
        var response = await httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();
        var googleResult = System.Text.Json.JsonDocument.Parse(json);

        var item = googleResult.RootElement.GetProperty("items").EnumerateArray().FirstOrDefault();
        if (item.ValueKind == System.Text.Json.JsonValueKind.Undefined)
            return null;

        var volumeInfo = item.GetProperty("volumeInfo");

        var book = new Book
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

    public BookDetailViewModel ToBookDetailViewModel(Book book)
    {
        if (book == null) return null;

        return new BookDetailViewModel
        {
            BookId = book.BookId,
            Title = book.Title,
            Authors = string.Join(", ", book.Authors ?? new List<string>()),
            PublishedDate = book.PublishedDate,
            Description = book.Description,
            Categories = book.Categories ?? new List<string>(),
            ThumbnailUrl = book.ImageLinks?.Thumbnail ?? book.ImageLinks?.SmallThumbnail ?? "/images/placeholder-book.svg",
            PageCount = book.PageCount,
            Language = book.Language,
            Publisher = book.Publisher,
            IndustryIdentifiers = book.IndustryIdentifiers?.Select(i => i.Identifier).ToList() ?? new List<string>()
        };
    }

    public async Task<(bool Success, string? ErrorMessage)> AddBookFromFormAsync(IFormCollection form)
    {
        try
        {
            var isbns = new List<string>();
            if (!string.IsNullOrWhiteSpace(form["IsbnPrimary"])) isbns.Add(form["IsbnPrimary"]);
            if (!string.IsNullOrWhiteSpace(form["IsbnSecondary"])) isbns.Add(form["IsbnSecondary"]);

            var book = new Book
            {
                Title = form["Title"],
                Authors = form["Author"].ToString().Split(',').Select(a => a.Trim()).ToList(),
                Publisher = form["Publisher"],
                PublishedDate = form["PublishDate"],
                Description = form["Description"],
                PageCount = int.TryParse(form["PageCount"], out var pc) ? pc : 0,
                Categories = form["Categories"].ToString().Split(',').Select(c => c.Trim()).Where(c => !string.IsNullOrWhiteSpace(c)).ToList(),
                Language = form["Language"],
                IndustryIdentifiers = isbns.Select(isbn => new IndustryIdentifier
                {
                    Type = isbn.Length == 13 ? "ISBN_13" : "ISBN_10",
                    Identifier = isbn.Trim()
                }).ToList(),
                ImageLinks = new ImageLinks
                {
                    Thumbnail = form["ThumbnailUrl"],
                    SmallThumbnail = form["SmallThumbnailUrl"]
                },
            };

            await _bookRepository.AddAsync(book);
            return (true, null);
        }
        catch (Exception ex)
        {
            // Log exception as needed
            return (false, ex.Message);
        }
    }
}
