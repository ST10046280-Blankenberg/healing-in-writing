using HealingInWriting.Domain.Common;

namespace HealingInWriting.Models.Common
{
    public static class TermsOfServiceMappingExtensions
    {
        public static TermsOfServiceViewModel ToViewModel(this TermsOfService entity)
        {
            var templateData = entity.ContentFormat == Domain.Common.PolicyContentFormat.Template
                ? PolicyTemplateSerializer.Deserialize(entity.Content)
                : PolicyTemplateDefaults.CreateTermsDefaults();
            PolicyTemplateSerializer.PopulateTextFields(templateData);
            PolicyTemplateDefaults.EnsureMinimumSections(templateData);

            return new TermsOfServiceViewModel
            {
                Id = entity.Id,
                Content = entity.Content,
                LastUpdated = entity.LastUpdated,
                RowVersion = entity.RowVersion,
                ContentFormat = entity.ContentFormat,
                TemplateData = templateData,
                UseSimpleEditor = entity.ContentFormat == Domain.Common.PolicyContentFormat.Template
            };
        }

        public static TermsOfService ToEntity(this TermsOfServiceViewModel vm)
        {
            return new TermsOfService
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
