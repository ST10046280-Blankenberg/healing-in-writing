// HealingInWriting.Models.Common.BankDetailsMappingExtensions.cs
using HealingInWriting.Domain.Common;
using HealingInWriting.Models.Common;

public static class BankDetailsMappingExtensions
{
    public static BankDetailsViewModel ToViewModel(this BankDetails entity)
    {
        return new BankDetailsViewModel
        {
            Id = entity.Id,
            BankName = entity.BankName,
            AccountNumber = entity.AccountNumber,
            Branch = entity.Branch ?? string.Empty,
            BranchCode = entity.BranchCode ?? string.Empty,
            RowVersion = entity.RowVersion
        };
    }

    public static BankDetails ToEntity(this BankDetailsViewModel vm)
    {
        return new BankDetails
        {
            Id = vm.Id,
            BankName = vm.BankName,
            AccountNumber = vm.AccountNumber,
            Branch = vm.Branch,
            BranchCode = vm.BranchCode,
            RowVersion = vm.RowVersion
        };
    }
}