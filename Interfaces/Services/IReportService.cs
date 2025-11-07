using HealingInWriting.Models.Admin;
using HealingInWriting.Models.Filters;

namespace HealingInWriting.Interfaces.Services;

public interface IReportService
{
    Task<ReportsDashboardViewModel> GetDashboardDataAsync(ReportsFilterViewModel filters);
}
