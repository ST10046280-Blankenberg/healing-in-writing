using HealingInWriting.Domain.Common;

namespace HealingInWriting.Models.Common
{
    public static class TermsOfServiceMappingExtensions
    {
        public static TermsOfServiceViewModel ToViewModel(this TermsOfService entity)
        {
            return new TermsOfServiceViewModel
            {
                Id = entity.Id,
                Content = entity.Content,
                LastUpdated = entity.LastUpdated,
                RowVersion = entity.RowVersion
            };
        }

        public static TermsOfService ToEntity(this TermsOfServiceViewModel vm)
        {
            return new TermsOfService
            {
                Id = vm.Id,
                Content = vm.Content,
                LastUpdated = vm.LastUpdated,
                RowVersion = vm.RowVersion
            };
        }
    }
}
