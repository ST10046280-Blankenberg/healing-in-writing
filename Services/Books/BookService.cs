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

    public BookService(
        IBookRepository bookRepository,
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory,
        ILogger<BookService> logger)
    {
        _bookRepository = bookRepository;
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    // Seed books into the database if not already present
    public async Task SeedBooksAsync()
    {
        var existingBooks = (await _bookRepository.GetAllAsync()).ToList();
        // Pull every stored ISBN so we avoid hitting Google Books for titles that already exist locally.
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
            books = books?.Where(b => b.Authors?.Contains(selectedAuthor) == true).ToList() ?? new List<Book>();

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

        var httpClient = _httpClientFactory.CreateClient();
        var url = $"https://www.googleapis.com/books/v1/volumes?q=isbn:{isbn}&key={apiKey}";
        var response = await httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();
        var googleResult = JsonDocument.Parse(json);

        var item = googleResult.RootElement.GetProperty("items").EnumerateArray().FirstOrDefault();
        if (item.ValueKind == JsonValueKind.Undefined)
            return null;

        var volumeInfo = item.GetProperty("volumeInfo");

        // Use mapping helper for Google Books JSON to Book
        var book = ViewModelMappers.ToBookFromGoogleJson(volumeInfo);

        return book;
    }

    public BookDetailViewModel ToBookDetailViewModel(Book book)
        => book.ToBookDetailViewModel();

    public Book ToBookFromDetailViewModel(BookDetailViewModel model)
        => model.ToBookFromDetailViewModel();

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

    public async Task<bool> DeleteBookAsync(int id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book == null)
            return false;

        await _bookRepository.DeleteAsync(id);
        return true;
    }

    public async Task UpdateBookAsync(Book book)
    {
        await _bookRepository.UpdateAsync(book);
    }

    public Task<Book?> GetBookByIdAsync(int id)
    {
        return _bookRepository.GetByIdAsync(id);
    }

    // New: Map a list of Book to a list of BookSummaryViewModel
    public List<BookSummaryViewModel> ToBookSummaryViewModels(IEnumerable<Book> books)
        => books.Select(ViewModelMappers.ToBookSummaryViewModel).ToList();

    // New: Build a BookListViewModel from a list of books
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
}
