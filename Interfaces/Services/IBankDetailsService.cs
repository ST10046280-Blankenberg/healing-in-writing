using HealingInWriting.Domain.Common;

namespace HealingInWriting.Interfaces.Services;

public interface IBankDetailsService
{
    Task<BankDetails> GetAsync();
    Task UpdateAsync(BankDetails entity, string updatedBy);
}
