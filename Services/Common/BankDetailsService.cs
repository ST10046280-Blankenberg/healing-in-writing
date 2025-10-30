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
            // Get the first (and only) bank details record, or create default
            var entity = await _context.BankDetails.FirstOrDefaultAsync();
            
            if (entity == null)
            {
                entity = new BankDetails
                {
                    BankName = "Not Set",
                    AccountNumber = "Not Set",
                    Branch = "Not Set",
                    BranchCode = "Not Set",
                    UpdatedBy = "System",
                    UpdatedAt = DateTime.UtcNow
                };
                
                _context.BankDetails.Add(entity);
                await _context.SaveChangesAsync();
            }
            
            return entity;
        }

        public async Task UpdateAsync(BankDetails entity, string updatedBy)
        {
            var existing = await _context.BankDetails.FirstOrDefaultAsync();

            if (existing == null)
            {
                entity.UpdatedBy = updatedBy;
                entity.UpdatedAt = DateTime.UtcNow;
                _context.BankDetails.Add(entity);
            }
            else
            {
                existing.BankName = entity.BankName;
                existing.AccountNumber = entity.AccountNumber;
                existing.Branch = entity.Branch;
                existing.BranchCode = entity.BranchCode;
                existing.UpdatedBy = updatedBy;
                existing.UpdatedAt = DateTime.UtcNow;
                
                _context.BankDetails.Update(existing);
            }

            await _context.SaveChangesAsync();
        }
    }
}