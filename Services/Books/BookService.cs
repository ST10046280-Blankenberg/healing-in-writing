using HealingInWriting.Domain.Books;
using HealingInWriting.Interfaces.Repository;
using HealingInWriting.Interfaces.Services;
using HealingInWriting.Models.Books;
using HealingInWriting.Mapping;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace HealingInWriting.Services.Books;

/// <summary>
/// Book service with persistent storage integration.
/// </summary>
public class BookService : IBookService
{
    private readonly IConfiguration _configuration;
    private readonly IBookRepository _bookRepository;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<BookService> _logger;
    private readonly IBackoffStateRepository _backoffStateRepository;

    //1. List of 100 ISBNs to seed
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
    "9780553380163", // A Short History of Nearly Everything
    "9780439023528", // The Hunger Games
    "9780439358071", // Harry Potter and the Order of the Phoenix
    "9780439554930", // Harry Potter and the Sorcerer's Stone
    "9780316769488", // The Catcher in the Rye
    "9780743273565", // The Great Gatsby
    "9780451524935", // 1984
    "9780393007442", // The Odyssey
    "9780142437230", // Moby-Dick
    "9780156012195", // The Little Prince
    "9780141182803", // Brave New World
    "9780143039433", // The Kite Runner
    "9780307949486", // Ready Player One
    "9780307887443", // The Girl on the Train
    "9780061122415", // To Kill a Mockingbird
    "9780307588371", // Gone Girl
    "9780007155668", // The Alchemist
    "9780141033570", // Thinking, Fast and Slow
    "9780307474278", // The Help
    "9780143110439", // Eat, Pray, Love
    "9780375704024", // Beloved
    "0747549559", // Harry Potter and the Philosopherï¿½s Stone
    "9780765376671", // Words of Radiance
    "9780590353427", // Harry Potter and the Chamber of Secrets
    "9780590353403", // Harry Potter and the Prisoner of Azkaban
    "9780553579901", // A Game of Thrones
    "9780553573428", // A Clash of Kings
    "9780553801507", // A Storm of Swords
    "9780307588364", // Inferno
    "9780307959478", // Origin
    "9780671027360", // Angels & Demons
    "9781400031702", // The Da Vinci Code
    "9781400079987", // The Lost Symbol
    "9780307277671", // The Night Circus
    "9780307387899", // The Shack
    "9780307588388", // Digital Fortress
    "9780446310789", // Mockingjay
    "9780679783268", // Pride and Prejudice
    "9780486282114", // Dracula
    "9780141439600", // Jane Eyre
    "9780141442464", // Wuthering Heights
    "9780140177398", // Of Mice and Men
    "9780143127550", // All the Light We Cannot See
    "9780385533224", // The Goldfinch
    "9780307743657", // The Silent Patient
    "9781250301697", // Where the Crawdads Sing
    "9781501161938", // It Ends with Us
    "9780062870605", // The Tattooist of Auschwitz
    "9780316420259", // The Institute
    "9780385545968", // The Midnight Library
    "9780525559474", // The Testaments
    "9781982137274", // The Invisible Life of Addie LaRue
    "9780593139135", // Project Hail Mary
    "9780593318172", // Klara and the Sun
    "9780345803481", // Fifty Shades of Grey
    "9780307346612", // The Road Back to You
    "9780310344513", // Love Does
    "9780062409851", // The Magnolia Story
    "9780316499019", // The Vanishing Half
    "9780735224315", // Becoming
    "9781982137120", // The Boy, The Mole, The Fox and The Horse
    "9780735222359", // The Four Winds
    "9780306824203", // Educated (alternate)
    "9780062976583", // Greenlights
    "9780593230251", // The Body Keeps the Score
    "9780062856625", // The Outsider
    "9780062963675", // The Institute
    "9781984801258", // Circe
    "9780316556347", // Strange the Dreamer
    "9781501160771", // The Woman in the Window
    "9780140283334", // The Tipping Point
    "9780062315009", // Outliers
    "9781594206115", // Grit
    "9780525536291", // 12 Rules for Life
    "9780349411903", // You Are a Badass
    "9780140449266", // Crime and Punishment
    "9780140447934", // The Brothers Karamazov
    "9780140449198", // War and Peace
    "9780143039433", // The Kite Runner
    "1338878956", // Harry Potter and the Goblet of Fire
    "9780439136365", // Harry Potter and the Half-Blood Prince
    "9780545010221", // Harry Potter and the Deathly Hallows
    "9780385754728", // The Book Thief
    "9780064400550", // Bridge to Terabithia
    "9780064404992", // Charlotte's Web
    "9780140366913", // Matilda
    "014240733X", // The Outsiders
    "9780375826696", // A Wrinkle in Time
    "9780307474278", // The Help
    "9780142407333", // The Outsiders (alternate)
    "9780307277671", // The Night Circus
    "9781451673319", // Fahrenheit 451
    "9780307743657"  // The Silent Patient (duplicate allowed for real datasets, but can be replaced)
};

    public BookService(
        IBookRepository bookRepository,
        IBackoffStateRepository backoffStateRepository,
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory,
        ILogger<BookService> logger)
    {
        _bookRepository = bookRepository;
        _backoffStateRepository = backoffStateRepository;
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    #region Book Seeding & Import

    // Seed books into the database if not already present
    public async Task<string?> SeedBooksAsync()
    {
        var existingBooks = (await _bookRepository.GetAllAsync()).ToList();
        var existingIsbns = existingBooks
            .SelectMany(b => b.IndustryIdentifiers ?? new List<IndustryIdentifier>())
            .Select(id => id.Identifier)
            .ToHashSet();

        foreach (var isbn in SeedIsbns)
        {
            if (existingIsbns.Contains(isbn))
                continue;

            var result = await ImportBookByIsbnAsync(isbn);
            if (result.RateLimited)
            {
                // Stop further imports and return the message
                return result.Message;
            }

            if (result.Book != null)
            {
                await _bookRepository.AddAsync(result.Book);
            }
            else
            {
                _logger.LogWarning("Failed to import book with ISBN {Isbn} during seeding.", isbn);
            }
        }
        return null;
    }

    public async Task<ImportResult> ImportBookByIsbnAsync(string isbn)
    {
        var backoff = await _backoffStateRepository.GetAsync() ?? new BackoffState();

        if (backoff.LastImportAttemptUtc.HasValue && backoff.CurrentBackoffSeconds > 0)
        {
            var now = DateTimeOffset.UtcNow;
            var nextAllowed = backoff.LastImportAttemptUtc.Value.AddSeconds(backoff.CurrentBackoffSeconds);
            if (now < nextAllowed)
            {
                var wait = (int)(nextAllowed - now).TotalSeconds;
                _logger.LogWarning("Import attempt aborted: rate limit not finished. Time left: {TimeLeft} seconds for ISBN {Isbn}.", wait, isbn);
                return new ImportResult
                {
                    Book = null,
                    RateLimited = true,
                    Message = $"Google Books API rate limit in effect. Please wait {wait} seconds before retrying."
                };
            }
        }
        backoff.LastImportAttemptUtc = DateTimeOffset.UtcNow;

        if (string.IsNullOrWhiteSpace(isbn))
            return new ImportResult { Book = null };

        // Normalize ISBN: remove dashes, spaces, and other non-digit/non-X characters
        string normalizedIsbn = new string(isbn
            .Where(c => char.IsDigit(c) || c == 'X' || c == 'x')
            .ToArray())
            .ToUpperInvariant();

        if (string.IsNullOrWhiteSpace(normalizedIsbn))
            return new ImportResult { Book = null };

        var apiKey = _configuration["ApiKeys:GoogleBooks"];
        if (string.IsNullOrWhiteSpace(apiKey))
            return new ImportResult { Book = null };

        var httpClient = _httpClientFactory.CreateClient();
        var url = $"https://www.googleapis.com/books/v1/volumes?q=isbn:{normalizedIsbn}&key={apiKey}";
        var response = await httpClient.GetAsync(url);

        if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
        {
            // Increase backoff exponentially, up to max
            var initial = 10.0;
            var max = 600.0;
            backoff.CurrentBackoffSeconds = backoff.CurrentBackoffSeconds == 0
                ? initial
                : Math.Min(backoff.CurrentBackoffSeconds * 2, max);
            backoff.LastImportAttemptUtc = DateTimeOffset.UtcNow;
            await _backoffStateRepository.SaveAsync(backoff);

            _logger.LogWarning("Google Books API rate limit exceeded (HTTP 429) while importing ISBN {Isbn}. Backoff now {Backoff}.", isbn, backoff.CurrentBackoffSeconds);
            return new ImportResult
            {
                Book = null,
                RateLimited = true,
                Message = $"Google Books API rate limit exceeded. Please wait {backoff.CurrentBackoffSeconds} seconds before retrying."
            };
        }

        // On success, reset backoff
        backoff.CurrentBackoffSeconds = 0;
        backoff.LastImportAttemptUtc = null;
        await _backoffStateRepository.SaveAsync(backoff);

        if (!response.IsSuccessStatusCode)
            return new ImportResult { Book = null, Message = "Failed to import book." };

        var json = await response.Content.ReadAsStringAsync();
        var googleResult = JsonDocument.Parse(json);

        if (!googleResult.RootElement.TryGetProperty("items", out var items) || items.ValueKind != JsonValueKind.Array)
            return new ImportResult { Book = null };

        var item = items.EnumerateArray().FirstOrDefault();
        if (item.ValueKind == JsonValueKind.Undefined)
            return new ImportResult { Book = null };

        var volumeInfo = item.GetProperty("volumeInfo");
        var book = ViewModelMappers.ToBookFromGoogleJson(volumeInfo);

        return new ImportResult { Book = book };
    }

    #endregion

    #region Book Retrieval

    public Task<Book?> GetBookByIdAsync(int id)
        => _bookRepository.GetByIdAsync(id);

    #endregion

    #region Book CRUD

    /// <inheritdoc />
    public async Task<(bool Success, string? ErrorMessage)> AddBookFromFormAsync(IFormCollection form)
    {
        try
        {
            var book = ViewModelMappers.ToBookFromForm(form);

            await _bookRepository.AddAsync(book);
            return (true, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding book from form");
            return (false, ex.Message);
        }
    }

    /// <inheritdoc />
    public async Task<bool> DeleteBookAsync(int id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book == null)
            return false;

        await _bookRepository.DeleteAsync(id);
        return true;
    }

    /// <inheritdoc />
    public async Task UpdateBookAsync(Book book)
    {
        await _bookRepository.UpdateAsync(book);
    }

    #endregion

    #region Mapping Methods

    public BookDetailViewModel ToBookDetailViewModel(Book book)
        => book.ToBookDetailViewModel();

    public Book ToBookFromDetailViewModel(BookDetailViewModel model)
        => model.ToBookFromDetailViewModel();

    public List<BookSummaryViewModel> ToBookSummaryViewModels(IEnumerable<Book> books)
        => books.Select(ViewModelMappers.ToBookSummaryViewModel).ToList();

    public BookListViewModel ToBookListViewModel(IEnumerable<Book> books)
    {
        var summaries = ToBookSummaryViewModels(books);
        return new BookListViewModel
        {
            Books = summaries,
            AvailableAuthors = books.SelectMany(b => b.Authors ?? Enumerable.Empty<string>())
                                   .Distinct()
                                   .OrderBy(a => a)
                                   .ToList(),
            AvailableCategories = books.SelectMany(b => b.Categories ?? Enumerable.Empty<string>())
                                      .Distinct()
                                      .OrderBy(c => c)
                                      .ToList()
        };
    }

    public List<BookInventoryRowViewModel> ToBookInventoryRowViewModel(IEnumerable<Book> books)
        => books.Select(ViewModelMappers.ToBookInventoryRowViewModel).ToList();

    public BookInventoryListViewModel ToBookInventoryViewModel(IEnumerable<Book> books)
    {
        var inventoryRows = ToBookInventoryRowViewModel(books);
        return new BookInventoryListViewModel
        {
            Books = inventoryRows,
            AvailableAuthors = books.SelectMany(b => b.Authors ?? Enumerable.Empty<string>())
                                   .Distinct()
                                   .OrderBy(a => a)
                                   .ToList(),
            AvailableCategories = books.SelectMany(b => b.Categories ?? Enumerable.Empty<string>())
                                      .Distinct()
                                      .OrderBy(c => c)
                                      .ToList()
        };
    }

    #endregion

    #region Query Helpers
    /// <summary>
    /// Retrieves a paged, filterable list of books for admin (all books, regardless of visibility).
    /// </summary>
    public async Task<IReadOnlyCollection<Book>> GetPagedForAdminAsync(
        string? searchTerm,
        string? selectedAuthor,
        string? selectedCategory,
        string? selectedTag,
        int skip,
        int take)
    {
        var books = await _bookRepository.GetFilteredPagedAsync(
            searchTerm, selectedAuthor, selectedCategory, selectedTag, skip, take, onlyVisible: false);
        return books.ToList();
    }

    /// <summary>
    /// Retrieves a paged, filterable list of books for users (only visible books).
    /// </summary>
    public async Task<IReadOnlyCollection<Book>> GetPagedForUserAsync(
        string? searchTerm,
        string? selectedAuthor,
        string? selectedCategory,
        string? selectedTag,
        int skip,
        int take)
    {
        var books = await _bookRepository.GetFilteredPagedAsync(
            searchTerm, selectedAuthor, selectedCategory, selectedTag, skip, take, onlyVisible: true);
        return books.ToList();
    }

    public Task<List<string>> GetAllAuthorsAsync(bool onlyVisible)
        => _bookRepository.GetAllAuthorsAsync(onlyVisible);

    public Task<List<string>> GetAllCategoriesAsync(bool onlyVisible)
        => _bookRepository.GetAllCategoriesAsync(onlyVisible);

    /// <summary>
    /// Gets the count of books for admin with filters (all books, regardless of visibility).
    /// </summary>
    public async Task<int> GetCountForAdminAsync(
        string? searchTerm,
        string? selectedAuthor,
        string? selectedCategory,
        string? selectedTag)
    {
        return await _bookRepository.GetFilteredCountAsync(
            searchTerm, selectedAuthor, selectedCategory, selectedTag, onlyVisible: false);
    }

    /// <summary>
    /// Gets the count of books for admin with filters (all books, regardless of visibility).
    /// </summary>
    public async Task<int> GetCountForUserAsync(
        string? searchTerm,
        string? selectedAuthor,
        string? selectedCategory,
        string? selectedTag)
    {
        return await _bookRepository.GetFilteredCountAsync(
            searchTerm, selectedAuthor, selectedCategory, selectedTag, onlyVisible: true);
    }

    #endregion
}
