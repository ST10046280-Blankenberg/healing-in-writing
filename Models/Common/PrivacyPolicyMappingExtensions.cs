using HealingInWriting.Domain.Common;
using HealingInWriting.Models.Common;

namespace HealingInWriting.Models.Common
{
    public static class PrivacyPolicyMappingExtensions
    {
        public static PrivacyPolicyViewModel ToViewModel(this PrivacyPolicy entity)
        {
            return new PrivacyPolicyViewModel
            {
                Id = entity.Id,
                Title = entity.Title,
                Content = entity.Content,
                LastUpdated = entity.LastUpdated,
                RowVersion = entity.RowVersion
            };
        }

        public static PrivacyPolicy ToEntity(this PrivacyPolicyViewModel vm)
        {
            return new PrivacyPolicy
            {
                Id = vm.Id,
                Title = vm.Title,
                Content = vm.Content,
                LastUpdated = vm.LastUpdated,
                RowVersion = vm.RowVersion
            };
        }
    }
}

