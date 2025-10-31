using System.ComponentModel.DataAnnotations;

namespace HealingInWriting.Models.Common
{
    public class OurImpactViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "People Helped is required")]
        [Display(Name = "People Helped")]
        [Range(0, int.MaxValue, ErrorMessage = "People Helped must be a positive number")]
        public int PeopleHelped { get; set; } = 0;

        [Required(ErrorMessage = "Workshops Hosted is required")]
        [Display(Name = "Workshops Hosted")]
        [Range(0, int.MaxValue, ErrorMessage = "Workshops Hosted must be a positive number")]
        public int WorkshopsHosted { get; set; } = 0;

        [Required(ErrorMessage = "Partner Organisations is required")]
        [Display(Name = "Partner Organisations")]
        [Range(0, int.MaxValue, ErrorMessage = "Partner Organisations must be a positive number")]
        public int PartnerOrganisations { get; set; } = 0;

        [Required(ErrorMessage = "Cities Reached is required")]
        [Display(Name = "Cities Reached")]
        [Range(0, int.MaxValue, ErrorMessage = "Cities Reached must be a positive number")]
        public int CitiesReached { get; set; } = 0;

        public byte[]? RowVersion { get; set; }
    }
}

