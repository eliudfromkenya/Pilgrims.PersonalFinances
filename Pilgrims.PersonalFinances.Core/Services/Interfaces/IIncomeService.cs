using Pilgrims.PersonalFinances.Core.Models;

namespace Pilgrims.PersonalFinances.Core.Services.Interfaces
{
    /// <summary>
    /// Interface for comprehensive income management and forecasting services
    /// </summary>
    public interface IIncomeService
    {
        // Basic CRUD Operations - Income
        Task<IEnumerable<Income>> GetAllIncomesAsync();
        Task<Income?> GetIncomeByIdAsync(string id);
        Task<Income> CreateIncomeAsync(Income income);
        Task<Income> UpdateIncomeAsync(Income income);
        Task<bool> DeleteIncomeAsync(string id);

        // Basic CRUD Operations - Income Categories
        Task<IEnumerable<IncomeCategory>> GetAllIncomeCategoriesAsync();
        Task<IncomeCategory?> GetIncomeCategoryByIdAsync(string id);
        Task<IncomeCategory> CreateIncomeCategoryAsync(IncomeCategory category);
        Task<IncomeCategory> UpdateIncomeCategoryAsync(IncomeCategory category);
        Task<bool> DeleteIncomeCategoryAsync(string id);

        // Income Filtering and Search
        Task<IEnumerable<Income>> GetIncomesByTypeAsync(string incomeType);
        Task<IEnumerable<Income>> GetIncomesByFrequencyAsync(string frequency);
        Task<IEnumerable<Income>> GetRegularIncomesAsync();
        Task<IEnumerable<Income>> GetVariableIncomesAsync();
        Task<IEnumerable<Income>> GetIncomesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Income>> GetIncomesByCategoryAsync(string categoryId);
        Task<IEnumerable<Income>> SearchIncomesAsync(string searchTerm);
        Task<IEnumerable<Income>> GetActiveIncomesAsync();
        Task<IEnumerable<Income>> GetUpcomingIncomesAsync(int days = 30);

        // Income Calculations and Analytics
        Task<decimal> GetTotalIncomeAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<decimal> GetTotalIncomeByTypeAsync(string incomeType, DateTime? startDate = null, DateTime? endDate = null);
        Task<decimal> GetTotalIncomeByCategoryAsync(string categoryId, DateTime? startDate = null, DateTime? endDate = null);
        Task<decimal> GetMonthlyIncomeAverageAsync(int months = 12);
        Task<decimal> GetYearlyIncomeProjectionAsync();
        Task<decimal> GetNetIncomeAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<decimal> GetGrossIncomeAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<int> GetIncomeCountByCategoryAsync(string categoryId);

        // Income Forecasting
        Task<decimal> ForecastIncomeAsync(DateTime targetDate);
        Task<decimal> ForecastMonthlyIncomeAsync(int year, int month);
        Task<decimal> ForecastYearlyIncomeAsync(int year);
        Task<IEnumerable<(DateTime Date, decimal Amount)>> GetIncomeProjectionAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<(string Month, decimal Amount)>> GetMonthlyIncomeHistoryAsync(int months = 12);

        // Regular Income Management
        Task<IEnumerable<Income>> GetRecurringIncomesAsync();
        Task<Income?> GetNextExpectedIncomeAsync(string incomeId);
        Task<bool> ProcessRecurringIncomeAsync(string incomeId);
        Task<IEnumerable<Income>> GetOverdueIncomesAsync();
        Task<bool> UpdateNextExpectedDateAsync(string incomeId, DateTime nextDate);

        // Tax Calculations
        Task<decimal> GetTotalTaxableIncomeAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<decimal> GetTotalTaxPaidAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<decimal> GetEstimatedTaxLiabilityAsync(decimal taxRate);
        Task<IEnumerable<Income>> GetPreTaxIncomesAsync();
        Task<IEnumerable<Income>> GetPostTaxIncomesAsync();

        // Income Statistics and Reporting
        Task<Dictionary<string, decimal>> GetIncomeByTypeBreakdownAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<Dictionary<string, decimal>> GetIncomeByCategoryBreakdownAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<Dictionary<string, decimal>> GetIncomeByFrequencyBreakdownAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<(decimal Regular, decimal Variable)> GetRegularVsVariableIncomeAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<decimal> GetIncomeGrowthRateAsync(int months = 12);
        Task<decimal> GetAverageIncomePerSourceAsync();

        // Income Source Management
        Task<IEnumerable<string>> GetIncomeSourcesAsync();
        Task<decimal> GetIncomeBySourceAsync(string source, DateTime? startDate = null, DateTime? endDate = null);
        Task<IEnumerable<Income>> GetIncomesBySourceAsync(string source);

        // Validation and Business Rules
        Task<bool> ValidateIncomeNameAsync(string name, string? excludeIncomeId = null);
        Task<bool> ValidateIncomeCategoryNameAsync(string name, string? excludeCategoryId = null);
        Task<bool> CanDeleteIncomeCategoryAsync(string categoryId);
        Task<bool> HasIncomesAsync(string categoryId);

        // Dashboard and Summary Data
        Task<(decimal ThisMonth, decimal LastMonth, decimal YearToDate)> GetIncomeSummaryAsync();
        Task<IEnumerable<Income>> GetRecentIncomesAsync(int count = 10);
        Task<IEnumerable<Income>> GetTopIncomeSourcesAsync(int count = 5, DateTime? startDate = null, DateTime? endDate = null);
        Task<decimal> GetMonthlyIncomeTargetAsync();
        Task<bool> IsIncomeTargetMetAsync(int year, int month);
    }
}
