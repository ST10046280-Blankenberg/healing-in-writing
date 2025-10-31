using HealingInWriting.Domain.Common;
using HealingInWriting.Interfaces.Services;
using HealingInWriting.Interfaces.Repository;

namespace HealingInWriting.Services.Common
{
    public class OurImpactService : IOurImpactService
    {
        private readonly IOurImpactRepository _repository;

        public OurImpactService(IOurImpactRepository repository)
        {
            _repository = repository;
        }

        public async Task<OurImpact> GetAsync()
        {
            var entity = await _repository.GetAsync();

            if (entity == null)
            {
                entity = new OurImpact
                {
                    PeopleHelped = 0,
                    WorkshopsHosted = 0,
                    PartnerOrganisations = 0,
                    CitiesReached = 0,
                    UpdatedBy = "System",
                    UpdatedAt = DateTime.UtcNow
                };

                await _repository.AddAsync(entity);
            }

            return entity ?? await _repository.GetAsync();
        }

        public async Task UpdateAsync(OurImpact entity, string updatedBy)
        {
            // Set audit fields
            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.UtcNow;

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

