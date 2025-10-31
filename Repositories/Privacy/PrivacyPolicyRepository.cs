using HealingInWriting.Data;
using HealingInWriting.Domain.Common;
using HealingInWriting.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HealingInWriting.Repositories.Privacy
{
    public class PrivacyPolicyRepository : IPrivacyPolicyRepository
    {
        private readonly ApplicationDbContext _context;

        public PrivacyPolicyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PrivacyPolicy entity)
        {
            _context.PrivacyPolicies.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<PrivacyPolicy?> GetAsync()
        {
            return await _context.PrivacyPolicies.AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(PrivacyPolicy entity)
        {
            var existing = await _context.PrivacyPolicies.FindAsync(entity.Id);
            if (existing != null)
            {
                _context.Entry(existing).CurrentValues.SetValues(entity);
                await _context.SaveChangesAsync();
            }
            else
            {
                _context.PrivacyPolicies.Update(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}

