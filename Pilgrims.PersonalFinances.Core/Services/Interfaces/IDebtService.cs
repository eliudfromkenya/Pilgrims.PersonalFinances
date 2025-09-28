using Pilgrims.PersonalFinances.Models;
using Pilgrims.PersonalFinances.Models.DTOs;
using Pilgrims.PersonalFinances.Models.Enums;

namespace Pilgrims.PersonalFinances.Services.Interfaces;

/// <summary>
/// Service interface for comprehensive debt and creditor management with payment tracking
/// </summary>
public interface IDebtService
{
    // Basic CRUD Operations - Debts
    Task<IEnumerable<Debt>> GetAllDebtsAsync();
    Task<Debt?> GetDebtByIdAsync(string id);
    Task<Debt> CreateDebtAsync(Debt debt);
    Task<Debt> UpdateDebtAsync(Debt debt);
    Task<bool> DeleteDebtAsync(string id);
    Task<bool> DeleteDebtsAsync(IEnumerable<string> ids);

    // Basic CRUD Operations - Creditors
    Task<IEnumerable<Creditor>> GetAllCreditorsAsync();
    Task<Creditor?> GetCreditorByIdAsync(string id);
    Task<Creditor> CreateCreditorAsync(Creditor creditor);
    Task<Creditor> UpdateCreditorAsync(Creditor creditor);
    Task<bool> DeleteCreditorAsync(string id);
    Task<bool> DeleteCreditorsAsync(IEnumerable<string> ids);

    // Basic CRUD Operations - Debt Payments
    Task<IEnumerable<DebtPayment>> GetAllDebtPaymentsAsync();
    Task<DebtPayment?> GetDebtPaymentByIdAsync(string id);
    Task<DebtPayment> CreateDebtPaymentAsync(DebtPayment payment);
    Task<DebtPayment> UpdateDebtPaymentAsync(DebtPayment payment);
    Task<bool> DeleteDebtPaymentAsync(string id);

    // Debt Filtering and Search
    Task<IEnumerable<Debt>> GetActiveDebtsAsync();
    Task<IEnumerable<Debt>> GetDebtsByCreditorAsync(string creditorId);
    Task<IEnumerable<Debt>> GetDebtsByTypeAsync(DebtType debtType);
    Task<IEnumerable<Debt>> GetDebtsByPriorityAsync(DebtPriority priority);
    Task<IEnumerable<Debt>> GetOverdueDebtsAsync();
    Task<IEnumerable<Debt>> GetDebtsDueInDaysAsync(int days);
    Task<IEnumerable<Debt>> SearchDebtsAsync(string searchTerm);

    // Payment Management
    Task<IEnumerable<DebtPayment>> GetPaymentsByDebtAsync(string debtId);
    Task<IEnumerable<DebtPayment>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<DebtPayment>> GetScheduledPaymentsAsync();
    Task<IEnumerable<DebtPayment>> GetOverduePaymentsAsync();

    // Debt Calculations and Analytics
    Task<decimal> GetTotalDebtBalanceAsync();
    Task<decimal> GetTotalDebtByTypeAsync(DebtType debtType);
    Task<decimal> GetTotalDebtByCreditorAsync(string creditorId);
    Task<decimal> GetMonthlyPaymentObligationsAsync();
    Task<decimal> GetTotalInterestPaidAsync(string? debtId = null);
    Task<decimal> GetTotalPrincipalPaidAsync(string? debtId = null);

    // Payoff Calculations
    Task<DateTime?> CalculatePayoffDateAsync(string debtId, decimal monthlyPayment);
    Task<decimal> CalculateInterestSavingsAsync(string debtId, decimal extraPayment);
    Task<decimal> CalculateMinimumPaymentAsync(string debtId);
    Task<IEnumerable<(DateTime Date, decimal Balance)>> GetAmortizationScheduleAsync(string debtId, decimal monthlyPayment);

    // Debt Prioritization
    Task<IEnumerable<Debt>> GetDebtsBySnowballMethodAsync(); // Smallest balance first
    Task<IEnumerable<Debt>> GetDebtsByAvalancheMethodAsync(); // Highest interest first
    Task<IEnumerable<Debt>> GetDebtsByCustomPriorityAsync();

    // Creditor Management
    Task<IEnumerable<Debt>> GetDebtsByCreditorWithBalanceAsync(string creditorId);
    Task<bool> CanDeleteCreditorAsync(string creditorId);
    Task<IEnumerable<Creditor>> SearchCreditorsAsync(string searchTerm);

    // Reporting and Statistics
    Task<Dictionary<DebtType, decimal>> GetDebtBalancesByTypeAsync();
    Task<Dictionary<DebtPriority, decimal>> GetDebtBalancesByPriorityAsync();
    Task<Dictionary<string, decimal>> GetDebtBalancesByCreditorsAsync();
    Task<decimal> GetDebtToIncomeRatioAsync(decimal monthlyIncome);
    Task<(decimal TotalPaid, decimal InterestPaid, decimal PrincipalPaid)> GetPaymentSummaryAsync(string? debtId = null, DateTime? startDate = null, DateTime? endDate = null);

    // Validation
    Task<bool> ValidateDebtNameAsync(string name, string? excludeDebtId = null);
    Task<bool> ValidateCreditorNameAsync(string name, string? excludeCreditorId = null);
    Task<bool> HasPaymentsAsync(string debtId);
    Task<bool> HasDebtsAsync(string creditorId);

    // Advanced Payoff Calculations
    Task<PayoffProjection> CalculatePayoffProjectionAsync(string debtId, decimal monthlyPayment);
    Task<DebtStrategy> CalculateDebtStrategyAsync(DebtPriority priority, decimal extraPayment);
    Task<List<AmortizationPayment>> GetAmortizationScheduleAsync(string debtId, decimal monthlyPayment, bool fullSchedule = false);
}