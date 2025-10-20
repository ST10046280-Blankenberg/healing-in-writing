using HealingInWriting.Domain.Books;
using HealingInWriting.Interfaces.Repository;

namespace HealingInWriting.Repositories.Books;

// TODO: Persist books through the configured ORM while enforcing data constraints.
public class BookRepository : IBookRepository
{
    // TODO: Inject DbContext and implement catalogue data access operations.
    public Task AddAsync(Book book)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(int bookId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Book>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Book?> GetByIdAsync(int bookId)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Book book)
    {
        throw new NotImplementedException();
    }
}
