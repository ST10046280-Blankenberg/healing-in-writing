using HealingInWriting.Domain.Common;

namespace HealingInWriting.Interfaces.Repository
{
    public interface IPrivacyPolicyRepository
    {
        Task AddAsync(PrivacyPolicy entity);
        Task<PrivacyPolicy?> GetAsync();
        Task UpdateAsync(PrivacyPolicy entity);
    }
}

