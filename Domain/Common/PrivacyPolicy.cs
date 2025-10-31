using System;
using System.ComponentModel.DataAnnotations;

namespace HealingInWriting.Domain.Common
{
    public class PrivacyPolicy
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(200)]
        public string Title { get; set; } = "Privacy Policy";

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        [StringLength(200)]
        public string UpdatedBy { get; set; } = "System";

        public byte[]? RowVersion { get; set; }
    }
}

