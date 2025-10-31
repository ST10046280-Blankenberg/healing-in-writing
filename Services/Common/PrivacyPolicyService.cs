using HealingInWriting.Domain.Common;
using HealingInWriting.Interfaces.Services;
using HealingInWriting.Interfaces.Repository;
using System;
using System.Threading.Tasks;

namespace HealingInWriting.Services.Common
{
    public class PrivacyPolicyService : IPrivacyPolicyService
    {
        private readonly IPrivacyPolicyRepository _repository;

        public PrivacyPolicyService(IPrivacyPolicyRepository repository)
        {
            _repository = repository;
        }

        public async Task<PrivacyPolicy> GetAsync()
        {
            var entity = await _repository.GetAsync();
            if (entity == null)
            {
                entity = new PrivacyPolicy
                {
                    Content = "Use this page to detail your site's privacy policy.",
                    UpdatedBy = "System",
                    LastUpdated = DateTime.UtcNow
                };
                await _repository.AddAsync(entity);
            }
            return entity ?? await _repository.GetAsync();
        }

        public async Task UpdateAsync(PrivacyPolicy entity, string updatedBy)
        {
            // Set audit fields
            entity.UpdatedBy = updatedBy;
            entity.LastUpdated = DateTime.UtcNow;

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

