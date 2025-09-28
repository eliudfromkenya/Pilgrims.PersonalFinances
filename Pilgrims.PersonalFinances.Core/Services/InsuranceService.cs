using Microsoft.EntityFrameworkCore;
using Pilgrims.PersonalFinances.Data;
using Pilgrims.PersonalFinances.Models;
using Pilgrims.PersonalFinances.Models.DTOs;
using Pilgrims.PersonalFinances.Models.Enums;
using Pilgrims.PersonalFinances.Services.Interfaces;

namespace Pilgrims.PersonalFinances.Services;

/// <summary>
/// Service implementation for comprehensive insurance management with policy tracking, claims, and premium management
/// </summary>
public class InsuranceService : IInsuranceService
{
    private readonly PersonalFinanceContext _context;
    private readonly INotificationService _notificationService;

    public InsuranceService(PersonalFinanceContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    #region Basic CRUD Operations - Insurance Policies

    public async Task<IEnumerable<Insurance>> GetAllInsurancePoliciesAsync()
    {
        return await _context.Insurances
            .Include(i => i.PremiumPayments)
            .Include(i => i.Claims)
            .Include(i => i.Documents)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();
    }

    public async Task<Insurance?> GetInsurancePolicyByIdAsync(string id)
    {
        return await _context.Insurances
            .Include(i => i.PremiumPayments)
            .Include(i => i.Claims)
            .Include(i => i.Documents)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<Insurance> CreateInsurancePolicyAsync(Insurance insurance)
    {
        insurance.Id = Guid.NewGuid().ToString();
        insurance.CreatedAt = DateTime.UtcNow;
        insurance.TouchUpdatedAt();

        _context.Insurances.Add(insurance);
        await _context.SaveChangesAsync();

        // Generate premium payments for the policy
        await GeneratePremiumPaymentsAsync(insurance.Id);

        // Create policy expiry notification
        await CreatePolicyExpiryNotificationAsync(insurance.Id);

        return insurance;
    }

    public async Task<Insurance> UpdateInsurancePolicyAsync(Insurance insurance)
    {
        var existing = await _context.Insurances.FindAsync(insurance.Id);
        if (existing == null)
            throw new InvalidOperationException("Insurance policy not found");

        existing.PolicyNumber = insurance.PolicyNumber;
        existing.InsuranceCompany = insurance.InsuranceCompany;
        existing.PolicyType = insurance.PolicyType;
        existing.PolicyName = insurance.PolicyName;
        existing.Description = insurance.Description;
        existing.CoverageAmount = insurance.CoverageAmount;
        existing.PremiumAmount = insurance.PremiumAmount;
        existing.PremiumFrequency = insurance.PremiumFrequency;
        existing.PolicyStartDate = insurance.PolicyStartDate;
        existing.PolicyEndDate = insurance.PolicyEndDate;
        existing.NextPremiumDueDate = insurance.NextPremiumDueDate;
        existing.DeductibleAmount = insurance.DeductibleAmount;
        existing.Status = insurance.Status;
        existing.BeneficiaryName = insurance.BeneficiaryName;
        existing.BeneficiaryRelationship = insurance.BeneficiaryRelationship;
        existing.BeneficiaryPhone = insurance.BeneficiaryPhone;
        existing.BeneficiaryEmail = insurance.BeneficiaryEmail;
        existing.Notes = insurance.Notes;
        existing.TouchUpdatedAt();

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteInsurancePolicyAsync(string id)
    {
        var insurance = await _context.Insurances.FindAsync(id);
        if (insurance == null)
            return false;

        _context.Insurances.Remove(insurance);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteInsurancePoliciesAsync(IEnumerable<string> ids)
    {
        var insurances = await _context.Insurances.Where(i => ids.Contains(i.Id)).ToListAsync();
        if (!insurances.Any())
            return false;

        _context.Insurances.RemoveRange(insurances);
        await _context.SaveChangesAsync();
        return true;
    }

    #endregion

    #region Insurance Policy Filtering and Search

    public async Task<IEnumerable<Insurance>> GetActivePoliciesAsync()
    {
        return await _context.Insurances
            .Include(i => i.PremiumPayments)
            .Include(i => i.Claims)
            .Where(i => i.Status == InsuranceStatus.Active && i.PolicyEndDate > DateTime.Now)
            .OrderBy(i => i.PolicyEndDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Insurance>> GetExpiredPoliciesAsync()
    {
        return await _context.Insurances
            .Include(i => i.PremiumPayments)
            .Include(i => i.Claims)
            .Where(i => i.Status == InsuranceStatus.Expired || i.PolicyEndDate <= DateTime.Now)
            .OrderByDescending(i => i.PolicyEndDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Insurance>> GetPoliciesByTypeAsync(string policyType)
    {
        return await _context.Insurances
            .Include(i => i.PremiumPayments)
            .Include(i => i.Claims)
            .Where(i => i.PolicyType.ToLower() == policyType.ToLower())
            .OrderBy(i => i.PolicyName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Insurance>> GetPoliciesByCompanyAsync(string company)
    {
        return await _context.Insurances
            .Include(i => i.PremiumPayments)
            .Include(i => i.Claims)
            .Where(i => i.InsuranceCompany.ToLower().Contains(company.ToLower()))
            .OrderBy(i => i.PolicyName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Insurance>> GetPoliciesExpiringInDaysAsync(int days)
    {
        var cutoffDate = DateTime.Now.AddDays(days);
        return await _context.Insurances
            .Include(i => i.PremiumPayments)
            .Include(i => i.Claims)
            .Where(i => i.Status == InsuranceStatus.Active && i.PolicyEndDate <= cutoffDate && i.PolicyEndDate > DateTime.Now)
            .OrderBy(i => i.PolicyEndDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Insurance>> SearchInsurancePoliciesAsync(string searchTerm)
    {
        var term = searchTerm.ToLower();
        return await _context.Insurances
            .Include(i => i.PremiumPayments)
            .Include(i => i.Claims)
            .Where(i => i.PolicyNumber.ToLower().Contains(term) ||
                       i.PolicyName.ToLower().Contains(term) ||
                       i.InsuranceCompany.ToLower().Contains(term) ||
                       i.PolicyType.ToLower().Contains(term) ||
                       i.Description.ToLower().Contains(term))
            .OrderBy(i => i.PolicyName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Insurance>> GetPoliciesByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Insurances
            .Include(i => i.PremiumPayments)
            .Include(i => i.Claims)
            .Where(i => i.PolicyStartDate >= startDate && i.PolicyStartDate <= endDate)
            .OrderBy(i => i.PolicyStartDate)
            .ToListAsync();
    }

    #endregion

    #region Premium Payment Operations

    public async Task<IEnumerable<InsurancePremiumPayment>> GetAllPremiumPaymentsAsync()
    {
        return await _context.InsurancePremiumPayments
            .Include(p => p.Insurance)
            .OrderByDescending(p => p.DueDate)
            .ToListAsync();
    }

    public async Task<InsurancePremiumPayment?> GetPremiumPaymentByIdAsync(string id)
    {
        return await _context.InsurancePremiumPayments
            .Include(p => p.Insurance)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<InsurancePremiumPayment>> GetPremiumPaymentsByPolicyAsync(string insuranceId)
    {
        return await _context.InsurancePremiumPayments
            .Include(p => p.Insurance)
            .Where(p => p.InsuranceId == insuranceId)
            .OrderByDescending(p => p.DueDate)
            .ToListAsync();
    }

    public async Task<InsurancePremiumPayment> CreatePremiumPaymentAsync(InsurancePremiumPayment payment)
    {
        payment.Id = Guid.NewGuid().ToString();
        payment.CreatedAt = DateTime.UtcNow;
        payment.TouchUpdatedAt();

        _context.InsurancePremiumPayments.Add(payment);
        await _context.SaveChangesAsync();
        return payment;
    }

    public async Task<InsurancePremiumPayment> UpdatePremiumPaymentAsync(InsurancePremiumPayment payment)
    {
        var existing = await _context.InsurancePremiumPayments.FindAsync(payment.Id);
        if (existing == null)
            throw new InvalidOperationException("Premium payment not found");

        existing.Amount = payment.Amount;
        existing.DueDate = payment.DueDate;
        existing.PaymentDate = payment.PaymentDate;
        existing.PaymentStatus = payment.PaymentStatus;
        existing.PaymentMethod = payment.PaymentMethod;
        existing.TransactionReference = payment.TransactionReference;
        existing.Notes = payment.Notes;
        existing.LateFee = payment.LateFee;
        existing.LateFeeAppliedDate = payment.LateFeeAppliedDate;
        existing.TouchUpdatedAt();

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeletePremiumPaymentAsync(string id)
    {
        var payment = await _context.InsurancePremiumPayments.FindAsync(id);
        if (payment == null)
            return false;

        _context.InsurancePremiumPayments.Remove(payment);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<InsurancePremiumPayment>> GetOverduePremiumPaymentsAsync()
    {
        return await _context.InsurancePremiumPayments
            .Include(p => p.Insurance)
            .Where(p => p.PaymentStatus == PaymentStatus.Pending && p.DueDate < DateTime.Now)
            .OrderBy(p => p.DueDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<InsurancePremiumPayment>> GetUpcomingPremiumPaymentsAsync(int days = 30)
    {
        var cutoffDate = DateTime.Now.AddDays(days);
        return await _context.InsurancePremiumPayments
            .Include(p => p.Insurance)
            .Where(p => p.PaymentStatus == PaymentStatus.Pending && 
                       p.DueDate >= DateTime.Now && 
                       p.DueDate <= cutoffDate)
            .OrderBy(p => p.DueDate)
            .ToListAsync();
    }

    #endregion

    #region Claims Management Operations

    public async Task<IEnumerable<InsuranceClaim>> GetAllClaimsAsync()
    {
        return await _context.InsuranceClaims
            .Include(c => c.Insurance)
            .Include(c => c.Documents)
            .OrderByDescending(c => c.ClaimDate)
            .ToListAsync();
    }

    public async Task<InsuranceClaim?> GetClaimByIdAsync(string id)
    {
        return await _context.InsuranceClaims
            .Include(c => c.Insurance)
            .Include(c => c.Documents)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<InsuranceClaim>> GetClaimsByPolicyAsync(string insuranceId)
    {
        return await _context.InsuranceClaims
            .Include(c => c.Insurance)
            .Include(c => c.Documents)
            .Where(c => c.InsuranceId == insuranceId)
            .OrderByDescending(c => c.ClaimDate)
            .ToListAsync();
    }

    public async Task<InsuranceClaim> CreateClaimAsync(InsuranceClaim claim)
    {
        claim.Id = Guid.NewGuid().ToString();
        claim.CreatedAt = DateTime.UtcNow;
        claim.TouchUpdatedAt();

        _context.InsuranceClaims.Add(claim);
        await _context.SaveChangesAsync();

        // Create claim notification
        await CreateClaimUpdateNotificationAsync(claim.Id);

        return claim;
    }

    public async Task<InsuranceClaim> UpdateClaimAsync(InsuranceClaim claim)
    {
        var existing = await _context.InsuranceClaims.FindAsync(claim.Id);
        if (existing == null)
            throw new InvalidOperationException("Insurance claim not found");

        var statusChanged = existing.Status != claim.Status;

        existing.ClaimNumber = claim.ClaimNumber;
        existing.ClaimDate = claim.ClaimDate;
        existing.IncidentDate = claim.IncidentDate;
        existing.Description = claim.Description;
        existing.ClaimAmount = claim.ClaimAmount;
        existing.ApprovedAmount = claim.ApprovedAmount;
        existing.PaidAmount = claim.PaidAmount;
        existing.Status = claim.Status;
        existing.SettlementDate = claim.SettlementDate;
        existing.Notes = claim.Notes;
        existing.TouchUpdatedAt();

        await _context.SaveChangesAsync();

        // Create notification if status changed
        if (statusChanged)
        {
            await CreateClaimUpdateNotificationAsync(claim.Id);
        }

        return existing;
    }

    public async Task<bool> DeleteClaimAsync(string id)
    {
        var claim = await _context.InsuranceClaims.FindAsync(id);
        if (claim == null)
            return false;

        _context.InsuranceClaims.Remove(claim);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<InsuranceClaim>> GetClaimsByStatusAsync(ClaimStatus status)
    {
        return await _context.InsuranceClaims
            .Include(c => c.Insurance)
            .Include(c => c.Documents)
            .Where(c => c.Status == status)
            .OrderByDescending(c => c.ClaimDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<InsuranceClaim>> GetClaimsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.InsuranceClaims
            .Include(c => c.Insurance)
            .Include(c => c.Documents)
            .Where(c => c.ClaimDate >= startDate && c.ClaimDate <= endDate)
            .OrderByDescending(c => c.ClaimDate)
            .ToListAsync();
    }

    #endregion

    #region Document Management Operations

    public async Task<IEnumerable<InsuranceDocument>> GetDocumentsByPolicyAsync(string insuranceId)
    {
        return await _context.InsuranceDocuments
            .Include(d => d.Insurance)
            .Include(d => d.Claim)
            .Where(d => d.InsuranceId == insuranceId)
            .OrderByDescending(d => d.UploadDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<InsuranceDocument>> GetDocumentsByClaimAsync(string claimId)
    {
        return await _context.InsuranceDocuments
            .Include(d => d.Insurance)
            .Include(d => d.Claim)
            .Where(d => d.ClaimId == claimId)
            .OrderByDescending(d => d.UploadDate)
            .ToListAsync();
    }

    public async Task<InsuranceDocument> CreateDocumentAsync(InsuranceDocument document)
    {
        document.Id = Guid.NewGuid().ToString();
        document.CreatedAt = DateTime.UtcNow;
        document.TouchUpdatedAt();
        document.UploadDate = DateTime.UtcNow;

        _context.InsuranceDocuments.Add(document);
        await _context.SaveChangesAsync();
        return document;
    }

    public async Task<InsuranceDocument> UpdateDocumentAsync(InsuranceDocument document)
    {
        var existing = await _context.InsuranceDocuments.FindAsync(document.Id);
        if (existing == null)
            throw new InvalidOperationException("Insurance document not found");

        existing.DocumentName = document.DocumentName;
        existing.DocumentType = document.DocumentType;
        existing.FilePath = document.FilePath;
        existing.FileType = document.FileType;
        existing.FileSize = document.FileSize;
        existing.Description = document.Description;
        existing.TouchUpdatedAt();

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteDocumentAsync(string id)
    {
        var document = await _context.InsuranceDocuments.FindAsync(id);
        if (document == null)
            return false;

        _context.InsuranceDocuments.Remove(document);
        await _context.SaveChangesAsync();
        return true;
    }

    #endregion

    #region Analytics and Reporting

    public async Task<decimal?> GetTotalCoverageAmountAsync()
    {
        return await _context.Insurances
            .Where(i => i.Status == InsuranceStatus.Active)
            .SumAsync(i => i.CoverageAmount);
    }

    public async Task<decimal> GetTotalAnnualPremiumsAsync()
    {
        return await _context.Insurances
            .Where(i => i.Status == InsuranceStatus.Active)
            .SumAsync(i => i.AnnualPremium);
    }

    public async Task<decimal> GetTotalMonthlyPremiumsAsync()
    {
        var totalAnnual = await GetTotalAnnualPremiumsAsync();
        return totalAnnual / 12;
    }

    public async Task<int> GetActivePolicyCountAsync()
    {
        return await _context.Insurances
            .CountAsync(i => i.Status == InsuranceStatus.Active && i.PolicyEndDate > DateTime.Now);
    }

    public async Task<decimal> GetTotalClaimsAmountAsync()
    {
        return await _context.InsuranceClaims
            .SumAsync(c => c.ClaimAmount);
    }

    public async Task<decimal> GetTotalPaidClaimsAmountAsync()
    {
        return await _context.InsuranceClaims
            .Where(c => c.Status == ClaimStatus.Settled && c.PaidAmount.HasValue)
            .SumAsync(c => c.PaidAmount.Value);
    }

    public async Task<IEnumerable<Insurance>> GetPoliciesRequiringRenewalAsync(int daysAhead = 30)
    {
        var cutoffDate = DateTime.Now.AddDays(daysAhead);
        return await _context.Insurances
            .Include(i => i.PremiumPayments)
            .Include(i => i.Claims)
            .Where(i => i.Status == InsuranceStatus.Active && 
                       i.PolicyEndDate <= cutoffDate && 
                       i.PolicyEndDate > DateTime.Now)
            .OrderBy(i => i.PolicyEndDate)
            .ToListAsync();
    }

    #endregion

    #region Premium Calculation and Scheduling

    public async Task GeneratePremiumPaymentsAsync(string insuranceId)
    {
        var insurance = await _context.Insurances.FindAsync(insuranceId);
        if (insurance == null)
            return;

        // Remove existing future payments
        var existingPayments = await _context.InsurancePremiumPayments
            .Where(p => p.InsuranceId == insuranceId && p.DueDate > DateTime.Now && p.PaymentStatus == PaymentStatus.Pending)
            .ToListAsync();
        
        _context.InsurancePremiumPayments.RemoveRange(existingPayments);

        // Generate new payments based on frequency
        var payments = new List<InsurancePremiumPayment>();
        var currentDate = insurance.NextPremiumDueDate;
        var endDate = insurance.PolicyEndDate;

        while (currentDate <= endDate)
        {
            var payment = new InsurancePremiumPayment
            {
                Id = Guid.NewGuid().ToString(),
                InsuranceId = insuranceId,
                Amount = insurance.PremiumAmount,
                DueDate = currentDate,
                PaymentStatus = PaymentStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };
            payment.TouchUpdatedAt();

            payments.Add(payment);

            // Calculate next payment date based on frequency
            currentDate = insurance.PremiumFrequency switch
            {
                PaymentFrequency.Monthly => currentDate?.AddMonths(1),
                PaymentFrequency.Quarterly => currentDate?.AddMonths(3),
                PaymentFrequency.SemiAnnual => currentDate?.AddMonths(6),
                PaymentFrequency.Annual => currentDate?.AddYears(1),
                _ => currentDate?.AddMonths(1)
            };
        }

        _context.InsurancePremiumPayments.AddRange(payments);
        await _context.SaveChangesAsync();
    }

    public async Task<decimal> CalculateMonthlyPremiumAsync(string insuranceId)
    {
        var insurance = await _context.Insurances.FindAsync(insuranceId);
        if (insurance == null)
            return 0;

        return insurance.PremiumFrequency switch
        {
            PaymentFrequency.Monthly => insurance.PremiumAmount,
            PaymentFrequency.Quarterly => insurance.PremiumAmount / 3,
            PaymentFrequency.SemiAnnual => insurance.PremiumAmount / 6,
            PaymentFrequency.Annual => insurance.PremiumAmount / 12,
            _ => insurance.PremiumAmount
        };
    }

    public async Task<decimal> CalculateAnnualPremiumAsync(string insuranceId)
    {
        var insurance = await _context.Insurances.FindAsync(insuranceId);
        if (insurance == null)
            return 0;

        return insurance.AnnualPremium;
    }

    #endregion

    #region Notification Management

    public async Task CreatePremiumDueNotificationsAsync()
    {
        var upcomingPayments = await GetUpcomingPremiumPaymentsAsync(7);
        
        foreach (var payment in upcomingPayments)
        {
            var dueDate = payment.DueDate?.AddDays(-7).Date;

            var existingNotification = await _context.InsuranceNotifications
                .FirstOrDefaultAsync(n => n.InsuranceId == payment.InsuranceId && 
                                         n.ScheduledDate.Date == dueDate &&
                                         !n.IsSent);

            if (existingNotification == null)
            {
                if(payment.DueDate == null)
                    throw new Exception("Due Date is null for payment, It's required to create premium due notification");
                var notification = InsuranceNotification.CreatePremiumDueNotification(payment.Insurance, payment.DueDate.Value);
                _context.InsuranceNotifications.Add(notification);
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task CreatePolicyExpiryNotificationsAsync()
    {
        var expiringPolicies = await GetPoliciesExpiringInDaysAsync(30);
        
        foreach (var policy in expiringPolicies)
        {
            var dueDate = policy.PolicyEndDate?.AddDays(-30).Date;

            var existingNotification = await _context.InsuranceNotifications
                .FirstOrDefaultAsync(n => n.InsuranceId == policy.Id && 
                                         n.ScheduledDate.Date == dueDate &&
                                         !n.IsSent);

            if (existingNotification == null)
            {
                var notification = InsuranceNotification.CreatePolicyExpiryNotification(policy);
                _context.InsuranceNotifications.Add(notification);
            }
        }

        await _context.SaveChangesAsync();
    }

    private async Task CreatePolicyExpiryNotificationAsync(string insuranceId)
    {
        var insurance = await _context.Insurances.FindAsync(insuranceId);
        if (insurance == null)
            return;

        var notification = InsuranceNotification.CreatePolicyExpiryNotification(insurance);
        _context.InsuranceNotifications.Add(notification);
        await _context.SaveChangesAsync();
    }

    public async Task CreateClaimUpdateNotificationAsync(string claimId)
    {
        var claim = await _context.InsuranceClaims
            .Include(c => c.Insurance)
            .FirstOrDefaultAsync(c => c.Id == claimId);
        
        if (claim == null)
            return;

        var notification = InsuranceNotification.CreateClaimUpdateNotification(claim.Insurance, claim);
        _context.InsuranceNotifications.Add(notification);
        await _context.SaveChangesAsync();
    }

    #endregion

    #region Dashboard Data

    public async Task<object> GetInsuranceDashboardDataAsync()
    {
        var totalCoverage = await GetTotalCoverageAmountAsync();
        var totalAnnualPremiums = await GetTotalAnnualPremiumsAsync();
        var totalMonthlyPremiums = await GetTotalMonthlyPremiumsAsync();
        var activePolicyCount = await GetActivePolicyCountAsync();
        var totalClaimsAmount = await GetTotalClaimsAmountAsync();
        var totalPaidClaimsAmount = await GetTotalPaidClaimsAmountAsync();
        var overduePayments = await GetOverduePremiumPaymentsAsync();
        var upcomingPayments = await GetUpcomingPremiumPaymentsAsync(30);
        var expiringPolicies = await GetPoliciesExpiringInDaysAsync(30);

        return new
        {
            TotalCoverage = totalCoverage,
            TotalAnnualPremiums = totalAnnualPremiums,
            TotalMonthlyPremiums = totalMonthlyPremiums,
            ActivePolicyCount = activePolicyCount,
            TotalClaimsAmount = totalClaimsAmount,
            TotalPaidClaimsAmount = totalPaidClaimsAmount,
            OverduePaymentsCount = overduePayments.Count(),
            UpcomingPaymentsCount = upcomingPayments.Count(),
            ExpiringPoliciesCount = expiringPolicies.Count(),
            OverduePayments = overduePayments.Take(5),
            UpcomingPayments = upcomingPayments.Take(5),
            ExpiringPolicies = expiringPolicies.Take(5)
        };
    }

    public async Task<object> GetInsuranceAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        startDate ??= DateTime.Now.AddYears(-1);
        endDate ??= DateTime.Now;

        var policies = await _context.Insurances
            .Include(i => i.PremiumPayments)
            .Include(i => i.Claims)
            .Where(i => i.CreatedAt >= startDate && i.CreatedAt <= endDate)
            .ToListAsync();

        var claims = await _context.InsuranceClaims
            .Include(c => c.Insurance)
            .Where(c => c.ClaimDate >= startDate && c.ClaimDate <= endDate)
            .ToListAsync();

        var premiumPayments = await _context.InsurancePremiumPayments
            .Include(p => p.Insurance)
            .Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate && p.PaymentStatus == PaymentStatus.Completed)
            .ToListAsync();

        return new
        {
            PoliciesByType = policies.GroupBy(p => p.PolicyType)
                .Select(g => new { Type = g.Key, Count = g.Count(), TotalCoverage = g.Sum(p => p.CoverageAmount) }),
            
            ClaimsByStatus = claims.GroupBy(c => c.Status)
                .Select(g => new { Status = g.Key.ToString(), Count = g.Count(), TotalAmount = g.Sum(c => c.ClaimAmount) }),
            
            MonthlyPremiumPayments = premiumPayments.GroupBy(p => new { p.PaymentDate.Value.Year, p.PaymentDate.Value.Month })
                .Select(g => new { Year = g.Key.Year, Month = g.Key.Month, TotalAmount = g.Sum(p => p.Amount) })
                .OrderBy(x => x.Year).ThenBy(x => x.Month),
            
            ClaimsSuccessRate = claims.Any() ? 
                (decimal)claims.Count(c => c.Status == ClaimStatus.Settled) / claims.Count() * 100 : 0,
            
            AveragePremiumByType = policies.GroupBy(p => p.PolicyType)
                .Select(g => new { Type = g.Key, AveragePremium = g.Average(p => p.AnnualPremium) })
        };
    }

    #endregion
}