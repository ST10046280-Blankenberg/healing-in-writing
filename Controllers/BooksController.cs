using HealingInWriting.Domain.Books;
using HealingInWriting.Interfaces.Services;
using HealingInWriting.Models.Books;
using HealingInWriting.Services.Books;
using Microsoft.AspNetCore.Authorization;
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

        // For regular users: only visible books
        public async Task<IActionResult> Index()
        {
            // Only fetch visible books for users
            var books = (await _bookService.GetFeaturedFilteredAsync(
                    searchTerm: null,
                    selectedAuthor: null,
                    selectedCategory: null,
                    selectedTag: null))
                .OrderByDescending(b => b.PublishedDate)
                .Take(10)
                .ToList();

            var viewModel = _bookService.ToBookListViewModel(books);

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int BookId)
        {
            var book = await _bookService.GetBookByIdAsync(BookId);

            if (book == null)
                return NotFound();

            var viewModel = _bookService.ToBookDetailViewModel(book);

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Filter(string searchTerm, string selectedAuthor, string selectedCategory)
        {
            // Only fetch visible books for users
            var books = await _bookService.GetFeaturedFilteredAsync(
                searchTerm, selectedAuthor, selectedCategory, null);

            var filteredBooks = _bookService.ToBookSummaryViewModels(books);

            return PartialView("_BookCardsPartial", filteredBooks);
        }
    }
}
