using HealingInWriting.Domain.Gallery;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealingInWriting.Interfaces.Repository
{
    public interface IGalleryRepository
    {
        Task<GalleryItem?> GetByIdAsync(int id);
        Task<List<GalleryItem>> GetAllAsync();
        Task<List<GalleryItem>> GetByCollectionIdAsync(string collectionId);
        Task<(List<GalleryItem> items, int totalCount)> GetPagedAsync(int page, int pageSize);
        Task<GalleryItem> AddAsync(GalleryItem entity);
        Task UpdateAsync(GalleryItem entity);
        Task DeleteAsync(int id);
    }
}

