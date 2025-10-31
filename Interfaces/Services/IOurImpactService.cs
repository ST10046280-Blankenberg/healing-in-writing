using HealingInWriting.Domain.Common;

namespace HealingInWriting.Interfaces.Services
{
    public interface IOurImpactService
    {
        Task<OurImpact> GetAsync();
        Task UpdateAsync(OurImpact entity, string updatedBy);
    }
}

