using System.ComponentModel.DataAnnotations;
using HealingInWriting.Domain.Common;

namespace HealingInWriting.Models.Common
{
    public class BankDetailsViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Bank Name is required")]
        [Display(Name = "Bank Name")]
        public string BankName { get; set; }

        [Required(ErrorMessage = "Account Name is required")]
        [Display(Name = "Account Name")]
        public string AccountName { get; set; }

        [Required(ErrorMessage = "Account Number is required")]
        [Display(Name = "Account Number")]
        public string AccountNumber { get; set; }

        [Required(ErrorMessage = "Account Type is required")]
        [Display(Name = "Account Type")]
        public string AccountType { get; set; }

        [Display(Name = "Branch")]
        public string Branch { get; set; }

        [Display(Name = "Branch Code")]
        public string BranchCode { get; set; }

        public byte[]? RowVersion { get; set; }
    }


}
