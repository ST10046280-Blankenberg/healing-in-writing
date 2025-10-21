using HealingInWriting.Domain.Books;

namespace HealingInWriting.Interfaces.Services;

public interface IBookService
{
    /// <summary>
    /// Retrieves books to display on the catalogue landing page.
    /// </summary>
    Task<IReadOnlyCollection<Book>> GetFeaturedAsync();
}
