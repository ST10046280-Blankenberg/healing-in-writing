using HealingInWriting.Domain.Common;
using HealingInWriting.Interfaces.Repository;
using HealingInWriting.Interfaces.Services;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace HealingInWriting.Services.Common
{
    public class PrivacyPolicyService : IPrivacyPolicyService
    {
        private readonly IPrivacyPolicyRepository _repository;
        private readonly ILogger<PrivacyPolicyService> _logger;

        public PrivacyPolicyService(
            IPrivacyPolicyRepository repository,
            ILogger<PrivacyPolicyService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<PrivacyPolicy> GetAsync()
        {
            try
            {
                var entity = await _repository.GetAsync();
                if (entity == null)
                {
                    entity = CreateDefaultPolicy();
                    await _repository.AddAsync(entity);
                }
                return entity;
            }
            catch (Exception ex) when (IsMissingTable(ex))
            {
                _logger.LogWarning(ex,
                    "PrivacyPolicies table not found. Returning default policy until migrations are applied.");
                return CreateDefaultPolicy();
            }
        }

        public async Task UpdateAsync(PrivacyPolicy entity, string updatedBy)
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
                    "Unable to persist privacy policy changes because the table is missing. Apply latest migrations.");
                throw new InvalidOperationException(
                    "Privacy policy storage has not been initialised. Please apply the latest database migrations.",
                    ex);
            }
        }

        private static PrivacyPolicy CreateDefaultPolicy()
        {
            return new PrivacyPolicy
            {
                Content = "Use this page to detail your site's privacy policy.",
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
