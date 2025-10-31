using System.Collections.Generic;
using HealingInWriting.Models.Gallery;

namespace HealingInWriting.Models.Gallery;

public class GalleryViewModel
{
    public List<GalleryItemViewModel> Photos { get; set; } = new List<GalleryItemViewModel>();
    public int TotalCount { get; set; }
    public int PageSize { get; set; } = 12;
    public int CurrentPage { get; set; } = 1;
    public int TotalPages => (TotalCount + PageSize - 1) / PageSize;
    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
}
