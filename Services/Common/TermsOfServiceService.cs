using HealingInWriting.Domain.Common;
using HealingInWriting.Interfaces.Repository;
using HealingInWriting.Interfaces.Services;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace HealingInWriting.Services.Common
{
    public class TermsOfServiceService : ITermsOfServiceService
    {
        private readonly ITermsOfServiceRepository _repository;
        private readonly ILogger<TermsOfServiceService> _logger;

        public TermsOfServiceService(
            ITermsOfServiceRepository repository,
            ILogger<TermsOfServiceService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<TermsOfService> GetAsync()
        {
            try
            {
                var entity = await _repository.GetAsync();
                if (entity == null)
                {
                    entity = CreateDefaultTerms();
                    await _repository.AddAsync(entity);
                }
                return entity;
            }
            catch (Exception ex) when (IsMissingTable(ex))
            {
                _logger.LogWarning(ex,
                    "TermsOfServices table not found. Returning default terms until migrations are applied.");
                return CreateDefaultTerms();
            }
        }

        public async Task UpdateAsync(TermsOfService entity, string updatedBy)
        {
            try
            {
                entity.UpdatedBy = updatedBy;
                entity.LastUpdated = DateTime.UtcNow;

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
                    "Unable to persist terms changes because the table is missing. Apply latest migrations.");
                throw new InvalidOperationException(
                    "Terms of service storage has not been initialised. Please apply the latest database migrations.",
                    ex);
            }
        }

        private static TermsOfService CreateDefaultTerms()
        {
            return new TermsOfService
            {
                Content = "Use this page to detail your site's terms of service.",
                UpdatedBy = "System",
                LastUpdated = DateTime.UtcNow
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
