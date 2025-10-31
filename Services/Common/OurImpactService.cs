using HealingInWriting.Domain.Common;
using HealingInWriting.Interfaces.Repository;
using HealingInWriting.Interfaces.Services;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace HealingInWriting.Services.Common
{
    public class OurImpactService : IOurImpactService
    {
        private readonly IOurImpactRepository _repository;
        private readonly ILogger<OurImpactService> _logger;

        public OurImpactService(
            IOurImpactRepository repository,
            ILogger<OurImpactService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<OurImpact> GetAsync()
        {
            try
            {
                var entity = await _repository.GetAsync();

                if (entity == null)
                {
                    entity = CreateDefaultImpact();
                    await _repository.AddAsync(entity);
                }

                return entity;
            }
            catch (Exception ex) when (IsMissingTable(ex))
            {
                _logger.LogWarning(ex,
                    "OurImpacts table not found. Returning default values until migrations are applied.");
                return CreateDefaultImpact();
            }
        }

        public async Task UpdateAsync(OurImpact entity, string updatedBy)
        {
            try
            {
                entity.UpdatedBy = updatedBy;
                entity.UpdatedAt = DateTime.UtcNow;

                var existing = await _repository.GetAsync();

                if (existing == null)
                {
                    await _repository.AddAsync(entity);
                }
                else
                {
                    await _repository.UpdateAsync(entity);
                }
            }
            catch (Exception ex) when (IsMissingTable(ex))
            {
                _logger.LogError(ex,
                    "Unable to persist OurImpact changes because the table is missing. Apply latest migrations.");
                throw new InvalidOperationException(
                    "OurImpact storage has not been initialised. Please apply the latest database migrations.",
                    ex);
            }
        }

        private static OurImpact CreateDefaultImpact()
        {
            return new OurImpact
            {
                PeopleHelped = 0,
                WorkshopsHosted = 0,
                PartnerOrganisations = 0,
                CitiesReached = 0,
                UpdatedBy = "System",
                UpdatedAt = DateTime.UtcNow
            };
        }

        private static bool IsMissingTable(Exception exception)
        {
            return exception switch
            {
                SqlException sqlEx when sqlEx.Number == 208 => true,
                SqliteException sqliteEx when sqliteEx.SqliteErrorCode == 1 &&
                    sqliteEx.Message.Contains("no such table", StringComparison.OrdinalIgnoreCase) => true,
                _ => false
            };
        }
    }
}
