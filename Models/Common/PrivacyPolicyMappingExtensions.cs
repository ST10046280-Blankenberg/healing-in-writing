using HealingInWriting.Domain.Common;
using HealingInWriting.Models.Common;

namespace HealingInWriting.Models.Common
{
    public static class PrivacyPolicyMappingExtensions
    {
        public static PrivacyPolicyViewModel ToViewModel(this PrivacyPolicy entity)
        {
            var templateData = entity.ContentFormat == Domain.Common.PolicyContentFormat.Template
                ? PolicyTemplateSerializer.Deserialize(entity.Content)
                : PolicyTemplateDefaults.CreatePrivacyDefaults();
            PolicyTemplateSerializer.PopulateTextFields(templateData);

            return new PrivacyPolicyViewModel
            {
                Id = entity.Id,
                Content = entity.Content,
                LastUpdated = entity.LastUpdated,
                RowVersion = entity.RowVersion,
                ContentFormat = entity.ContentFormat,
                TemplateData = templateData
            };
        }

        public static PrivacyPolicy ToEntity(this PrivacyPolicyViewModel vm)
        {
            return new PrivacyPolicy
            {
                Id = vm.Id,
                Content = vm.Content,
                LastUpdated = vm.LastUpdated,
                RowVersion = vm.RowVersion,
                ContentFormat = vm.ContentFormat
            };
        }
    }
}
