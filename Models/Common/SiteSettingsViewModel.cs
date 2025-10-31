using System;
using System.ComponentModel.DataAnnotations;

namespace HealingInWriting.Models.Common
{
    public class SiteSettingsViewModel
    {
        [Required]
        public BankDetailsViewModel BankDetails { get; set; } = new BankDetailsViewModel();

        [Required]
        public PrivacyPolicyViewModel PrivacyPolicy { get; set; } = new PrivacyPolicyViewModel();
    }
}
