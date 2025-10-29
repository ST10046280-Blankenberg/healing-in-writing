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

        public async Task<IEnumerable<Book>> GetFilteredPagedAsync(
            string? searchTerm,
            string? selectedAuthor,
            string? selectedCategory,
            string? selectedTag,
            int skip,
            int take,
            bool onlyVisible)
        {
            var query = _context.Books
                .Include(b => b.IndustryIdentifiers)
                .Include(b => b.ImageLinks)
                .AsQueryable();

            if (onlyVisible)
                query = query.Where(b => b.IsVisible);

            if (!string.IsNullOrWhiteSpace(selectedAuthor))
                query = query.Where(b => b.Authors.Contains(selectedAuthor));

            if (!string.IsNullOrWhiteSpace(selectedCategory))
                query = query.Where(b => b.Categories.Contains(selectedCategory));

            if (!string.IsNullOrWhiteSpace(selectedTag))
                query = query.Where(b => b.Categories.Contains(selectedTag));

            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(b =>
                    b.Title.Contains(searchTerm) ||
                    b.Description.Contains(searchTerm));

            return await query
                .OrderBy(b => b.Title)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<List<string>> GetAllAuthorsAsync(bool onlyVisible)
        {
            var query = _context.Books.AsQueryable();
            if (onlyVisible)
                query = query.Where(b => b.IsVisible);

            // Load books into memory, then select authors
            var books = await query.ToListAsync();
            return books
                .SelectMany(b => b.Authors ?? new List<string>())
                .Distinct()
                .OrderBy(a => a)
                .ToList();
        }

        public async Task<List<string>> GetAllCategoriesAsync(bool onlyVisible)
        {
            var query = _context.Books.AsQueryable();
            if (onlyVisible)
                query = query.Where(b => b.IsVisible);

            // Load books into memory, then select categories
            var books = await query.ToListAsync();
            return books
                .SelectMany(b => b.Categories ?? new List<string>())
                .Distinct()
                .OrderBy(c => c)
                .ToList();
        }

        public async Task<int> GetFilteredCountAsync(
            string? searchTerm,
            string? selectedAuthor,
            string? selectedCategory,
            string? selectedTag,
            bool onlyVisible)
        {
            var query = _context.Books.AsQueryable();

            if (onlyVisible)
                query = query.Where(b => b.IsVisible);

            if (!string.IsNullOrWhiteSpace(selectedAuthor))
                query = query.Where(b => b.Authors.Contains(selectedAuthor));

            if (!string.IsNullOrWhiteSpace(selectedCategory))
                query = query.Where(b => b.Categories.Contains(selectedCategory));

            if (!string.IsNullOrWhiteSpace(selectedTag))
                query = query.Where(b => b.Categories.Contains(selectedTag));

            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(b =>
                    b.Title.Contains(searchTerm) ||
                    b.Description.Contains(searchTerm));

            return await query.CountAsync();
        }
    }
}
