using HealingInWriting.Domain.Books;

namespace HealingInWriting.Interfaces.Services;

public interface IBookService
{
    /// <summary>
    /// Retrieves books to display on the catalogue landing page.
    /// </summary>
    Task<IReadOnlyCollection<Book>> GetFeaturedAsync();

    Task<IReadOnlyCollection<Book>> GetFeaturedFilteredAsync(
    string searchTerm,
    string selectedAuthor,
    string selectedCategory,
    string selectedTag
);

    Task<Book?> ImportBookByIsbnAsync(string isbn);

    Task<(bool Success, string? ErrorMessage)> AddBookFromFormAsync(IFormCollection form);
}
