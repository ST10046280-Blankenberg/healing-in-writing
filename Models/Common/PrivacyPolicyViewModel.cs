using System;
using System.ComponentModel.DataAnnotations;

namespace HealingInWriting.Models.Common
{
    public class PrivacyPolicyViewModel
    {
        public int Id { get; set; }
        

        [Required(ErrorMessage = "Content is required")]
        [Display(Name = "Privacy Policy Content")]
        public string Content { get; set; } = string.Empty;

        public DateTime LastUpdated { get; set; }

        public byte[]? RowVersion { get; set; }
    }
}

