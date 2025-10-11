using Pilgrims.PersonalFinances.Models;
using Pilgrims.PersonalFinances.Models.DTOs;
using Pilgrims.PersonalFinances.Models.Enums;

namespace Pilgrims.PersonalFinances.Services.Interfaces;

/// <summary>
/// Service interface for comprehensive insurance management with policy tracking, claims, and premium management
/// </summary>
public interface IInsuranceService
{
    // Basic CRUD Operations - Insurance Policies
    Task<IEnumerable<Insurance>> GetAllInsurancePoliciesAsync();
    Task<Insurance?> GetInsurancePolicyByIdAsync(string id);
    Task<Insurance> CreateInsurancePolicyAsync(Insurance insurance);
    Task<Insurance> UpdateInsurancePolicyAsync(Insurance insurance);
    Task<bool> DeleteInsurancePolicyAsync(string id);
    Task<bool> DeleteInsurancePoliciesAsync(IEnumerable<string> ids);

    // Insurance Policy Filtering and Search
    Task<IEnumerable<Insurance>> GetActivePoliciesAsync();
    Task<IEnumerable<Insurance>> GetExpiredPoliciesAsync();
    Task<IEnumerable<Insurance>> GetPoliciesByTypeAsync(string policyType);
    Task<IEnumerable<Insurance>> GetPoliciesByCompanyAsync(string company);
    Task<IEnumerable<Insurance>> GetPoliciesExpiringInDaysAsync(int days);
    Task<IEnumerable<Insurance>> SearchInsurancePoliciesAsync(string searchTerm);
    Task<IEnumerable<Insurance>> GetPoliciesByDateRangeAsync(DateTime startDate, DateTime endDate);

    // Premium Payment Operations
    Task<IEnumerable<InsurancePremiumPayment>> GetAllPremiumPaymentsAsync();
    Task<InsurancePremiumPayment?> GetPremiumPaymentByIdAsync(string id);
    Task<IEnumerable<InsurancePremiumPayment>> GetPremiumPaymentsByPolicyAsync(string insuranceId);
    Task<InsurancePremiumPayment> CreatePremiumPaymentAsync(InsurancePremiumPayment payment);
    Task<InsurancePremiumPayment> UpdatePremiumPaymentAsync(InsurancePremiumPayment payment);
    Task<bool> DeletePremiumPaymentAsync(string id);
    Task<IEnumerable<InsurancePremiumPayment>> GetOverduePremiumPaymentsAsync();
    Task<IEnumerable<InsurancePremiumPayment>> GetUpcomingPremiumPaymentsAsync(int days = 30);

    // Claims Management Operations
    Task<IEnumerable<InsuranceClaim>> GetAllClaimsAsync();
    Task<InsuranceClaim?> GetClaimByIdAsync(string id);
    Task<IEnumerable<InsuranceClaim>> GetClaimsByPolicyAsync(string insuranceId);
    Task<InsuranceClaim> CreateClaimAsync(InsuranceClaim claim);
    Task<InsuranceClaim> UpdateClaimAsync(InsuranceClaim claim);
    Task<bool> DeleteClaimAsync(string id);
    Task<IEnumerable<InsuranceClaim>> GetClaimsByStatusAsync(ClaimStatus status);
    Task<IEnumerable<InsuranceClaim>> GetClaimsByDateRangeAsync(DateTime startDate, DateTime endDate);

    // Document Management Operations
    Task<IEnumerable<InsuranceDocument>> GetDocumentsByPolicyAsync(string insuranceId);
    Task<IEnumerable<InsuranceDocument>> GetDocumentsByClaimAsync(string claimId);
    Task<InsuranceDocument> CreateDocumentAsync(InsuranceDocument document);
    Task<InsuranceDocument> UpdateDocumentAsync(InsuranceDocument document);
    Task<bool> DeleteDocumentAsync(string id);

    // Analytics and Reporting
    Task<decimal?> GetTotalCoverageAmountAsync();
    Task<decimal> GetTotalAnnualPremiumsAsync();
    Task<decimal> GetTotalMonthlyPremiumsAsync();
    Task<int> GetActivePolicyCountAsync();
    Task<decimal> GetTotalClaimsAmountAsync();
    Task<decimal> GetTotalPaidClaimsAmountAsync();
    Task<IEnumerable<Insurance>> GetPoliciesRequiringRenewalAsync(int daysAhead = 30);

    // Premium Calculation and Scheduling
    Task GeneratePremiumPaymentsAsync(string insuranceId);
    Task<decimal> CalculateMonthlyPremiumAsync(string insuranceId);
    Task<decimal> CalculateAnnualPremiumAsync(string insuranceId);

    // Notification Management
    Task CreatePremiumDueNotificationsAsync();
    Task CreatePolicyExpiryNotificationsAsync();
    Task CreateClaimUpdateNotificationAsync(string claimId);

    // Dashboard Data
    Task<object> GetInsuranceDashboardDataAsync();
    Task<object> GetInsuranceAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null);
}