using HealingInWriting.Domain.Books;
using HealingInWriting.Models.Books;

namespace HealingInWriting.Interfaces.Services;

/// <summary>
/// Service interface for managing books, including retrieval, creation, updating, deletion, and mapping to view models.
/// </summary>
public interface IBookService
{
    /// <summary>
    /// Imports a book from an external source (e.g., Google Books API) using its ISBN.
    /// </summary>
    /// <param name="isbn">The ISBN to import.</param>
    /// <returns>An <see cref="ImportResult"/> containing the imported book, rate limit status, and message.</returns>
    Task<ImportResult> ImportBookByIsbnAsync(string isbn);

    /// <summary>
    /// Seeds books into the repository from a predefined list of ISBNs.
    /// </summary>
    /// <returns>
    /// A message string if the operation was interrupted (e.g., due to rate limiting); otherwise, <c>null</c>.
    /// </returns>
    Task<string?> SeedBooksAsync();

    /// <summary>
    /// Adds a new book to the repository from form data.
    /// </summary>
    /// <param name="form">The form collection containing book data.</param>
    /// <returns>A tuple indicating success and an optional error message.</returns>
    Task<(bool Success, string? ErrorMessage)> AddBookFromFormAsync(IFormCollection form);

    /// <summary>
    /// Deletes a book by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the book to delete.</param>
    /// <returns><c>true</c> if the book was deleted; otherwise, <c>false</c>.</returns>
    Task<bool> DeleteBookAsync(int id);

    /// <summary>
    /// Updates an existing book in the repository.
    /// </summary>
    /// <param name="book">The <see cref="Book"/> entity to update.</param>
    Task UpdateBookAsync(Book book);

    /// <summary>
    /// Retrieves a book by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the book.</param>
    /// <returns>The <see cref="Book"/> if found; otherwise, <c>null</c>.</returns>
    Task<Book?> GetBookByIdAsync(int id);

    /// <summary>
    /// Maps a <see cref="Book"/> entity to a <see cref="BookDetailViewModel"/> for detailed presentation.
    /// </summary>
    /// <param name="book">The <see cref="Book"/> to map.</param>
    /// <returns>A <see cref="BookDetailViewModel"/> representing the book.</returns>
    BookDetailViewModel ToBookDetailViewModel(Book book);

    /// <summary>
    /// Maps a <see cref="BookDetailViewModel"/> back to a <see cref="Book"/> entity.
    /// </summary>
    /// <param name="model">The <see cref="BookDetailViewModel"/> to map.</param>
    /// <returns>A <see cref="Book"/> entity.</returns>
    Book ToBookFromDetailViewModel(BookDetailViewModel model);

    /// <summary>
    /// Maps a collection of <see cref="Book"/> entities to a list of <see cref="BookSummaryViewModel"/> for summary presentation.
    /// </summary>
    /// <param name="books">The collection of <see cref="Book"/> entities.</param>
    /// <returns>A list of <see cref="BookSummaryViewModel"/>.</returns>
    List<BookSummaryViewModel> ToBookSummaryViewModels(IEnumerable<Book> books);

    /// <summary>
    /// Builds a <see cref="BookListViewModel"/> from a collection of <see cref="Book"/> entities, including filtering metadata.
    /// </summary>
    /// <param name="books">The collection of <see cref="Book"/> entities.</param>
    /// <returns>A <see cref="BookListViewModel"/> for use in views.</returns>
    BookListViewModel ToBookListViewModel(IEnumerable<Book> books);

    /// <summary>
    /// Retrieves a paged, filterable list of books for admin (all books, regardless of visibility).
    /// </summary>
    Task<IReadOnlyCollection<Book>> GetPagedForAdminAsync(
        string? searchTerm, string? selectedAuthor, string? selectedCategory, string? selectedTag, int skip, int take);

    /// <summary>
    /// Retrieves a paged, filterable list of books for users (only visible books).
    /// </summary>
    Task<IReadOnlyCollection<Book>> GetPagedForUserAsync(
        string? searchTerm, string? selectedAuthor, string? selectedCategory, string? selectedTag, int skip, int take);

    Task<List<string>> GetAllAuthorsAsync(bool onlyVisible);
    Task<List<string>> GetAllCategoriesAsync(bool onlyVisible);
}

/// <summary>
/// Represents the result of an import operation, including the imported book, rate limit status, and a message.
/// </summary>
public class ImportResult
{
    public Book? Book { get; set; }
    public bool RateLimited { get; set; }
    public string? Message { get; set; }
}
