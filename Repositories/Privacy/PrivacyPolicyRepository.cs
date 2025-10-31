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
            return await _context.PrivacyPolicies.FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(PrivacyPolicy entity)
        {
            _context.PrivacyPolicies.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}

