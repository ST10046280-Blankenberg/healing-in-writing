using HealingInWriting.Domain.Gallery;
using HealingInWriting.Interfaces.Services;
using HealingInWriting.Interfaces.Repository;
using System;

namespace HealingInWriting.Services.Gallery
{
    public class GalleryService : IGalleryService
    {
        private readonly IGalleryRepository _repository;

        public GalleryService(IGalleryRepository repository)
        {
            _repository = repository;
        }

        public async Task<GalleryItem?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<List<GalleryItem>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<List<GalleryItem>> GetByCollectionIdAsync(string collectionId)
        {
            return await _repository.GetByCollectionIdAsync(collectionId);
        }

        public async Task<(List<GalleryItem> items, int totalCount)> GetPagedAsync(int page, int pageSize)
        {
            return await _repository.GetPagedAsync(page, pageSize);
        }

        public async Task<GalleryItem> AddAsync(GalleryItem entity, string uploadedBy)
        {
            entity.UploadedBy = uploadedBy;
            entity.CreatedDate = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;
            return await _repository.AddAsync(entity);
        }

        public async Task UpdateAsync(GalleryItem entity, string updatedBy)
        {
            entity.UploadedBy = updatedBy;
            entity.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}

