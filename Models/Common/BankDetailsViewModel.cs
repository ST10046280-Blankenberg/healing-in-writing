using System;
using System.ComponentModel.DataAnnotations;

namespace HealingInWriting.Models.Common
{
    public class BankDetailsViewModel
    {
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

        public DateTime UpdatedAt { get; set; }

        public byte[] RowVersion { get; set; }
    }
}