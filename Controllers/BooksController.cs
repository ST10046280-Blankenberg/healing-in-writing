using HealingInWriting.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

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
            var books = await _bookService.GetPagedForUserAsync(
                searchTerm: null,
                selectedAuthor: null,
                selectedCategory: null,
                selectedTag: null,
                skip: 0,
                take: 10);

            var allAuthors = await _bookService.GetAllAuthorsAsync(onlyVisible: true);
            var allCategories = await _bookService.GetAllCategoriesAsync(onlyVisible: true);

            var viewModel = _bookService.ToBookListViewModel(books);
            viewModel.AvailableAuthors = allAuthors;
            viewModel.AvailableCategories = allCategories;

            // Get the total count for initial pagination
            var totalCount = await _bookService.GetCountForUserAsync(
                searchTerm: null,
                selectedAuthor: null,
                selectedCategory: null,
                selectedTag: null);

            ViewBag.TotalCount = totalCount;
            ViewBag.PageSize = 20;

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

            // Get the total count for the current filter
            var totalCount = await _bookService.GetCountForUserAsync(
                searchTerm,
                selectedAuthor,
                selectedCategory,
                null);

            // Use the helper to render the partial view to string
            var html = await RenderPartialViewToStringAsync("_BookCardsPartial", filteredBooks);

            return Json(new { html, totalCount });
        }

        /// <summary>
        /// Helper to render a partial view to string for AJAX responses.
        /// </summary>
        private async Task<string> RenderPartialViewToStringAsync(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.ActionDescriptor.ActionName;

            ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                var viewEngine = HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
                var viewResult = viewEngine.FindView(ControllerContext, viewName, false);

                if (!viewResult.Success)
                {
                    throw new InvalidOperationException($"View '{viewName}' not found.");
                }

                var viewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    ViewData,
                    TempData,
                    sw,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}
