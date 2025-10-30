// HealingInWriting.Models.Common.BankDetailsMappingExtensions.cs
using HealingInWriting.Domain.Common;
using HealingInWriting.Models.Common;

public static class BankDetailsMappingExtensions
{
    public static BankDetailsViewModel ToViewModel(this BankDetails entity)
    {
        if (entity == null) return new BankDetailsViewModel();
        return new BankDetailsViewModel
        {
            Id = entity.Id,
            BankName = entity.BankName,
            AccountNumber = entity.AccountNumber,
            Branch = entity.Branch,
            BranchCode = entity.BranchCode,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt,
            RowVersion = entity.RowVersion
        };
    }

    public static BankDetails ToEntity(this BankDetailsViewModel vm)
    {
        if (vm == null) return new BankDetails();
        return new BankDetails
        {
            Id = vm.Id,
            BankName = vm.BankName,
            AccountNumber = vm.AccountNumber,
            Branch = vm.Branch,
            BranchCode = vm.BranchCode,
            UpdatedBy = vm.UpdatedBy,
            UpdatedAt = vm.UpdatedAt,
            RowVersion = vm.RowVersion
        };
    }
}