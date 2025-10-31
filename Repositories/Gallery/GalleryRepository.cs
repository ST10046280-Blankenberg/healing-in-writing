using HealingInWriting.Data;
using HealingInWriting.Domain.Gallery;
using HealingInWriting.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace HealingInWriting.Repositories.GalleryFolder
{
    public class GalleryRepository : IGalleryRepository
    {
        private readonly ApplicationDbContext _context;

        public GalleryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<GalleryItem?> GetByIdAsync(int id)
        {
            return await _context.GalleryItems.AsNoTracking().FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<List<GalleryItem>> GetAllAsync()
        {
            return await _context.GalleryItems
                .AsNoTracking()
                .OrderByDescending(g => g.CreatedDate)
                .ToListAsync();
        }

        public async Task<(List<GalleryItem> items, int totalCount)> GetPagedAsync(int page, int pageSize)
        {
            var query = _context.GalleryItems.AsNoTracking();
            var totalCount = await query.CountAsync();
            
            var items = await query
                .OrderByDescending(g => g.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<GalleryItem> AddAsync(GalleryItem entity)
        {
            _context.GalleryItems.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(GalleryItem entity)
        {
            var existing = await _context.GalleryItems.FindAsync(entity.Id);
            if (existing != null)
            {
                _context.Entry(existing).CurrentValues.SetValues(entity);
                await _context.SaveChangesAsync();
            }
            else
            {
                _context.GalleryItems.Update(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.GalleryItems.FindAsync(id);
            if (entity != null)
            {
                _context.GalleryItems.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}