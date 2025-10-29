using HealingInWriting.Domain.Books;

namespace HealingInWriting.Interfaces.Repository
{
    /// <summary>
    /// Declares book persistence operations decoupled from domain logic.
    /// </summary>
    public interface IBookRepository
    {
        /// <summary>
        /// Retrieves all books in the catalogue.
        /// </summary>
        Task<IEnumerable<Book>> GetAllAsync(); // For admin: all books, regardless of visibility

        /// <summary>
        /// Retrieves a single book by its unique identifier.
        /// </summary>
        Task<Book?> GetByIdAsync(int bookId);

        /// <summary>
        /// Adds a new book to the catalogue.
        /// </summary>
        Task AddAsync(Book book);

        /// <summary>
        /// Updates an existing book in the catalogue.
        /// </summary>
        Task UpdateAsync(Book book);

        /// <summary>
        /// Deletes a book from the catalogue by its unique identifier.
        /// </summary>
        Task DeleteAsync(int bookId);

        /// <summary>
        /// Retrieves visible books with applied filters: search term, author, category, and tag.
        /// </summary>
        Task<IEnumerable<Book>> GetVisibleFilteredAsync(
            string? searchTerm,
            string? selectedAuthor,
            string? selectedCategory,
            string? selectedTag); // For users: only visible books, with filters
    }
}
