using Pilgrims.PersonalFinances.Models;
using Pilgrims.PersonalFinances.Models.DTOs;
using Pilgrims.PersonalFinances.Models.Enums;

namespace Pilgrims.PersonalFinances.Services.Interfaces;

/// <summary>
/// Service interface for comprehensive budget management with tracking and alerts
/// </summary>
public interface IBudgetService
{
    // Basic CRUD Operations
    Task<IEnumerable<Budget>> GetAllBudgetsAsync();
    Task<Budget?> GetBudgetByIdAsync(string id);
    Task<Budget> CreateBudgetAsync(Budget budget);
    Task<Budget> UpdateBudgetAsync(Budget budget);
    Task<bool> DeleteBudgetAsync(string id);
    Task<bool> DeleteBudgetsAsync(IEnumerable<string> ids);

    // Budget Filtering and Search
    Task<IEnumerable<Budget>> GetActiveBudgetsAsync();
    Task<IEnumerable<Budget>> GetBudgetsByTypeAsync(BudgetType budgetType);
    Task<IEnumerable<Budget>> GetBudgetsByPeriodAsync(BudgetPeriod period);
    Task<IEnumerable<Budget>> GetBudgetsByPeriodAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<Budget>> GetBudgetsWithTrackingAsync();
    Task<IEnumerable<Budget>> GetBudgetsByCategoryAsync(string categoryId);
    Task<IEnumerable<Budget>> GetBudgetsByAccountAsync(string accountId);
    Task<IEnumerable<Budget>> GetBudgetsByTagAsync(string tag);
    Task<IEnumerable<Budget>> GetCurrentBudgetsAsync();
    Task<IEnumerable<Budget>> GetOverBudgetsAsync();

    // Budget Tracking and Calculations
    Task<decimal> CalculateBudgetUtilizationAsync(string budgetId);
    Task<decimal> GetBudgetSpentAmountAsync(string budgetId);
    Task<decimal> GetBudgetRemainingAmountAsync(string budgetId);
    Task<bool> UpdateBudgetSpentAmountAsync(string budgetId, decimal amount);
    Task<bool> RecalculateBudgetSpentAmountAsync(string budgetId);
    Task<bool> RecalculateAllBudgetSpentAmountsAsync();

    // Budget Alerts
    Task<IEnumerable<BudgetAlert>> GetBudgetAlertsAsync(string budgetId);
    Task<IEnumerable<BudgetAlert>> GetActiveBudgetAlertsAsync();
    Task<BudgetAlert> CreateBudgetAlertAsync(BudgetAlert alert);
    Task<BudgetAlert> UpdateBudgetAlertAsync(BudgetAlert alert);
    Task<bool> DeleteBudgetAlertAsync(string alertId);
    Task<bool> CheckAndCreateBudgetAlertsAsync(string budgetId);
    Task<bool> CheckAllBudgetAlertsAsync();
    Task<bool> DismissBudgetAlertAsync(string alertId);

    // Budget Templates
    Task<Budget> CreateBudgetFromTemplateAsync(string templateId, DateTime startDate);
    Task<Budget> SaveBudgetAsTemplateAsync(string budgetId, string templateName);
    Task<IEnumerable<Budget>> GetBudgetTemplatesAsync();
    Task<bool> DeleteBudgetTemplateAsync(string templateId);

    // Budget Rollover
    Task<bool> ProcessBudgetRolloverAsync(string budgetId);
    Task<bool> ProcessAllBudgetRolloversAsync();
    Task<decimal> CalculateRolloverAmountAsync(string budgetId);

    // Budget Analytics and Reporting
    Task<BudgetSummaryDto> GetBudgetSummaryAsync();
    Task<BudgetSummaryDto> GetBudgetSummaryByPeriodAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<MonthlyBudgetPerformanceDto>> GetMonthlyBudgetPerformanceAsync(int year);
    Task<BudgetVarianceReportDto> GetBudgetVarianceReportAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<Budget>> GetBudgetPerformanceHistoryAsync(string budgetId, int months = 12);

    // Budget Validation
    Task<bool> ValidateBudgetAsync(Budget budget);
    Task<bool> CanDeleteBudgetAsync(string budgetId);
    Task<bool> IsBudgetNameUniqueAsync(string name, string? excludeBudgetId = null);

    // Transaction Integration
    Task<bool> ProcessTransactionForBudgetsAsync(Transaction transaction);
    Task<bool> RemoveTransactionFromBudgetsAsync(Transaction transaction);
    Task<IEnumerable<Budget>> GetAffectedBudgetsAsync(Transaction transaction);

    // Budget Period Management
    Task<bool> CreateNextPeriodBudgetAsync(string budgetId);
    Task<bool> ArchiveExpiredBudgetsAsync();
    Task<DateTime> GetNextPeriodStartDateAsync(Budget budget);
    Task<DateTime> GetNextPeriodEndDateAsync(Budget budget);

    // Dashboard Data
    Task<object> GetBudgetDashboardDataAsync();
    Task<object> GetBudgetProgressDataAsync();
    Task<object> GetBudgetAlertsDataAsync();

    /// <summary>
    /// Gets budget analytics for a specific period
    /// </summary>
    Task<List<BudgetAnalysisDto>> GetBudgetAnalysisAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Gets budget analytics overview data
    /// </summary>
    Task<BudgetAnalyticsData> GetBudgetAnalyticsAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Gets budget performance metrics
    /// </summary>
    Task<BudgetPerformanceDto> GetBudgetPerformanceAsync(string budgetId, DateTime startDate, DateTime endDate);

    /// <summary>
    /// Gets budget variance analysis
    /// </summary>
    Task<List<BudgetVarianceDto>> GetBudgetVarianceAnalysisAsync(DateTime startDate, DateTime endDate);
}