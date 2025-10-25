using HealingInWriting.Interfaces.Services;
using HealingInWriting.Models.Books;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace HealingInWriting.Controllers;

public class AdminController : Controller
{
    private readonly IBookService _bookService;

    public AdminController(IBookService bookService)
    {
        _bookService = bookService;
    }

    // GET: Admin Dashboard
    public IActionResult Index()
    {
        return View();
    }

    // GET: Manage Books
    public async Task<IActionResult> ManageBooks()
    {
        var books = (await _bookService.GetFeaturedAsync()).ToList();

        // Map to your inventory view model. Adjust as needed for your actual BookInventoryViewModel.
        var model = new BookInventoryViewModel
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
                ThumbnailUrl = book.ImageLinks?.Thumbnail ?? string.Empty
            }).ToList()
        };

        return View(model);
    }

    // GET: Add Book Form
    public IActionResult AddBook()
    {
        return View();
    }

    // POST: Add Book (placeholder - will be implemented with actual logic later)
    [HttpPost]
    public IActionResult AddBook(string title, string author, string isbn, string description, int? publishedYear)
    {
        // TODO: Implement book creation logic
        // For now, just redirect back to ManageBooks
        return RedirectToAction(nameof(ManageBooks));
    }

    // GET: Manage Stories
    public IActionResult ManageStories()
    {
        return View();
    }

    // GET: Manage Events (placeholder)
    public IActionResult ManageEvents()
    {
        return View();
    }

    // GET: Approve Volunteer Hours (placeholder)
    public IActionResult ApproveVolunteerHours()
    {
        return View();
    }

    // GET: Reports (placeholder)
    public IActionResult Reports()
    {
        return View();
    }
}

