using HealingInWriting.Domain.Gallery;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealingInWriting.Interfaces.Services
{
    public interface IGalleryService
    {
        Task<GalleryItem?> GetByIdAsync(int id);
        Task<List<GalleryItem>> GetAllAsync();
        Task<(List<GalleryItem> items, int totalCount)> GetPagedAsync(int page, int pageSize);
        Task<GalleryItem> AddAsync(GalleryItem entity, string uploadedBy);
        Task UpdateAsync(GalleryItem entity, string updatedBy);
        Task DeleteAsync(int id);
    }
}

