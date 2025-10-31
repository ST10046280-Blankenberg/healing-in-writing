﻿using HealingInWriting.Data;
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
            return await _context.BankDetails.AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(BankDetails entity)
        {
            var existing = await _context.BankDetails.FindAsync(entity.Id);
            if (existing != null)
            {
                _context.Entry(existing).CurrentValues.SetValues(entity);
                await _context.SaveChangesAsync();
            }
            else
            {
                _context.BankDetails.Update(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}