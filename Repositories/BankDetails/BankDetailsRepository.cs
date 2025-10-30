using HealingInWriting.Data;
using HealingInWriting.Domain.Common;
using HealingInWriting.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace HealingInWriting.Repositories.BankDetailsFolder
{
    public class BankDetailsRepository : IBankDetailsRepository
    {
        private readonly ApplicationDbContext _context;

        public BankDetailsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(BankDetails entity)
        {
            _context.BankDetails.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<BankDetails?> GetAsync()
        {
            return await _context.BankDetails.FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(BankDetails entity)
        {
            _context.BankDetails.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}