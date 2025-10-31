using System;
using System.ComponentModel.DataAnnotations;

namespace HealingInWriting.Domain.Gallery
{
    public class GalleryItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(500)]
        public string ImageUrl { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string AltText { get; set; } = string.Empty;

        public bool IsAlbum { get; set; } = false;

        public int? AlbumPhotoCount { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [StringLength(200)]
        public string UploadedBy { get; set; } = "System";

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public byte[]? RowVersion { get; set; }
    }
}

