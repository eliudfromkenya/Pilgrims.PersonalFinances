using Microsoft.EntityFrameworkCore;
using Pilgrims.PersonalFinances.Data;
using Pilgrims.PersonalFinances.Models;
using Pilgrims.PersonalFinances.Models.DTOs;
using Pilgrims.PersonalFinances.Models.Enums;
using Pilgrims.PersonalFinances.Services.Interfaces;

namespace Pilgrims.PersonalFinances.Services;

/// <summary>
/// Service implementation for comprehensive obligation management with payment tracking and analytics
/// </summary>
public class ObligationService : IObligationService
{
    private readonly PersonalFinanceContext _context;
    private readonly INotificationService _notificationService;

    public ObligationService(PersonalFinanceContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    #region Basic CRUD Operations - Obligations

    public async Task<IEnumerable<Obligation>> GetAllObligationsAsync()
    {
        return await _context.Obligations
            .Include(o => o.Payments)
            .Include(o => o.Benefits)
            .Include(o => o.Documents)
            .OrderBy(o => o.Name)
            .ToListAsync();
    }

    public async Task<Obligation?> GetObligationByIdAsync(string id)
    {
        return await _context.Obligations
            .Include(o => o.Payments)
            .Include(o => o.Benefits)
            .Include(o => o.Documents)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<Obligation> CreateObligationAsync(Obligation obligation)
    {
        obligation.Id = Guid.NewGuid().ToString();
        obligation.CreatedAt = DateTime.UtcNow;
        obligation.TouchUpdatedAt();

        _context.Obligations.Add(obligation);
        await _context.SaveChangesAsync();

        // Create initial notification for new obligation
        await CreatePaymentReminderAsync(obligation.Id, obligation.NextContributionDue ?? DateTime.Now.AddDays(30));

        return obligation;
    }

    public async Task<Obligation> UpdateObligationAsync(Obligation obligation)
    {
        obligation.TouchUpdatedAt();
        _context.Obligations.Update(obligation);
        await _context.SaveChangesAsync();
        return obligation;
    }

    public async Task<bool> DeleteObligationAsync(string id)
    {
        var obligation = await _context.Obligations.FindAsync(id);
        if (obligation == null) return false;

        // Check if obligation has payments or benefits
        var hasPayments = await _context.ObligationPayments.AnyAsync(p => p.ObligationId == id);
        var hasBenefits = await _context.ObligationBenefits.AnyAsync(b => b.ObligationId == id);
        
        if (hasPayments || hasBenefits)
        {
            // Soft delete by marking as inactive
            obligation.Status = ObligationStatus.Inactive;
            _context.Obligations.Update(obligation);
        }
        else
        {
            // Hard delete if no payments or benefits
            _context.Obligations.Remove(obligation);
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteObligationsAsync(IEnumerable<string> ids)
    {
        foreach (var id in ids)
        {
            await DeleteObligationAsync(id);
        }
        return true;
    }

    #endregion

    #region Basic CRUD Operations - Obligation Payments

    public async Task<IEnumerable<ObligationPayment>> GetAllObligationPaymentsAsync()
    {
        return await _context.ObligationPayments
            .Include(p => p.Obligation)
            .OrderByDescending(p => p.DueDate)
            .ToListAsync();
    }

    public async Task<ObligationPayment?> GetObligationPaymentByIdAsync(string id)
    {
        return await _context.ObligationPayments
            .Include(p => p.Obligation)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<ObligationPayment> CreateObligationPaymentAsync(ObligationPayment payment)
    {
        payment.Id = Guid.NewGuid().ToString();
        payment.CreatedAt = DateTime.UtcNow;
        payment.TouchUpdatedAt();

        _context.ObligationPayments.Add(payment);
        await _context.SaveChangesAsync();

        // Update obligation balance if payment is completed
        if (payment.PaymentStatus == PaymentStatus.Completed)
        {
            await UpdateObligationBalanceAsync(payment.ObligationId);
        }

        return payment;
    }

    public async Task<ObligationPayment> UpdateObligationPaymentAsync(ObligationPayment payment)
    {
        payment.TouchUpdatedAt();
        _context.ObligationPayments.Update(payment);
        await _context.SaveChangesAsync();

        // Update obligation balance
        await UpdateObligationBalanceAsync(payment.ObligationId);

        return payment;
    }

    public async Task<bool> DeleteObligationPaymentAsync(string id)
    {
        var payment = await _context.ObligationPayments.FindAsync(id);
        if (payment == null) return false;

        var obligationId = payment.ObligationId;
        _context.ObligationPayments.Remove(payment);
        await _context.SaveChangesAsync();

        // Update obligation balance
        await UpdateObligationBalanceAsync(obligationId);

        return true;
    }

    #endregion

    #region Basic CRUD Operations - Obligation Benefits

    public async Task<IEnumerable<ObligationBenefit>> GetAllObligationBenefitsAsync()
    {
        return await _context.ObligationBenefits
            .Include(b => b.Obligation)
            .OrderByDescending(b => b.ReceivedDate)
            .ToListAsync();
    }

    public async Task<ObligationBenefit?> GetObligationBenefitByIdAsync(string id)
    {
        return await _context.ObligationBenefits
            .Include(b => b.Obligation)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<ObligationBenefit> CreateObligationBenefitAsync(ObligationBenefit benefit)
    {
        benefit.Id = Guid.NewGuid().ToString();
        benefit.CreatedAt = DateTime.UtcNow;
        benefit.TouchUpdatedAt();

        _context.ObligationBenefits.Add(benefit);
        await _context.SaveChangesAsync();

        return benefit;
    }

    public async Task<ObligationBenefit> UpdateObligationBenefitAsync(ObligationBenefit benefit)
    {
        benefit.TouchUpdatedAt();
        _context.ObligationBenefits.Update(benefit);
        await _context.SaveChangesAsync();
        return benefit;
    }

    public async Task<bool> DeleteObligationBenefitAsync(string id)
    {
        var benefit = await _context.ObligationBenefits.FindAsync(id);
        if (benefit == null) return false;

        _context.ObligationBenefits.Remove(benefit);
        await _context.SaveChangesAsync();
        return true;
    }

    #endregion

    #region Obligation Filtering and Search

    public async Task<IEnumerable<Obligation>> GetActiveObligationsAsync()
    {
        return await _context.Obligations
            .Include(o => o.Payments)
            .Include(o => o.Benefits)
            .Where(o => o.Status == ObligationStatus.Active)
            .OrderBy(o => o.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Obligation>> GetObligationsByTypeAsync(ObligationType obligationType)
    {
        return await _context.Obligations
            .Include(o => o.Payments)
            .Include(o => o.Benefits)
            .Where(o => o.ObligationType == obligationType && o.Status == ObligationStatus.Active)
            .OrderBy(o => o.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Obligation>> GetObligationsByStatusAsync(ObligationStatus status)
    {
        return await _context.Obligations
            .Include(o => o.Payments)
            .Include(o => o.Benefits)
            .Where(o => o.Status == status)
            .OrderBy(o => o.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Obligation>> GetOverdueObligationsAsync()
    {
        var today = DateTime.Today;
        return await _context.Obligations
            .Include(o => o.Payments)
            .Include(o => o.Benefits)
            .Where(o => o.Status == ObligationStatus.Active && o.NextContributionDue < today)
            .OrderBy(o => o.NextContributionDue)
            .ToListAsync();
    }

    public async Task<IEnumerable<Obligation>> GetObligationsDueInDaysAsync(int days)
    {
        var targetDate = DateTime.Today.AddDays(days);
        return await _context.Obligations
            .Include(o => o.Payments)
            .Include(o => o.Benefits)
            .Where(o => o.Status == ObligationStatus.Active && 
                       o.NextContributionDue <= targetDate && 
                       o.NextContributionDue >= DateTime.Today)
            .OrderBy(o => o.NextContributionDue)
            .ToListAsync();
    }

    public async Task<IEnumerable<Obligation>> SearchObligationsAsync(string searchTerm)
    {
        return await _context.Obligations
            .Include(o => o.Payments)
            .Include(o => o.Benefits)
            .Where(o => o.Name.Contains(searchTerm) || 
                       o.Organization.Contains(searchTerm) ||
                       (o.Description != null && o.Description.Contains(searchTerm)) ||
                       (o.Notes != null && o.Notes.Contains(searchTerm)))
            .OrderBy(o => o.Name)
            .ToListAsync();
    }

    #endregion

    #region Payment Management

    public async Task<IEnumerable<ObligationPayment>> GetPaymentsByObligationAsync(string obligationId)
    {
        return await _context.ObligationPayments
            .Include(p => p.Obligation)
            .Where(p => p.ObligationId == obligationId)
            .OrderByDescending(p => p.DueDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<ObligationPayment>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.ObligationPayments
            .Include(p => p.Obligation)
            .Where(p => p.DueDate >= startDate && p.DueDate <= endDate)
            .OrderBy(p => p.DueDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<ObligationPayment>> GetScheduledPaymentsAsync()
    {
        return await _context.ObligationPayments
            .Include(p => p.Obligation)
            .Where(p => p.PaymentStatus == PaymentStatus.Pending)
            .OrderBy(p => p.DueDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<ObligationPayment>> GetOverduePaymentsAsync()
    {
        var today = DateTime.Today;
        return await _context.ObligationPayments
            .Include(p => p.Obligation)
            .Where(p => p.PaymentStatus == PaymentStatus.Pending && p.DueDate < today)
            .OrderBy(p => p.DueDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<ObligationPayment>> GetUpcomingPaymentsAsync(int days = 30)
    {
        var targetDate = DateTime.Today.AddDays(days);
        return await _context.ObligationPayments
            .Include(p => p.Obligation)
            .Where(p => p.PaymentStatus == PaymentStatus.Pending && 
                       p.DueDate <= targetDate && 
                       p.DueDate >= DateTime.Today)
            .OrderBy(p => p.DueDate)
            .ToListAsync();
    }

    #endregion

    #region Obligation Calculations and Analytics

    public async Task<decimal> GetTotalObligationBalanceAsync()
    {
        return await _context.Obligations
            .Where(o => o.Status == ObligationStatus.Active)
            .SumAsync(o => o.CurrentBalance ?? 0);
    }

    public async Task<decimal> GetTotalObligationByTypeAsync(ObligationType obligationType)
    {
        return await _context.Obligations
            .Where(o => o.Status == ObligationStatus.Active && o.ObligationType == obligationType)
            .SumAsync(o => o.CurrentBalance ?? 0);
    }

    public async Task<decimal> GetMonthlyObligationPaymentsAsync()
    {
        return await _context.Obligations
            .Where(o => o.Status == ObligationStatus.Active)
            .SumAsync(o => o.MonthlyContribution);
    }

    public async Task<decimal> GetTotalContributionsPaidAsync(string? obligationId = null)
    {
        var query = _context.ObligationPayments.AsQueryable();
        
        if (!string.IsNullOrEmpty(obligationId))
            query = query.Where(p => p.ObligationId == obligationId);

        return await query
            .Where(p => p.PaymentStatus == PaymentStatus.Completed)
            .SumAsync(p => p.Amount);
    }

    public async Task<decimal> GetTotalArrearsAsync()
    {
        return await _context.ObligationPayments
            .Where(p => p.PaymentStatus == PaymentStatus.Overdue)
            .SumAsync(p => p.TotalAmountDue);
    }

    #endregion

    #region Debt Management Tools

    public async Task<IEnumerable<Obligation>> GetObligationsBySnowballMethodAsync()
    {
        return await _context.Obligations
            .Include(o => o.Payments)
            .Include(o => o.Benefits)
            .Where(o => o.Status == ObligationStatus.Active && (o.CurrentBalance ?? 0) > 0)
            .OrderBy(o => o.CurrentBalance ?? 0) // Smallest balance first
            .ToListAsync();
    }

    public async Task<IEnumerable<Obligation>> GetObligationsByAvalancheMethodAsync()
    {
        return await _context.Obligations
            .Include(o => o.Payments)
            .Include(o => o.Benefits)
            .Where(o => o.Status == ObligationStatus.Active && (o.CurrentBalance ?? 0) > 0)
            .OrderByDescending(o => o.InterestRate ?? 0) // Highest interest first
            .ToListAsync();
    }

    public async Task<IEnumerable<Obligation>> GetObligationsByCustomPriorityAsync()
    {
        return await _context.Obligations
            .Include(o => o.Payments)
            .Include(o => o.Benefits)
            .Where(o => o.Status == ObligationStatus.Active && (o.CurrentBalance ?? 0) > 0)
            .OrderBy(o => o.Priority)
            .ThenByDescending(o => o.InterestRate ?? 0)
            .ToListAsync();
    }

    #endregion

    #region Payoff Calculations

    public async Task<DateTime?> CalculatePayoffDateAsync(string obligationId, decimal monthlyPayment)
    {
        var obligation = await GetObligationByIdAsync(obligationId);
        if (obligation == null || (obligation.CurrentBalance ?? 0) <= 0)
            return null;

        var currentBalance = obligation.CurrentBalance ?? 0m;
        var interestRate = (obligation.InterestRate ?? 0) / 100m / 12m;
        var monthsToPayoff = 0;
        var balance = currentBalance;

        while (balance > 0 && monthsToPayoff < 600) // Max 50 years
        {
            var interestPayment = balance * interestRate;
            var principalPayment = Math.Min(monthlyPayment - interestPayment, balance);
            
            if (principalPayment <= 0)
                return null; // Payment insufficient to cover interest

            balance -= principalPayment;
            monthsToPayoff++;
        }

        return DateTime.Now.AddMonths(monthsToPayoff);
    }

    public async Task<decimal> CalculateInterestSavingsAsync(string obligationId, decimal extraPayment)
    {
        var obligation = await GetObligationByIdAsync(obligationId);
        if (obligation == null) return 0;

        var currentBalance = obligation.CurrentBalance ?? 0m;
        var monthlyPayment = obligation.MonthlyContribution;
        var interestRate = (obligation.InterestRate ?? 0) / 100m / 12m;

        // Calculate interest with current payment
        var totalInterestCurrent = CalculateTotalInterest(currentBalance, monthlyPayment, interestRate);
        
        // Calculate interest with extra payment
        var totalInterestWithExtra = CalculateTotalInterest(currentBalance, monthlyPayment + extraPayment, interestRate);

        return totalInterestCurrent - totalInterestWithExtra;
    }

    public async Task<decimal> CalculateMinimumPaymentAsync(string obligationId)
    {
        var obligation = await GetObligationByIdAsync(obligationId);
        if (obligation == null) return 0;

        // For obligations with interest, calculate minimum payment to cover interest + small principal
        if ((obligation.InterestRate ?? 0) > 0)
        {
            var monthlyInterest = (obligation.CurrentBalance ?? 0) * (obligation.InterestRate ?? 0) / 100m / 12m;
            return monthlyInterest + (obligation.CurrentBalance ?? 0) * 0.01m; // 1% of balance as principal
        }

        return obligation.MonthlyContribution;
    }

    public async Task<IEnumerable<(DateTime Date, decimal Balance)>> GetAmortizationScheduleAsync(string obligationId, decimal monthlyPayment)
    {
        var obligation = await GetObligationByIdAsync(obligationId);
        if (obligation == null) return new List<(DateTime, decimal)>();

        var schedule = new List<(DateTime Date, decimal Balance)>();
        var currentBalance = obligation.CurrentBalance ?? 0m;
        var interestRate = (obligation.InterestRate ?? 0) / 100m / 12m;
        var currentDate = DateTime.Now;

        while (currentBalance > 0 && schedule.Count < 600) // Max 50 years
        {
            var interestPayment = currentBalance * interestRate;
            var principalPayment = Math.Min(monthlyPayment - interestPayment, currentBalance);
            
            if (principalPayment <= 0) break;

            currentBalance -= principalPayment;
            currentDate = currentDate.AddMonths(1);
            schedule.Add((currentDate, currentBalance));
        }

        return schedule;
    }

    #endregion

    #region Analytics and Reporting

    public async Task<Dictionary<ObligationType, decimal>> GetObligationBalancesByTypeAsync()
    {
        return await _context.Obligations
            .Where(o => o.Status == ObligationStatus.Active)
            .GroupBy(o => o.ObligationType)
            .ToDictionaryAsync(g => g.Key, g => g.Sum(o => o.CurrentBalance ?? 0));
    }

    public async Task<Dictionary<ObligationStatus, decimal>> GetObligationBalancesByStatusAsync()
    {
        return await _context.Obligations
            .GroupBy(o => o.Status)
            .ToDictionaryAsync(g => g.Key, g => g.Sum(o => o.CurrentBalance ?? 0));
    }

    public async Task<decimal> GetDebtToIncomeRatioAsync(decimal monthlyIncome)
    {
        if (monthlyIncome <= 0) return 0;

        var monthlyObligations = await GetMonthlyObligationPaymentsAsync();
        return monthlyObligations / monthlyIncome;
    }

    public async Task<(decimal TotalPaid, decimal TotalArrears, decimal TotalOutstanding)> GetPaymentSummaryAsync(string? obligationId = null, DateTime? startDate = null, DateTime? endDate = null)
    {
        var paymentsQuery = _context.ObligationPayments.AsQueryable();
        
        if (!string.IsNullOrEmpty(obligationId))
            paymentsQuery = paymentsQuery.Where(p => p.ObligationId == obligationId);
        
        if (startDate.HasValue)
            paymentsQuery = paymentsQuery.Where(p => p.DueDate >= startDate.Value);
        
        if (endDate.HasValue)
            paymentsQuery = paymentsQuery.Where(p => p.DueDate <= endDate.Value);

        var totalPaid = await paymentsQuery
            .Where(p => p.PaymentStatus == PaymentStatus.Completed)
            .SumAsync(p => p.Amount);

        var totalArrears = await paymentsQuery
            .Where(p => p.PaymentStatus == PaymentStatus.Overdue)
            .SumAsync(p => p.TotalAmountDue);

        var totalOutstanding = await paymentsQuery
            .Where(p => p.PaymentStatus == PaymentStatus.Pending)
            .SumAsync(p => p.Amount);

        return (totalPaid, totalArrears, totalOutstanding);
    }

    #endregion

    #region Benefit Management

    public async Task<IEnumerable<ObligationBenefit>> GetBenefitsByObligationAsync(string obligationId)
    {
        return await _context.ObligationBenefits
            .Include(b => b.Obligation)
            .Where(b => b.ObligationId == obligationId)
            .OrderByDescending(b => b.ReceivedDate)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalBenefitsReceivedAsync(string? obligationId = null)
    {
        var query = _context.ObligationBenefits.AsQueryable();
        
        if (!string.IsNullOrEmpty(obligationId))
            query = query.Where(b => b.ObligationId == obligationId);

        return await query.SumAsync(b => b.Amount);
    }

    public async Task<decimal> GetExpectedAnnualReturnsAsync()
    {
        return await _context.Obligations
            .Where(o => o.Status == ObligationStatus.Active)
            .SumAsync(o => o.ExpectedAnnualReturn ?? 0);
    }

    #endregion

    #region Document Management

    public async Task<IEnumerable<ObligationDocument>> GetDocumentsByObligationAsync(string obligationId)
    {
        return await _context.ObligationDocuments
            .Where(d => d.ObligationId == obligationId)
            .OrderByDescending(d => d.UploadDate)
            .ToListAsync();
    }

    public async Task<ObligationDocument> CreateObligationDocumentAsync(ObligationDocument document)
    {
        document.Id = Guid.NewGuid().ToString();
        document.CreatedAt = DateTime.UtcNow;
        document.TouchUpdatedAt();

        _context.ObligationDocuments.Add(document);
        await _context.SaveChangesAsync();

        return document;
    }

    public async Task<bool> DeleteObligationDocumentAsync(string id)
    {
        var document = await _context.ObligationDocuments.FindAsync(id);
        if (document == null) return false;

        _context.ObligationDocuments.Remove(document);
        await _context.SaveChangesAsync();
        return true;
    }

    #endregion

    #region Validation

    public async Task<bool> ValidateObligationNameAsync(string name, string? excludeObligationId = null)
    {
        var query = _context.Obligations.Where(o => o.Name == name);
        
        if (!string.IsNullOrEmpty(excludeObligationId))
            query = query.Where(o => o.Id != excludeObligationId);

        return !await query.AnyAsync();
    }

    public async Task<bool> HasPaymentsAsync(string obligationId)
    {
        return await _context.ObligationPayments.AnyAsync(p => p.ObligationId == obligationId);
    }

    public async Task<bool> HasBenefitsAsync(string obligationId)
    {
        return await _context.ObligationBenefits.AnyAsync(b => b.ObligationId == obligationId);
    }

    #endregion

    #region Advanced Analytics

    public async Task<PayoffProjection> CalculatePayoffProjectionAsync(string obligationId, decimal monthlyPayment)
    {
        var obligation = await GetObligationByIdAsync(obligationId);
        if (obligation == null)
            throw new ArgumentException("Obligation not found", nameof(obligationId));

        var currentBalance = obligation.CurrentBalance ?? 0m;
        var interestRate = (obligation.InterestRate ?? 0) / 100m / 12m;
        var totalInterest = 0m;
        var monthsToPayoff = 0;
        var balance = currentBalance;

        while (balance > 0 && monthsToPayoff < 600) // Max 50 years
        {
            var interestPayment = balance * interestRate;
            var principalPayment = Math.Min(monthlyPayment - interestPayment, balance);
            
            if (principalPayment <= 0)
                throw new InvalidOperationException("Monthly payment is insufficient to cover interest");

            totalInterest += interestPayment;
            balance -= principalPayment;
            monthsToPayoff++;
        }

        return new PayoffProjection
        {
            DebtId = obligationId,
            MonthsToPayoff = monthsToPayoff,
            PayoffDate = DateTime.Now.AddMonths(monthsToPayoff),
            TotalInterest = totalInterest,
            TotalPayments = currentBalance + totalInterest,
            MinimumPayment = monthlyPayment
        };
    }

    public async Task<DebtStrategy> CalculateDebtStrategyAsync(DebtPriority priority, decimal extraPayment)
    {
        var obligations = priority switch
        {
            DebtPriority.High => await GetObligationsByAvalancheMethodAsync(),
            DebtPriority.Low => await GetObligationsBySnowballMethodAsync(),
            _ => await GetObligationsByCustomPriorityAsync()
        };

        var strategy = new DebtStrategy
        {
            Priority = priority,
            ExtraPayment = extraPayment,
            Obligations = obligations.ToList(),
            TotalMonthsToPayoff = 0,
            TotalInterestSaved = 0
        };

        // Calculate strategy details
        foreach (var obligation in obligations)
        {
            var projection = await CalculatePayoffProjectionAsync(obligation.Id, 
                (obligation.MonthlyContribution) + (extraPayment / obligations.Count()));
            strategy.TotalMonthsToPayoff = Math.Max(strategy.TotalMonthsToPayoff, projection.MonthsToPayoff);
        }

        return strategy;
    }

    public async Task<List<AmortizationPayment>> GetAmortizationScheduleAsync(string obligationId, decimal monthlyPayment, bool fullSchedule = false)
    {
        var obligation = await GetObligationByIdAsync(obligationId);
        if (obligation == null) return new List<AmortizationPayment>();

        var schedule = new List<AmortizationPayment>();
        var currentBalance = obligation.CurrentBalance ?? 0m;
        var interestRate = (obligation.InterestRate ?? 0) / 100m / 12m;
        var currentDate = DateTime.Now;
        var paymentNumber = 1;

        while (currentBalance > 0 && (fullSchedule || schedule.Count < 12))
        {
            var interestPayment = currentBalance * interestRate;
            var principalPayment = Math.Min(monthlyPayment - interestPayment, currentBalance);
            
            if (principalPayment <= 0) break;

            currentBalance -= principalPayment;
            currentDate = currentDate.AddMonths(1);

            schedule.Add(new AmortizationPayment
            {
                PaymentNumber = paymentNumber++,
                PaymentDate = currentDate,
                PaymentAmount = monthlyPayment,
                PrincipalAmount = principalPayment,
                InterestAmount = interestPayment,
                RemainingBalance = currentBalance
            });
        }

        return schedule;
    }

    #endregion

    #region Notification Integration

    public async Task<IEnumerable<ObligationNotification>> GetObligationNotificationsAsync(string obligationId)
    {
        return await _context.ObligationNotifications
            .Where(n => n.ObligationId == obligationId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task CreatePaymentReminderAsync(string obligationId, DateTime reminderDate)
    {
        var obligation = await GetObligationByIdAsync(obligationId);
        if (obligation == null) return;

        var notification = new ObligationNotification
        {
            Id = Guid.NewGuid().ToString(),
            ObligationId = obligationId,
            Title = $"Payment Reminder: {obligation.Name}",
            Message = $"Payment of {obligation.MonthlyContribution:C} is due on {reminderDate:MMM dd, yyyy}",
            NotificationType = AppNotificationType.ObligationPaymentDue,
            CreatedAt = DateTime.UtcNow
        };
        notification.TouchUpdatedAt();

        _context.ObligationNotifications.Add(notification);
        await _context.SaveChangesAsync();

        // Send notification through notification service
        await _notificationService.CreateNotificationAsync(notification.Title, notification.Message, AppNotificationType.ObligationPaymentDue);
    }

    public async Task CreateOverdueNotificationAsync(string obligationId)
    {
        var obligation = await GetObligationByIdAsync(obligationId);
        if (obligation == null) return;

        var notification = new ObligationNotification
        {
            Id = Guid.NewGuid().ToString(),
            ObligationId = obligationId,
            Title = $"Overdue Payment: {obligation.Name}",
            Message = $"Payment is overdue. Please make payment as soon as possible to avoid late fees.",
            NotificationType = AppNotificationType.ObligationArrears,
            CreatedAt = DateTime.UtcNow
        };
        notification.TouchUpdatedAt();

        _context.ObligationNotifications.Add(notification);
        await _context.SaveChangesAsync();

        // Send notification through notification service
        await _notificationService.CreateNotificationAsync(notification.Title, notification.Message, AppNotificationType.ObligationArrears);
    }

    #endregion

    #region Private Helper Methods

    private async Task UpdateObligationBalanceAsync(string obligationId)
    {
        var obligation = await _context.Obligations.FindAsync(obligationId);
        if (obligation == null) return;

        var totalPaid = await _context.ObligationPayments
            .Where(p => p.ObligationId == obligationId && p.PaymentStatus == PaymentStatus.Completed)
            .SumAsync(p => p.Amount);

        // Update current balance (assuming it's calculated from original amount minus payments)
        if (obligation.OriginalAmount.HasValue)
        {
            obligation.CurrentBalance = Math.Max(0, obligation.OriginalAmount.Value - totalPaid);
        }

        obligation.TouchUpdatedAt();
        _context.Obligations.Update(obligation);
        await _context.SaveChangesAsync();
    }

    private decimal CalculateTotalInterest(decimal balance, decimal monthlyPayment, decimal monthlyInterestRate)
    {
        var totalInterest = 0m;
        var currentBalance = balance;

        while (currentBalance > 0)
        {
            var interestPayment = currentBalance * monthlyInterestRate;
            var principalPayment = Math.Min(monthlyPayment - interestPayment, currentBalance);
            
            if (principalPayment <= 0) break;

            totalInterest += interestPayment;
            currentBalance -= principalPayment;
        }

        return totalInterest;
    }

    #endregion

    public async Task<decimal> CalculateDebtAvalancheSavingsAsync()
    {
        var debts = await GetAllObligationsAsync();
        var activeDebts = debts.Where(o => o.IsDebt && o.Status == ObligationStatus.Active && o.CurrentBalance > 0).ToList();
        
        if (!activeDebts.Any()) return 0m;

        // Calculate total interest with minimum payments (snowball method - smallest balance first)
        var snowballDebts = activeDebts.OrderBy(d => d.CurrentBalance ?? 0m).ToList();
        var snowballInterest = await CalculateStrategyTotalInterest(snowballDebts);

        // Calculate total interest with avalanche method (highest interest first)
        var avalancheDebts = activeDebts.OrderByDescending(d => d.InterestRate ?? 0m).ToList();
        var avalancheInterest = await CalculateStrategyTotalInterest(avalancheDebts);

        return Math.Max(0, snowballInterest - avalancheInterest);
    }

    public async Task<PayoffScenario> CalculatePayoffScenarioAsync(IEnumerable<string> debtIds, string strategy, decimal extraPayment, string paymentFrequency, DateTime? targetPayoffDate)
    {
        var debts = new List<Obligation>();
        foreach (var debtId in debtIds)
        {
            var debt = await GetObligationByIdAsync(debtId);
            if (debt != null && debt.IsDebt && debt.Status == ObligationStatus.Active)
            {
                debts.Add(debt);
            }
        }

        if (!debts.Any())
        {
            return new PayoffScenario
            {
                Strategy = strategy,
                PayoffMonths = 0,
                TotalInterest = 0m,
                TotalPayments = 0m,
                PayoffSchedule = new List<DebtPayoffItem>()
            };
        }

        // Sort debts based on strategy
        var sortedDebts = strategy.ToLower() switch
        {
            "avalanche" => debts.OrderByDescending(d => d.InterestRate ?? 0m).ToList(),
            "snowball" => debts.OrderBy(d => d.CurrentBalance ?? 0m).ToList(),
            _ => debts.OrderByDescending(d => d.InterestRate ?? 0m).ToList() // Default to avalanche
        };

        var payoffSchedule = new List<DebtPayoffItem>();
        var totalInterest = 0m;
        var totalPayments = 0m;
        var maxPayoffMonths = 0;

        foreach (var debt in sortedDebts)
        {
            var monthlyPayment = debt.MonthlyContribution + (extraPayment / debts.Count);
            var projection = await CalculatePayoffProjectionAsync(debt.Id, monthlyPayment);
            
            if (projection != null)
            {
                payoffSchedule.Add(new DebtPayoffItem
                {
                    DebtId = debt.Id,
                    DebtName = debt.Name,
                    ObligationType = debt.Type,
                    CurrentBalance = debt.CurrentBalance ?? 0m,
                    InterestRate = debt.InterestRate ?? 0m,
                    MonthlyPayment = monthlyPayment,
                    PayoffDate = projection.PayoffDate,
                    TotalInterest = projection.TotalInterest
                });

                totalInterest += projection.TotalInterest;
                totalPayments += projection.TotalPayments;
                maxPayoffMonths = Math.Max(maxPayoffMonths, projection.MonthsToPayoff);
            }
        }

        return new PayoffScenario
        {
            Strategy = strategy,
            PayoffMonths = maxPayoffMonths,
            TotalInterest = totalInterest,
            TotalPayments = totalPayments,
            PayoffSchedule = payoffSchedule
        };
    }

    private async Task<decimal> CalculateStrategyTotalInterest(List<Obligation> sortedDebts)
    {
        var totalInterest = 0m;
        
        foreach (var debt in sortedDebts)
        {
            var monthlyPayment = debt.MonthlyContribution;
            if (monthlyPayment > 0)
            {
                var projection = await CalculatePayoffProjectionAsync(debt.Id, monthlyPayment);
                if (projection != null)
                {
                    totalInterest += projection.TotalInterest;
                }
            }
        }

        return totalInterest;
    }
}