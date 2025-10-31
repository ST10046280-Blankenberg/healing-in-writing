using HealingInWriting.Interfaces.Services;
using HealingInWriting.Models.Gallery;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace HealingInWriting.Controllers
{
    public class GalleryController : Controller
    {
        private readonly IGalleryService _galleryService;

        public GalleryController(IGalleryService galleryService)
        {
            _galleryService = galleryService;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 12)
        {
            // Ensure valid pagination parameters
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 12;
            if (pageSize > 100) pageSize = 100; // Max page size for performance

            var (items, totalCount) = await _galleryService.GetPagedAsync(page, pageSize);

            var viewModel = new GalleryViewModel
            {
                Photos = items.Select(i => i.ToViewModel()).ToList(),
                TotalCount = totalCount,
                PageSize = pageSize,
                CurrentPage = page
            };

            return View(viewModel);
        }
    }
}
