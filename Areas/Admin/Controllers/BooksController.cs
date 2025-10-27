using HealingInWriting.Interfaces.Services;
using HealingInWriting.Models.Books;
using HealingInWriting.Services.Books;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealingInWriting.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BooksController : Controller
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        public async Task<IActionResult> Manage()
        {
            var books = (await _bookService.GetFeaturedAsync()).ToList();
            var model = new BookInventoryViewModel
            {
                Books = books.Select(book => new BookInventoryRowViewModel
                {
                    BookId = book.BookId,
                    Title = book.Title,
                    Categories = book.Categories?.ToList() ?? new List<string>(),
                    ThumbnailUrl = book.ImageLinks?.Thumbnail ?? string.Empty
                }).ToList()
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EnableRateLimiting("standard")]
        public async Task<IActionResult> Add(IFormCollection form)
        {
            // Move all parsing and creation logic to the service
            var result = await _bookService.AddBookFromFormAsync(form);

            if (!result.Success)
            {
                TempData["Error"] = result.ErrorMessage ?? "Failed to add book.";
                return RedirectToAction("Manage");
            }

            return RedirectToAction("Manage");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _bookService.DeleteBookAsync(id);
            if (!result)
                return BadRequest();
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> ImportBookByIsbn(string isbn)
        {
            if (string.IsNullOrWhiteSpace(isbn))
                return Json(new { success = false, message = "ISBN required." });

            var book = await _bookService.ImportBookByIsbnAsync(isbn);

            if (book == null)
                return Json(new { success = false, message = "Book not found for this ISBN." });

            var viewModel = (_bookService as BookService)?.ToBookDetailViewModel(book);

            return Json(new { success = true, data = viewModel });
        }

        [HttpGet]
        public async Task<IActionResult> EditBook(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
                return NotFound();

            var viewModel = _bookService.ToBookDetailViewModel(book);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBook(BookDetailViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var book = _bookService.ToBookFromDetailViewModel(model);

            await _bookService.UpdateBookAsync(book);
            return RedirectToAction("Index");
        }
    }
}
