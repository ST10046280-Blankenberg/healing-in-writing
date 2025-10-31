using HealingInWriting.Domain.Common;

namespace HealingInWriting.Interfaces.Services
{
    public interface IPrivacyPolicyService
    {
        Task<PrivacyPolicy> GetAsync();
        Task UpdateAsync(PrivacyPolicy entity, string updatedBy);
    }
}

