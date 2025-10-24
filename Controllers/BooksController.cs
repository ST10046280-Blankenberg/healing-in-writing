using HealingInWriting.Domain.Books;
using HealingInWriting.Interfaces.Services;
using HealingInWriting.Models.Books;
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

        // Show newest books only
        public async Task<IActionResult> Index()
        {
            var books = (await _bookService.GetFeaturedAsync())
                .OrderByDescending(b => b.PublishedDate)
                .Take(10) // Show top 10 newest books
                .ToList();

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
                    ThumbnailUrl = book.ImageLinks?.Thumbnail ?? book.ImageLinks?.Thumbnail ?? string.Empty
                }).ToList(),
                AvailableAuthors = books.SelectMany(book => book.Authors ?? Enumerable.Empty<string>())
                    .Distinct()
                    .OrderBy(author => author)
                    .ToList(),
                AvailableCategories = books.SelectMany(book => book.Categories ?? Enumerable.Empty<string>())
                    .Distinct()
                    .OrderBy(category => category)
                    .ToList()
            };

            return View(viewModel);
        }

        // AJAX filter endpoint
        [HttpGet]
        public async Task<IActionResult> Filter(string searchTerm, string selectedAuthor, string selectedCategory)
        {
            var books = await _bookService.GetFeaturedFilteredAsync(searchTerm, selectedAuthor, selectedCategory, null);

            var filteredBooks = books.Select(book => new BookSummaryViewModel
            {
                BookId = book.BookId,
                Title = book.Title,
                Authors = book.Authors?.Any() == true ? string.Join(", ", book.Authors) : string.Empty,
                PublishedDate = book.PublishedDate ?? string.Empty,
                Publisher = book.Publisher ?? string.Empty,
                PageCount = book.PageCount,
                Description = book.Description ?? string.Empty,
                Categories = book.Categories?.ToList() ?? new List<string>(),
                ThumbnailUrl = book.ImageLinks?.Thumbnail ?? book.ImageLinks?.Thumbnail ?? string.Empty
            }).ToList();

            // Return a partial view with just the book cards
            return PartialView("_BookCardsPartial", filteredBooks);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> ImportBookByIsbn(string isbn)
        {
            if (string.IsNullOrWhiteSpace(isbn))
                return Json(new { success = false, message = "ISBN required." });

            var book = await _bookService.ImportBookByIsbnAsync(isbn);

            if (book == null)
                return Json(new { success = false, message = "Book not found for this ISBN." });

            var viewModel = new BookDetailViewModel
            {
                BookId = book.BookId,
                Title = book.Title,
                Authors = string.Join(", ", book.Authors ?? new List<string>()),
                PublishedDate = book.PublishedDate,
                Description = book.Description,
                Categories = book.Categories ?? new List<string>(),
                ThumbnailUrl = book.ImageLinks?.Thumbnail ?? book.ImageLinks?.SmallThumbnail ?? "/images/placeholder-book.svg",
                PageCount = book.PageCount,
                Language = book.Language,
                Publisher = book.Publisher,
                IndustryIdentifiers = book.IndustryIdentifiers?.Select(i => i.Identifier).ToList() ?? new List<string>()
            };

            return Json(new { success = true, data = viewModel });
        }

        //    [HttpPost]
        //    [Authorize(Roles = "Admin")]
        //    public async Task<IActionResult> AddBook(IFormCollection form)
        //    {
        //        var isbns = new List<string>();
        //        if (!string.IsNullOrWhiteSpace(form["IsbnPrimary"])) isbns.Add(form["IsbnPrimary"]);
        //        if (!string.IsNullOrWhiteSpace(form["IsbnSecondary"])) isbns.Add(form["IsbnSecondary"]);

        //        var book = new Book
        //        {
        //            Title = form["Title"],
        //            Authors = form["Author"].ToString().Split(',').Select(a => a.Trim()).ToList(),
        //            Publisher = form["Publisher"],
        //            PublishedDate = form["PublishDate"],
        //            Description = form["Description"],
        //            PageCount = int.TryParse(form["PageCount"], out var pc) ? pc : 0,
        //            Categories = form["Categories"].ToString().Split(',').Select(c => c.Trim()).Where(c => !string.IsNullOrWhiteSpace(c)).ToList(),
        //            Language = form["Language"],
        //            IndustryIdentifiers = isbns.Select(isbn => new IndustryIdentifier
        //            {
        //                Type = isbn.Length == 13 ? "ISBN_13" : "ISBN_10",
        //                Identifier = isbn.Trim()
        //            }).ToList(),
        //            // TODO: Handle ImageLinks, PreviewLink, InfoLink as needed
        //        };

        //        // TODO: Save book to database or in-memory collection

        //        return RedirectToAction("ManageBooks");
        //    }
    }
}
