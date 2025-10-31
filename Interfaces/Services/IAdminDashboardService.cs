using HealingInWriting.Models.Admin;

namespace HealingInWriting.Interfaces.Services;

public interface IAdminDashboardService
{
    Task<AdminDashboardViewModel> GetDashboardAsync(DateTime utcNow);
}
