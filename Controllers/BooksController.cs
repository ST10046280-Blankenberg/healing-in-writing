using HealingInWriting.Interfaces.Services;
using HealingInWriting.Models.Books;
using Microsoft.AspNetCore.Mvc;

namespace HealingInWriting.Controllers
{
    public class BooksController : Controller
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        public async Task<IActionResult> Index(string searchTerm, string selectedAuthor, string selectedCategory, string selectedTag)
        {
            var books = await _bookService.GetFeaturedFilteredAsync(searchTerm, selectedAuthor, selectedCategory, selectedTag);

            var viewModel = new BookListViewModel
            {
                Books = books.Select(book => new BookSummaryViewModel
                {
                    BookId = book.BookId,
                    Title = book.Title,
                    Authors = book.Authors?.Any() == true ? string.Join(", ", book.Authors) : string.Empty,
                    PublishedDate = book.PublishedDate ?? string.Empty,
                    Publisher = book.Publisher ?? string.Empty,
                    PageCount = book.PageCount,
                    Description = book.Description ?? string.Empty,
                    Categories = book.Categories?.ToList() ?? new List<string>(),
                    ThumbnailUrl = book.ImageLinks?.Thumbnail ?? book.ImageLinks?.SmallThumbnail ?? string.Empty
                }).ToList(),
                AvailableAuthors = books.SelectMany(book => book.Authors ?? Enumerable.Empty<string>())
                    .Distinct()
                    .OrderBy(author => author)
                    .ToList(),
                AvailableCategories = books.SelectMany(book => book.Categories ?? Enumerable.Empty<string>())
                    .Distinct()
                    .OrderBy(category => category)
                    .ToList(),
                SelectedAuthor = selectedAuthor ?? string.Empty,
                SelectedCategory = selectedCategory ?? string.Empty
            };

            return View(viewModel);
        }
    }
}
