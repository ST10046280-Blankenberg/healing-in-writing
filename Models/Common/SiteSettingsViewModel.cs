using System;
using System.ComponentModel.DataAnnotations;
using HealingInWriting.Models.Gallery;

namespace HealingInWriting.Models.Common
{
    public class SiteSettingsViewModel
    {
        [Required]
        public BankDetailsViewModel BankDetails { get; set; } = new BankDetailsViewModel();

        [Required]
        public PrivacyPolicyViewModel PrivacyPolicy { get; set; } = new PrivacyPolicyViewModel();

        [Required]
        public OurImpactViewModel OurImpact { get; set; } = new OurImpactViewModel();
        
        public List<GalleryItemViewModel> GalleryItems { get; set; } = new List<GalleryItemViewModel>();

    }
}
