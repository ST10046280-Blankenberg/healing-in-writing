using HealingInWriting.Domain.Books;
using HealingInWriting.Models.Books;
using Microsoft.AspNetCore.Http;

namespace HealingInWriting.Interfaces.Services;

/// <summary>
/// Service interface for managing books, including retrieval, creation, updating, deletion, and mapping to view models.
/// </summary>
public interface IBookService
{
    #region Book Retrieval

    /// <summary>
    /// Retrieves all books to display on the catalogue landing page.
    /// </summary>
    /// <returns>A read-only collection of <see cref="Book"/> entities.</returns>
    Task<IReadOnlyCollection<Book>> GetFeaturedAsync();

    /// <summary>
    /// Retrieves books filtered by search term, author, category, and tag.
    /// </summary>
    /// <param name="searchTerm">A search string to match against book titles and descriptions.</param>
    /// <param name="selectedAuthor">The author to filter by.</param>
    /// <param name="selectedCategory">The category to filter by.</param>
    /// <param name="selectedTag">The tag to filter by.</param>
    /// <returns>A read-only collection of filtered <see cref="Book"/> entities.</returns>
    Task<IReadOnlyCollection<Book>> GetFeaturedFilteredAsync(
        string searchTerm,
        string selectedAuthor,
        string selectedCategory,
        string selectedTag
    );

    /// <summary>
    /// Retrieves a book by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the book.</param>
    /// <returns>The <see cref="Book"/> if found; otherwise, <c>null</c>.</returns>
    Task<Book?> GetBookByIdAsync(int id);

    #endregion

    #region Book Import

    /// <summary>
    /// Imports a book from an external source (e.g., Google Books API) using its ISBN.
    /// </summary>
    /// <param name="isbn">The ISBN to import.</param>
    /// <returns>The imported <see cref="Book"/> if found; otherwise, <c>null</c>.</returns>
    Task<Book?> ImportBookByIsbnAsync(string isbn);

    #endregion

    #region Book CRUD

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

    #endregion

    #region Mapping Methods

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

    #endregion
}
