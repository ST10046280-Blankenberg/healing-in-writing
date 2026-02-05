using HealingInWriting.Domain.Common;

namespace HealingInWriting.Interfaces.Services
{
    public interface ITermsOfServiceService
    {
        Task<TermsOfService> GetAsync();
        Task UpdateAsync(TermsOfService entity, string updatedBy);
    }
}
