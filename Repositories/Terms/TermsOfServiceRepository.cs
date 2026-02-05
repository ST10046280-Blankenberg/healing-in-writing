using HealingInWriting.Data;
using HealingInWriting.Domain.Common;
using HealingInWriting.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HealingInWriting.Repositories.Terms
{
    public class TermsOfServiceRepository : ITermsOfServiceRepository
    {
        private readonly ApplicationDbContext _context;

        public TermsOfServiceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TermsOfService entity)
        {
            _context.TermsOfServices.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<TermsOfService?> GetAsync()
        {
            return await _context.TermsOfServices.AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(TermsOfService entity)
        {
            var existing = await _context.TermsOfServices.FindAsync(entity.Id);
            if (existing != null)
            {
                _context.Entry(existing).CurrentValues.SetValues(entity);
                await _context.SaveChangesAsync();
            }
            else
            {
                _context.TermsOfServices.Update(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
