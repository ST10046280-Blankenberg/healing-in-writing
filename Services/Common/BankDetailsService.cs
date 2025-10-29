using System;
using System.Threading.Tasks;
using HealingInWriting.Data;
using HealingInWriting.Domain.Common;
using HealingInWriting.Interfaces.Services;
using Microsoft.EntityFrameworkCore;

namespace HealingInWriting.Services.Common
{
    public class BankDetailsService : IBankDetailsService
    {
        private readonly ApplicationDbContext _context;

        public BankDetailsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BankDetails> GetAsync()
        {
            return await _context.BankDetails.FirstOrDefaultAsync() 
                   ?? new BankDetails();
        }

        public async Task UpdateAsync(BankDetails entity, string updatedBy)
        {
            var existing = await _context.BankDetails.FirstOrDefaultAsync();

            if (existing == null)
            {
                entity.UpdatedAt = DateTime.UtcNow;
                entity.UpdatedBy = updatedBy;
                _context.BankDetails.Add(entity);
            }
            else
            {
                existing.BankName = entity.BankName;
                existing.AccountNumber = entity.AccountNumber;
                existing.Branch = entity.Branch;
                existing.BranchCode = entity.BranchCode;
                existing.UpdatedAt = DateTime.UtcNow;
                existing.UpdatedBy = updatedBy;
            }

            await _context.SaveChangesAsync();
        }
    }
}