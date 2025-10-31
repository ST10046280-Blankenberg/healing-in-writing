using System;

namespace HealingInWriting.Models.Gallery
{
    public class GalleryItemViewModel
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string AltText { get; set; } = string.Empty;
        public bool IsAlbum { get; set; } = false;
        public int? AlbumPhotoCount { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}

