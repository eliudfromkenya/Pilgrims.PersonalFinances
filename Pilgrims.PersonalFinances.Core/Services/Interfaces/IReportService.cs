using Pilgrims.PersonalFinances.Models.DTOs;

namespace Pilgrims.PersonalFinances.Services.Interfaces
{
    /// <summary>
    /// Interface for comprehensive financial reporting and analytics services
    /// </summary>
    public interface IReportService
    {
        // Standard Reports
        Task<IncomeExpenseReportDto> GenerateIncomeStatementAsync(DateTime startDate, DateTime endDate);
        Task<BalanceSheetDto> GenerateBalanceSheetAsync(DateTime asOfDate);
        Task<CashFlowReportDto> GenerateCashFlowStatementAsync(DateTime startDate, DateTime endDate);
        Task<BudgetVarianceReportDto> GenerateBudgetVarianceReportAsync(DateTime startDate, DateTime endDate);
        Task<List<Models.DTOs.NetWorthTrendDto>> GenerateNetWorthTrendAsync(DateTime startDate, DateTime endDate);
        Task<Models.DTOs.CategoryAnalysisDto> GenerateCategoryAnalysisAsync(DateTime startDate, DateTime endDate);
        Task<Models.DTOs.AccountSummaryReportDto> GenerateAccountSummaryReportAsync(DateTime startDate, DateTime endDate);

        // Comparison Features
        Task<ComparisonReportDto> GenerateYearOverYearComparisonAsync(int currentYear, int previousYear);
        Task<ComparisonReportDto> GenerateMonthOverMonthComparisonAsync(DateTime currentMonth, DateTime previousMonth);
    }
}