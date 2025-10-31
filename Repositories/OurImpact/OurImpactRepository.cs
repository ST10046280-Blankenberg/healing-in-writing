using HealingInWriting.Data;
using HealingInWriting.Domain.Common;
using HealingInWriting.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace HealingInWriting.Repositories.OurImpactFolder
{
    public class OurImpactRepository : IOurImpactRepository
    {
        private readonly ApplicationDbContext _context;

        public OurImpactRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(OurImpact entity)
        {
            _context.OurImpacts.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<OurImpact?> GetAsync()
        {
            return await _context.OurImpacts.AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(OurImpact entity)
        {
            var existing = await _context.OurImpacts.FindAsync(entity.Id);
            if (existing != null)
            {
                _context.Entry(existing).CurrentValues.SetValues(entity);
                await _context.SaveChangesAsync();
            }
            else
            {
                _context.OurImpacts.Update(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}

