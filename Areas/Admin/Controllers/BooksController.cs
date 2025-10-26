using HealingInWriting.Interfaces.Services;
using HealingInWriting.Models.Books;
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
        public IActionResult Add(string title, string author, string isbn, string description, int? publishedYear)
        {
            return RedirectToAction(nameof(Manage));
        }
    }
}
