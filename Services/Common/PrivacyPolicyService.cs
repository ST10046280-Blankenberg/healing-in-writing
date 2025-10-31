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
            var existing = await _repository.GetAsync();
            if (existing == null)
            {
                entity.UpdatedBy = updatedBy;
                entity.LastUpdated = DateTime.UtcNow;
                await _repository.AddAsync(entity);
            }
            else
            {
                existing.Content = entity.Content;
                existing.UpdatedBy = updatedBy;
                existing.LastUpdated = DateTime.UtcNow;
                await _repository.UpdateAsync(existing);
            }
        }
    }
}

