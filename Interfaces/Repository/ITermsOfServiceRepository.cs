using HealingInWriting.Domain.Common;

namespace HealingInWriting.Interfaces.Repository
{
    public interface ITermsOfServiceRepository
    {
        Task AddAsync(TermsOfService entity);
        Task<TermsOfService?> GetAsync();
        Task UpdateAsync(TermsOfService entity);
    }
}
