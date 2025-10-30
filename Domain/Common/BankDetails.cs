using System;
using System.ComponentModel.DataAnnotations;

namespace HealingInWriting.Domain.Common
{
    public class BankDetails
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string BankName { get; set; }

        [Required, StringLength(50)]
        public string AccountNumber { get; set; }

        [StringLength(100)]
        public string Branch { get; set; }

        [StringLength(20)]
        public string BranchCode { get; set; }

        [StringLength(200)]
        public string UpdatedBy { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Remove [Timestamp] attribute for SQLite compatibility
        public byte[]? RowVersion { get; set; }
    }
}