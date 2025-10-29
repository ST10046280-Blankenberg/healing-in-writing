using HealingInWriting.Domain.Books;
using HealingInWriting.Interfaces.Services;
using HealingInWriting.Models.Books;
using HealingInWriting.Services.Books;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.RateLimiting;

namespace HealingInWriting.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BooksController : Controller
    {
        #region Fields

        private readonly IBookService _bookService;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BooksController"/> class.
        /// </summary>
        /// <param name="bookService">The book service dependency.</param>
        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        #endregion

        #region HTTP GET: Views (Page Initialization)

        /// <summary>
        /// Displays the book inventory management view.
        /// </summary>
        /// <returns>The Manage view with a list of featured books.</returns>
        [HttpGet]
        public async Task<IActionResult> Manage()
        {
            var books = await _bookService.GetPagedForAdminAsync(
                searchTerm: null,
                selectedAuthor: null,
                selectedCategory: null,
                selectedTag: null,
                skip: 0,
                take: 20);

            var allAuthors = await _bookService.GetAllAuthorsAsync(onlyVisible: false);
            var allCategories = await _bookService.GetAllCategoriesAsync(onlyVisible: false);

            var viewModel = _bookService.ToBookInventoryViewModel(books);
            viewModel.AvailableAuthors = allAuthors;
            viewModel.AvailableCategories = allCategories;

            return View(viewModel);
        }

        /// <summary>
        /// Returns the view for adding a new book.
        /// </summary>
        /// <returns>The AddBook view.</returns>
        [HttpGet]
        public IActionResult AddBook()
        {
            ViewBag.BookConditions = Enum.GetValues(typeof(BookCondition))
                .Cast<BookCondition>()
                .Select(bc => new SelectListItem
                {
                    Value = bc.ToString(),
                    Text = bc.ToString()
                }).ToList();

            return View();
        }

        /// <summary>
        /// Returns the view for editing a book.
        /// </summary>
        /// <param name="id">The unique identifier of the book to edit.</param>
        /// <returns>The EditBook view with the book details, or NotFound if not found.</returns>
        [HttpGet]
        public async Task<IActionResult> EditBook(int id)
        {
            ViewBag.BookConditions = Enum.GetValues(typeof(BookCondition))
            .Cast<BookCondition>()
            .Select(bc => new SelectListItem
            {
                Value = bc.ToString(),
                Text = bc.ToString()
            }).ToList();

            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
                return NotFound();

            var viewModel = _bookService.ToBookDetailViewModel(book);
            return View(viewModel);
        }

        #endregion

        #region HTTP POST: Actions (CUD)

        /// <summary>
        /// Handles the POST request to add a new book.
        /// </summary>
        /// <param name="form">The form collection containing book data.</param>
        /// <returns>Redirects to Manage on success or failure, with error in TempData if failed.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EnableRateLimiting("standard")]
        public async Task<IActionResult> AddBook(IFormCollection form)
        {
            var result = await _bookService.AddBookFromFormAsync(form);

            if (!result.Success)
            {
                TempData["Error"] = result.ErrorMessage ?? "Failed to add book.";
                return RedirectToAction("Manage");
            }

            return RedirectToAction("Manage");
        }

        /// <summary>
        /// Handles the POST request to edit a book.
        /// </summary>
        /// <param name="model">The book detail view model with updated data.</param>
        /// <returns>Redirects to Manage on success, or returns the EditBook view if model state is invalid.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBook(BookDetailViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var book = _bookService.ToBookFromDetailViewModel(model);

            await _bookService.UpdateBookAsync(book);
            return RedirectToAction("Manage");
        }

        /// <summary>
        /// Deletes a book by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the book to delete.</param>
        /// <returns>Returns Ok on success, BadRequest on failure.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var result = await _bookService.DeleteBookAsync(id);
            if (!result)
                return BadRequest();
            return Ok();
        }

        /// <summary>
        /// Seeds the database with sample book data.
        /// </summary>
        /// <returns>Redirects to Manage with success or error message.</returns>
        //TODO: Remove in production.
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SeedSampleData()
        {
            var message = await _bookService.SeedBooksAsync();
            if (!string.IsNullOrEmpty(message))
            {
                TempData["Error"] = message;
            }
            else
            {
                TempData["Success"] = "Sample data seeded successfully.";
            }
            return RedirectToAction("Manage");
        }

        #endregion

        #region HTTP GET/POST: Utility Actions

        /// <summary>
        /// Imports a book by its ISBN from an external source.
        /// </summary>
        /// <param name="isbn">The ISBN to import.</param>
        /// <returns>
        /// A JSON result with success status and book data if found, or an error message if not.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> ImportBookByIsbn(string isbn)
        {
            if (string.IsNullOrWhiteSpace(isbn))
                return Json(new { success = false, message = "ISBN required." });

            var result = await (_bookService as BookService)?.ImportBookByIsbnAsync(isbn);

            if (result == null)
                return Json(new { success = false, message = "Book not found for this ISBN." });

            if (result.RateLimited)
                return Json(new { success = false, message = result.Message ?? "Rate limit exceeded. Please try again later." });

            if (result.Book == null)
                return Json(new { success = false, message = result.Message ?? "Book not found for this ISBN." });

            var viewModel = (_bookService as BookService)?.ToBookDetailViewModel(result.Book);

            return Json(new { success = true, data = viewModel });
        }

        /// <summary>
        /// Sets the visibility of a book.
        /// </summary>
        /// <param name="request">The request object containing book ID and visibility status.</param>
        /// <returns>Ok on success, BadRequest if request is null, NotFound if book doesn't exist.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetVisibility([FromBody] SetVisibilityRequest request)
        {
            if (request == null)
                return BadRequest();

            var book = await _bookService.GetBookByIdAsync(request.BookId);
            if (book == null)
                return NotFound();

            book.IsVisible = request.IsVisible;
            await _bookService.UpdateBookAsync(book);

            return Ok();
        }

        public class SetVisibilityRequest
        {
            public int BookId { get; set; }
            public bool IsVisible { get; set; }
        }

        /// <summary>
        /// Filters books based on the provided criteria.
        /// </summary>
        /// <param name="searchTerm">The search term to filter books.</param>
        /// <param name="selectedAuthor">The selected author to filter books.</param>
        /// <param name="selectedCategory">The selected category to filter books.</param>
        /// <param name="skip">The number of records to skip for pagination.</param>
        /// <param name="take">The number of records to take for pagination.</param>
        /// <returns>A partial view with the filtered list of books.</returns>
        [HttpGet]
        public async Task<IActionResult> Filter(string searchTerm, string selectedAuthor, string selectedCategory, int skip = 0, int take = 10)
        {
            var books = await _bookService.GetPagedForAdminAsync(
                searchTerm,
                selectedAuthor,
                selectedCategory,
                null,
                skip,
                take);

            var filteredBooks = _bookService.ToBookInventoryRowViewModel(books);

            return PartialView("_BookInventoryRow", filteredBooks);
        }

        /// <summary>
        /// Lists books in a paginated format for admin view.
        /// </summary>
        /// <param name="skip">The number of records to skip for pagination.</param>
        /// <param name="take">The number of records to take for pagination.</param>
        /// <returns>A partial view with the paginated list of books.</returns>
        [HttpGet]
        public async Task<IActionResult> ListPaged(
            string? searchTerm, string? selectedAuthor, string? selectedCategory, int skip = 0, int take = 10)
        {
            var books = await _bookService.GetPagedForAdminAsync(
                searchTerm,
                selectedAuthor,
                selectedCategory,
                null,
                skip,
                take);

            var filteredBooks = _bookService.ToBookInventoryRowViewModel(books);

            return PartialView("_BookInventoryRows", filteredBooks);
        }
        #endregion
    }
}
