using HealingInWriting.Domain.Gallery;

namespace HealingInWriting.Models.Gallery
{
    public static class GalleryItemMappingExtensions
    {
        public static GalleryItemViewModel ToViewModel(this GalleryItem entity)
        {
            return new GalleryItemViewModel
            {
                Id = entity.Id,
                ImageUrl = entity.ImageUrl,
                AltText = entity.AltText,
                IsAlbum = entity.IsAlbum,
                AlbumPhotoCount = entity.AlbumPhotoCount,
                CreatedDate = entity.CreatedDate
            };
        }

        public static GalleryItem ToEntity(this GalleryItemViewModel vm)
        {
            return new GalleryItem
            {
                Id = vm.Id,
                ImageUrl = vm.ImageUrl,
                AltText = vm.AltText,
                IsAlbum = vm.IsAlbum,
                AlbumPhotoCount = vm.AlbumPhotoCount,
                CreatedDate = vm.CreatedDate
            };
        }
    }
}

