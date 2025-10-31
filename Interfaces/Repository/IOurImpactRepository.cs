using HealingInWriting.Domain.Common;

namespace HealingInWriting.Interfaces.Repository
{
    public interface IOurImpactRepository
    {
        Task<OurImpact?> GetAsync();
        Task AddAsync(OurImpact entity);
        Task UpdateAsync(OurImpact entity);
    }
}

