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

        // For regular users: only visible books, paged
        public async Task<IActionResult> Index()
        {
            // Fetch first page of visible books for users
            var books = await _bookService.GetPagedForUserAsync(
                    searchTerm: null,
                    selectedAuthor: null,
                    selectedCategory: null,
                    selectedTag: null,
                    skip: 0,
                    take: 10);

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
        public async Task<IActionResult> Filter(string searchTerm, string selectedAuthor, string selectedCategory, int skip = 0, int take = 10)
        {
            // Use paged method for filtering
            var books = await _bookService.GetPagedForUserAsync(
                searchTerm,
                selectedAuthor,
                selectedCategory,
                null,
                skip,
                take);

            var filteredBooks = _bookService.ToBookSummaryViewModels(books);

            return PartialView("_BookCardsPartial", filteredBooks);
        }

        [HttpGet]
        public async Task<IActionResult> ListPaged(
            string? searchTerm, string? selectedAuthor, string? selectedCategory, int skip = 0, int take = 10)
        {
            var books = await _bookService.GetPagedForUserAsync(
                searchTerm,
                selectedAuthor,
                selectedCategory,
                null,
                skip,
                take);

            var filteredBooks = _bookService.ToBookSummaryViewModels(books);

            return PartialView("_BookCardsPartial", filteredBooks);
        }
    }
}
