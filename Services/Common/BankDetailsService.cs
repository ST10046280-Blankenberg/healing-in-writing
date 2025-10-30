using HealingInWriting.Domain.Common;
using HealingInWriting.Interfaces.Services;
using HealingInWriting.Interfaces.Repository;

namespace HealingInWriting.Services.Common
{
    public class BankDetailsService : IBankDetailsService
    {
        private readonly IBankDetailsRepository _repository;

        public BankDetailsService(IBankDetailsRepository repository)
        {
            _repository = repository;
        }

        public async Task<BankDetails> GetAsync()
        {
            // Get the first (and only) bank details record, or create default
            var entity = await _repository.GetAsync();

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

                await _repository.AddAsync(entity);
                // No need to call SaveChangesAsync separately, AddAsync handles it
            }

            return entity ?? await _repository.GetAsync();
        }

        public async Task UpdateAsync(BankDetails entity, string updatedBy)
        {
            var existing = await _repository.GetAsync();

            if (existing == null)
            {
                entity.UpdatedBy = updatedBy;
                entity.UpdatedAt = DateTime.UtcNow;
                await _repository.AddAsync(entity);
                // No need to call SaveChangesAsync separately, AddAsync handles it
            }
            else
            {
                existing.BankName = entity.BankName;
                existing.AccountNumber = entity.AccountNumber;
                existing.Branch = entity.Branch;
                existing.BranchCode = entity.BranchCode;
                existing.UpdatedBy = updatedBy;
                existing.UpdatedAt = DateTime.UtcNow;

                await _repository.UpdateAsync(existing);
                // No need to call SaveChangesAsync separately, UpdateAsync handles it
            }
        }
    }
}