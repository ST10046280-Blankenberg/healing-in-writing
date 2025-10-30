using HealingInWriting.Domain.Common;

namespace HealingInWriting.Interfaces.Repository
{
    public interface IBankDetailsRepository
    {
        Task AddAsync(BankDetails entity);
        Task<BankDetails?> GetAsync();
        Task UpdateAsync(BankDetails entity);
    }
}
