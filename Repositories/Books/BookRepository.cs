using HealingInWriting.Domain.Books;
using HealingInWriting.Interfaces.Repository;
using HealingInWriting.Data;
using Microsoft.EntityFrameworkCore;

namespace HealingInWriting.Repositories.Books
{
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationDbContext _context;

        public BookRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int bookId)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Book>> GetAllAsync()
        {
            // Pull identifiers and image links in one go so admin screens do not trigger extra queries.
            return await _context.Books
                .Include(b => b.IndustryIdentifiers)
                .Include(b => b.ImageLinks)
                .ToListAsync();
        }

        public async Task<Book?> GetByIdAsync(int bookId)
        {
            // Same include set as the list query to keep detail pages consistent.
            return await _context.Books
                .Include(b => b.IndustryIdentifiers)
                .Include(b => b.ImageLinks)
                .FirstOrDefaultAsync(b => b.BookId == bookId);
        }

        public async Task UpdateAsync(Book book)
        {
            _context.Books.Update(book);
            await _context.SaveChangesAsync();
        }
    }
}
