using HealingInWriting.Domain.Common;
using HealingInWriting.Models.Common;

namespace HealingInWriting.Models.Common
{
    public static class OurImpactMappingExtensions
    {
        public static OurImpactViewModel ToViewModel(this OurImpact entity)
        {
            return new OurImpactViewModel
            {
                Id = entity.Id,
                PeopleHelped = entity.PeopleHelped,
                WorkshopsHosted = entity.WorkshopsHosted,
                PartnerOrganisations = entity.PartnerOrganisations,
                CitiesReached = entity.CitiesReached,
                RowVersion = entity.RowVersion
            };
        }

        public static OurImpact ToEntity(this OurImpactViewModel vm)
        {
            return new OurImpact
            {
                Id = vm.Id,
                PeopleHelped = vm.PeopleHelped,
                WorkshopsHosted = vm.WorkshopsHosted,
                PartnerOrganisations = vm.PartnerOrganisations,
                CitiesReached = vm.CitiesReached,
                RowVersion = vm.RowVersion
            };
        }
    }
}

