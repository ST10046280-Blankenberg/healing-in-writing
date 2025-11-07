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
                    AccountName = "Not Set",
                    AccountNumber = "Not Set",
                    AccountType = "Not Set",
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
            // Set audit fields
            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.UtcNow;

            // Check if entity exists
            var existing = await _repository.GetAsync();
            
            if (existing == null)
            {
                // Create new if doesn't exist
                await _repository.AddAsync(entity);
            }
            else
            {
                // Update existing - repository handles the tracking
                await _repository.UpdateAsync(entity);
            }
        }
    }
}
