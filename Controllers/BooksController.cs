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

        public async Task<IActionResult> Index()
        {
            var books = (await _bookService.GetFeaturedAsync())
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
            var books = await _bookService.GetFeaturedFilteredAsync(searchTerm, selectedAuthor, selectedCategory, null);

            var filteredBooks = _bookService.ToBookSummaryViewModels(books);

            return PartialView("_BookCardsPartial", filteredBooks);
        }
    }
}
