using Pilgrims.PersonalFinances.Core.Models;
using Pilgrims.PersonalFinances.Core.Models.DTOs;
using Pilgrims.PersonalFinances.Core.Models.Enums;

namespace Pilgrims.PersonalFinances.Core.Services.Interfaces;

/// <summary>
/// Service interface for comprehensive obligation management with payment tracking and analytics
/// </summary>
public interface IObligationService
{
    // Basic CRUD Operations - Obligations
    Task<IEnumerable<Obligation>> GetAllObligationsAsync();
    Task<Obligation?> GetObligationByIdAsync(string id);
    Task<Obligation> CreateObligationAsync(Obligation obligation);
    Task<Obligation> UpdateObligationAsync(Obligation obligation);
    Task<bool> DeleteObligationAsync(string id);
    Task<bool> DeleteObligationsAsync(IEnumerable<string> ids);

    // Basic CRUD Operations - Obligation Payments
    Task<IEnumerable<ObligationPayment>> GetAllObligationPaymentsAsync();
    Task<ObligationPayment?> GetObligationPaymentByIdAsync(string id);
    Task<ObligationPayment> CreateObligationPaymentAsync(ObligationPayment payment);
    Task<ObligationPayment> UpdateObligationPaymentAsync(ObligationPayment payment);
    Task<bool> DeleteObligationPaymentAsync(string id);

    // Basic CRUD Operations - Obligation Benefits
    Task<IEnumerable<ObligationBenefit>> GetAllObligationBenefitsAsync();
    Task<ObligationBenefit?> GetObligationBenefitByIdAsync(string id);
    Task<ObligationBenefit> CreateObligationBenefitAsync(ObligationBenefit benefit);
    Task<ObligationBenefit> UpdateObligationBenefitAsync(ObligationBenefit benefit);
    Task<bool> DeleteObligationBenefitAsync(string id);

    // Obligation Filtering and Search
    Task<IEnumerable<Obligation>> GetActiveObligationsAsync();
    Task<IEnumerable<Obligation>> GetObligationsByTypeAsync(ObligationType obligationType);
    Task<IEnumerable<Obligation>> GetObligationsByStatusAsync(ObligationStatus status);
    Task<IEnumerable<Obligation>> GetOverdueObligationsAsync();
    Task<IEnumerable<Obligation>> GetObligationsDueInDaysAsync(int days);
    Task<IEnumerable<Obligation>> SearchObligationsAsync(string searchTerm);

    // Payment Management
    Task<IEnumerable<ObligationPayment>> GetPaymentsByObligationAsync(string obligationId);
    Task<IEnumerable<ObligationPayment>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<ObligationPayment>> GetScheduledPaymentsAsync();
    Task<IEnumerable<ObligationPayment>> GetOverduePaymentsAsync();
    Task<IEnumerable<ObligationPayment>> GetUpcomingPaymentsAsync(int days = 30);

    // Obligation Calculations and Analytics
    Task<decimal> GetTotalObligationBalanceAsync();
    Task<decimal> GetTotalObligationByTypeAsync(ObligationType obligationType);
    Task<decimal> GetMonthlyObligationPaymentsAsync();
    Task<decimal> GetTotalContributionsPaidAsync(string? obligationId = null);
    Task<decimal> GetTotalArrearsAsync();

    // Debt Management Tools
    Task<IEnumerable<Obligation>> GetObligationsBySnowballMethodAsync(); // Smallest balance first
    Task<IEnumerable<Obligation>> GetObligationsByAvalancheMethodAsync(); // Highest interest first
    Task<IEnumerable<Obligation>> GetObligationsByCustomPriorityAsync();

    // Payoff Calculations
    Task<DateTime?> CalculatePayoffDateAsync(string obligationId, decimal monthlyPayment);
    Task<decimal> CalculateInterestSavingsAsync(string obligationId, decimal extraPayment);
    Task<decimal> CalculateMinimumPaymentAsync(string obligationId);
    Task<IEnumerable<(DateTime Date, decimal Balance)>> GetAmortizationScheduleAsync(string obligationId, decimal monthlyPayment);

    // Analytics and Reporting
    Task<Dictionary<ObligationType, decimal>> GetObligationBalancesByTypeAsync();
    Task<Dictionary<ObligationStatus, decimal>> GetObligationBalancesByStatusAsync();
    Task<decimal> GetDebtToIncomeRatioAsync(decimal monthlyIncome);
    Task<(decimal TotalPaid, decimal TotalArrears, decimal TotalOutstanding)> GetPaymentSummaryAsync(string? obligationId = null, DateTime? startDate = null, DateTime? endDate = null);

    // Benefit Management
    Task<IEnumerable<ObligationBenefit>> GetBenefitsByObligationAsync(string obligationId);
    Task<decimal> GetTotalBenefitsReceivedAsync(string? obligationId = null);
    Task<decimal> GetExpectedAnnualReturnsAsync();

    // Document Management
    Task<IEnumerable<ObligationDocument>> GetDocumentsByObligationAsync(string obligationId);
    Task<ObligationDocument> CreateObligationDocumentAsync(ObligationDocument document);
    Task<bool> DeleteObligationDocumentAsync(string id);

    // Validation
    Task<bool> ValidateObligationNameAsync(string name, string? excludeObligationId = null);
    Task<bool> HasPaymentsAsync(string obligationId);
    Task<bool> HasBenefitsAsync(string obligationId);

    // Advanced Analytics
    Task<PayoffProjection> CalculatePayoffProjectionAsync(string obligationId, decimal monthlyPayment);
    Task<DebtStrategy> CalculateDebtStrategyAsync(DebtPriority priority, decimal extraPayment);
    Task<List<AmortizationPayment>> GetAmortizationScheduleAsync(string obligationId, decimal monthlyPayment, bool fullSchedule = false);
    Task<decimal> CalculateDebtAvalancheSavingsAsync();
    Task<PayoffScenario> CalculatePayoffScenarioAsync(IEnumerable<string> debtIds, string strategy, decimal extraPayment, string paymentFrequency, DateTime? targetPayoffDate);

    // Notification Integration
    Task<IEnumerable<ObligationNotification>> GetObligationNotificationsAsync(string obligationId);
    Task CreatePaymentReminderAsync(string obligationId, DateTime reminderDate);
    Task CreateOverdueNotificationAsync(string obligationId);
}
