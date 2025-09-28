using Microsoft.EntityFrameworkCore;
using Pilgrims.PersonalFinances.Data;
using Pilgrims.PersonalFinances.Models;
using Pilgrims.PersonalFinances.Models.DTOs;
using Pilgrims.PersonalFinances.Models.Enums;
using Pilgrims.PersonalFinances.Services.Interfaces;

namespace Pilgrims.PersonalFinances.Services;

/// <summary>
/// Service implementation for comprehensive debt and creditor management with payment tracking
/// </summary>
public class DebtService : IDebtService
{
    private readonly PersonalFinanceContext _context;
    private readonly INotificationService _notificationService;

    public DebtService(PersonalFinanceContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    #region Basic CRUD Operations - Debts

    public async Task<IEnumerable<Debt>> GetAllDebtsAsync()
    {
        return await _context.Debts
            .Include(d => d.Creditor)
            .Include(d => d.Payments)
            .OrderBy(d => d.Name)
            .ToListAsync();
    }

    public async Task<Debt?> GetDebtByIdAsync(string id)
    {
        return await _context.Debts
            .Include(d => d.Creditor)
            .Include(d => d.Payments)
            .Include(d => d.RelatedTransactions)
            .Include(d => d.RelatedBudgets)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<Debt> CreateDebtAsync(Debt debt)
    {
        debt.Id = Guid.NewGuid().ToString();
        debt.CreatedAt = DateTime.UtcNow;
        debt.MarkAsDirty();
        debt.IsActive = true;

        _context.Debts.Add(debt);
        await _context.SaveChangesAsync();

        // Create notification for new debt
        await _notificationService.CreateNotificationAsync(
            "New Debt Added",
            $"Debt '{debt.Name}' has been added with a balance of {debt.CurrentBalance:C}",
            AppNotificationType.DebtPayment,
            NotificationPriority.Medium);

        return debt;
    }

    public async Task<Debt> UpdateDebtAsync(Debt debt)
    {
        _context.Debts.Update(debt);
        await _context.SaveChangesAsync();
        return debt;
    }

    public async Task<bool> DeleteDebtAsync(string id)
    {
        var debt = await _context.Debts.FindAsync(id);
        if (debt == null) return false;

        // Check if debt has payments
        var hasPayments = await _context.DebtPayments.AnyAsync(p => p.DebtId == id);
        if (hasPayments)
        {
            // Soft delete by marking as inactive
            debt.IsActive = false;
            _context.Debts.Update(debt);
        }
        else
        {
            // Hard delete if no payments
            _context.Debts.Remove(debt);
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteDebtsAsync(IEnumerable<string> ids)
    {
        var debts = await _context.Debts.Where(d => ids.Contains(d.Id)).ToListAsync();
        if (!debts.Any()) return false;

        foreach (var debt in debts)
        {
            var hasPayments = await _context.DebtPayments.AnyAsync(p => p.DebtId == debt.Id);
            if (hasPayments)
            {
                debt.IsActive = false;
            }
            else
            {
                _context.Debts.Remove(debt);
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }

    #endregion

    #region Basic CRUD Operations - Creditors

    public async Task<IEnumerable<Creditor>> GetAllCreditorsAsync()
    {
        return await _context.Creditors
            .Include(c => c.Debts)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Creditor?> GetCreditorByIdAsync(string id)
    {
        return await _context.Creditors
            .Include(c => c.Debts)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Creditor> CreateCreditorAsync(Creditor creditor)
    {
        creditor.Id = Guid.NewGuid().ToString();
        creditor.CreatedAt = DateTime.UtcNow;
        creditor.MarkAsDirty();

        _context.Creditors.Add(creditor);
        await _context.SaveChangesAsync();
        return creditor;
    }

    public async Task<Creditor> UpdateCreditorAsync(Creditor creditor)
    {
        _context.Creditors.Update(creditor);
        await _context.SaveChangesAsync();
        return creditor;
    }

    public async Task<bool> DeleteCreditorAsync(string id)
    {
        var creditor = await _context.Creditors.FindAsync(id);
        if (creditor == null) return false;

        // Check if creditor has debts
        var hasDebts = await _context.Debts.AnyAsync(d => d.CreditorId == id);
        if (hasDebts) return false; // Cannot delete creditor with existing debts

        _context.Creditors.Remove(creditor);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteCreditorsAsync(IEnumerable<string> ids)
    {
        var creditors = await _context.Creditors.Where(c => ids.Contains(c.Id)).ToListAsync();
        if (!creditors.Any()) return false;

        foreach (var creditor in creditors)
        {
            var hasDebts = await _context.Debts.AnyAsync(d => d.CreditorId == creditor.Id);
            if (!hasDebts)
            {
                _context.Creditors.Remove(creditor);
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }

    #endregion

    #region Basic CRUD Operations - Debt Payments

    public async Task<IEnumerable<DebtPayment>> GetAllDebtPaymentsAsync()
    {
        return await _context.DebtPayments
            .Include(p => p.Debt)
            .ThenInclude(d => d.Creditor)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync();
    }

    public async Task<DebtPayment?> GetDebtPaymentByIdAsync(string id)
    {
        return await _context.DebtPayments
            .Include(p => p.Debt)
            .ThenInclude(d => d.Creditor)
            .Include(p => p.ScheduledTransaction)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<DebtPayment> CreateDebtPaymentAsync(DebtPayment payment)
    {
        payment.Id = Guid.NewGuid().ToString();
        payment.CreatedAt = DateTime.UtcNow;

        _context.DebtPayments.Add(payment);

        // Update debt balance
        var debt = await _context.Debts.FindAsync(payment.DebtId);
        if (debt != null)
        {
            debt.CurrentBalance -= payment.PrincipalAmount ?? 0;

            // Check if debt is paid off
            if (debt.CurrentBalance <= 0)
            {
                debt.CurrentBalance = 0;
                debt.PaidOffDate = payment.PaymentDate;
                debt.IsActive = false;

                await _notificationService.CreateNotificationAsync(
                    "Debt Paid Off!",
                    $"Congratulations! You've paid off '{debt.Name}'",
                    AppNotificationType.DebtPayment,
                    NotificationPriority.High);
            }
        }

        await _context.SaveChangesAsync();
        return payment;
    }

    public async Task<DebtPayment> UpdateDebtPaymentAsync(DebtPayment payment)
    {
        _context.DebtPayments.Update(payment);
        await _context.SaveChangesAsync();
        return payment;
    }

    public async Task<bool> DeleteDebtPaymentAsync(string id)
    {
        var payment = await _context.DebtPayments.FindAsync(id);
        if (payment == null) return false;

        // Restore debt balance
        var debt = await _context.Debts.FindAsync(payment.DebtId);
        if (debt != null)
        {
            debt.CurrentBalance += payment.PrincipalAmount ?? 0;
            
            // If debt was marked as paid off, reactivate it
            if (debt.PaidOffDate.HasValue)
            {
                debt.PaidOffDate = null;
                debt.IsActive = true;
            }
        }

        _context.DebtPayments.Remove(payment);
        await _context.SaveChangesAsync();
        return true;
    }

    #endregion

    #region Debt Filtering and Search

    public async Task<IEnumerable<Debt>> GetActiveDebtsAsync()
    {
        return await _context.Debts
            .Include(d => d.Creditor)
            .Include(d => d.Payments)
            .Where(d => d.IsActive && d.CurrentBalance > 0)
            .OrderBy(d => d.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Debt>> GetDebtsByCreditorAsync(string creditorId)
    {
        return await _context.Debts
            .Include(d => d.Creditor)
            .Include(d => d.Payments)
            .Where(d => d.CreditorId == creditorId)
            .OrderBy(d => d.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Debt>> GetDebtsByTypeAsync(DebtType debtType)
    {
        return await _context.Debts
            .Include(d => d.Creditor)
            .Include(d => d.Payments)
            .Where(d => d.DebtType == debtType && d.IsActive)
            .OrderBy(d => d.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Debt>> GetDebtsByPriorityAsync(DebtPriority priority)
    {
        return await _context.Debts
            .Include(d => d.Creditor)
            .Include(d => d.Payments)
            .Where(d => d.Priority == priority && d.IsActive)
            .OrderBy(d => d.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Debt>> GetOverdueDebtsAsync()
    {
        var today = DateTime.Today;
        return await _context.Debts
            .Include(d => d.Creditor)
            .Include(d => d.Payments)
            .Where(d => d.IsActive && d.NextPaymentDue < today)
            .OrderBy(d => d.NextPaymentDue)
            .ToListAsync();
    }

    public async Task<IEnumerable<Debt>> GetDebtsDueInDaysAsync(int days)
    {
        var targetDate = DateTime.Today.AddDays(days);
        return await _context.Debts
            .Include(d => d.Creditor)
            .Include(d => d.Payments)
            .Where(d => d.IsActive && d.NextPaymentDue <= targetDate && d.NextPaymentDue >= DateTime.Today)
            .OrderBy(d => d.NextPaymentDue)
            .ToListAsync();
    }

    public async Task<IEnumerable<Debt>> SearchDebtsAsync(string searchTerm)
    {
        return await _context.Debts
            .Include(d => d.Creditor)
            .Include(d => d.Payments)
            .Where(d => d.Name.Contains(searchTerm) || 
                       d.Creditor.Name.Contains(searchTerm) ||
                       (d.AccountNumber != null && d.AccountNumber.Contains(searchTerm)) ||
                       (d.Notes != null && d.Notes.Contains(searchTerm)))
            .OrderBy(d => d.Name)
            .ToListAsync();
    }

    #endregion

    #region Payment Management

    public async Task<IEnumerable<DebtPayment>> GetPaymentsByDebtAsync(string debtId)
    {
        return await _context.DebtPayments
            .Include(p => p.Debt)
            .Where(p => p.DebtId == debtId)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<DebtPayment>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.DebtPayments
            .Include(p => p.Debt)
            .ThenInclude(d => d.Creditor)
            .Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<DebtPayment>> GetScheduledPaymentsAsync()
    {
        return await _context.DebtPayments
            .Include(p => p.Debt)
            .ThenInclude(d => d.Creditor)
            .Include(p => p.ScheduledTransaction)
            .Where(p => p.IsScheduled && p.ScheduledTransactionId != null)
            .OrderBy(p => p.PaymentDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<DebtPayment>> GetOverduePaymentsAsync()
    {
        var today = DateTime.Today;
        return await _context.DebtPayments
            .Include(p => p.Debt)
            .ThenInclude(d => d.Creditor)
            .Where(p => p.IsScheduled && p.PaymentDate < today)
            .OrderBy(p => p.PaymentDate)
            .ToListAsync();
    }

    #endregion

    #region Debt Calculations and Analytics

    public async Task<decimal> GetTotalDebtBalanceAsync()
    {
        return await _context.Debts
            .Where(d => d.IsActive)
            .SumAsync(d => d.CurrentBalance);
    }

    public async Task<decimal> GetTotalDebtByTypeAsync(DebtType debtType)
    {
        return await _context.Debts
            .Where(d => d.IsActive && d.DebtType == debtType)
            .SumAsync(d => d.CurrentBalance);
    }

    public async Task<decimal> GetTotalDebtByCreditorAsync(string creditorId)
    {
        return await _context.Debts
            .Where(d => d.IsActive && d.CreditorId == creditorId)
            .SumAsync(d => d.CurrentBalance);
    }

    public async Task<decimal> GetMonthlyPaymentObligationsAsync()
    {
        return await _context.Debts
            .Where(d => d.IsActive)
            .SumAsync(d => d.MinimumPayment ?? 0);
    }

    public async Task<decimal> GetTotalInterestPaidAsync(string? debtId = null)
    {
        var query = _context.DebtPayments.AsQueryable();
        
        if (!string.IsNullOrEmpty(debtId))
            query = query.Where(p => p.DebtId == debtId);

        return await query.SumAsync(p => p.InterestAmount ?? 0);
    }

    public async Task<decimal> GetTotalPrincipalPaidAsync(string? debtId = null)
    {
        var query = _context.DebtPayments.AsQueryable();
        
        if (!string.IsNullOrEmpty(debtId))
            query = query.Where(p => p.DebtId == debtId);

        return await query.SumAsync(p => p.PrincipalAmount ?? 0);
    }

    #endregion

    #region Payoff Calculations

    public async Task<DateTime?> CalculatePayoffDateAsync(string debtId, decimal monthlyPayment)
    {
        var debt = await GetDebtByIdAsync(debtId);
        if (debt == null || debt.CurrentBalance <= 0 || monthlyPayment <= 0)
            return null;

        var balance = debt.CurrentBalance;
        var monthlyInterestRate = (debt.InterestRate ?? 0) / 100 / 12;
        var months = 0;

        if (monthlyInterestRate == 0)
        {
            // Simple calculation without interest
            months = (int)Math.Ceiling(balance / monthlyPayment);
        }
        else
        {
            // Compound interest calculation
            while (balance > 0 && months < 1200) // Max 100 years
            {
                var interestPayment = balance * monthlyInterestRate;
                var principalPayment = Math.Min(monthlyPayment - interestPayment, balance);
                
                if (principalPayment <= 0) return null; // Payment too low to cover interest
                
                balance -= principalPayment;
                months++;
            }
        }

        return DateTime.Today.AddMonths(months);
    }

    public async Task<decimal> CalculateInterestSavingsAsync(string debtId, decimal extraPayment)
    {
        var debt = await GetDebtByIdAsync(debtId);
        if (debt == null || extraPayment <= 0) return 0;

        var normalPayoffDate = await CalculatePayoffDateAsync(debtId, debt.MinimumPayment ?? 0);
        var acceleratedPayoffDate = await CalculatePayoffDateAsync(debtId, (debt.MinimumPayment ?? 0) + extraPayment);

        if (normalPayoffDate == null || acceleratedPayoffDate == null) return 0;

        var monthsSaved = (normalPayoffDate.Value.Year - acceleratedPayoffDate.Value.Year) * 12 +
                         (normalPayoffDate.Value.Month - acceleratedPayoffDate.Value.Month);

        var monthlyInterestRate = (debt.InterestRate ?? 0) / 100 / 12;
        return monthsSaved * debt.CurrentBalance * monthlyInterestRate;
    }

    public async Task<decimal> CalculateMinimumPaymentAsync(string debtId)
    {
        var debt = await GetDebtByIdAsync(debtId);
        if (debt == null) return 0;

        // Return stored minimum payment or calculate based on balance percentage
        if (debt.MinimumPayment.HasValue && debt.MinimumPayment > 0)
            return debt.MinimumPayment.Value;

        // Default to 2% of balance for credit cards, or fixed amount for loans
        return debt.DebtType == DebtType.CreditCard 
            ? debt.CurrentBalance * 0.02m 
            : debt.CurrentBalance * 0.01m;
    }

    public async Task<IEnumerable<(DateTime Date, decimal Balance)>> GetAmortizationScheduleAsync(string debtId, decimal monthlyPayment)
    {
        var debt = await GetDebtByIdAsync(debtId);
        if (debt == null) return new List<(DateTime, decimal)>();

        var schedule = new List<(DateTime Date, decimal Balance)>();
        var balance = debt.CurrentBalance;
        var monthlyInterestRate = (debt.InterestRate ?? 0) / 100 / 12;
        var currentDate = DateTime.Today;

        while (balance > 0.01m && schedule.Count < 1200) // Max 100 years
        {
            var interestPayment = balance * monthlyInterestRate;
            var principalPayment = Math.Min(monthlyPayment - interestPayment, balance);
            
            if (principalPayment <= 0) break;
            
            balance -= principalPayment;
            currentDate = currentDate.AddMonths(1);
            schedule.Add((currentDate, Math.Max(0, balance)));
        }

        return schedule;
    }

    #endregion

    #region Debt Prioritization

    public async Task<IEnumerable<Debt>> GetDebtsBySnowballMethodAsync()
    {
        return await _context.Debts
            .Include(d => d.Creditor)
            .Include(d => d.Payments)
            .Where(d => d.IsActive && d.CurrentBalance > 0)
            .OrderBy(d => d.CurrentBalance) // Smallest balance first
            .ToListAsync();
    }

    public async Task<IEnumerable<Debt>> GetDebtsByAvalancheMethodAsync()
    {
        return await _context.Debts
            .Include(d => d.Creditor)
            .Include(d => d.Payments)
            .Where(d => d.IsActive && d.CurrentBalance > 0)
            .OrderByDescending(d => d.InterestRate ?? 0) // Highest interest first
            .ToListAsync();
    }

    public async Task<IEnumerable<Debt>> GetDebtsByCustomPriorityAsync()
    {
        return await _context.Debts
            .Include(d => d.Creditor)
            .Include(d => d.Payments)
            .Where(d => d.IsActive && d.CurrentBalance > 0)
            .OrderBy(d => d.Priority)
            .ThenByDescending(d => d.InterestRate ?? 0)
            .ToListAsync();
    }

    #endregion

    #region Creditor Management

    public async Task<IEnumerable<Debt>> GetDebtsByCreditorWithBalanceAsync(string creditorId)
    {
        return await _context.Debts
            .Include(d => d.Creditor)
            .Include(d => d.Payments)
            .Where(d => d.CreditorId == creditorId && d.CurrentBalance > 0)
            .OrderBy(d => d.Name)
            .ToListAsync();
    }

    public async Task<bool> CanDeleteCreditorAsync(string creditorId)
    {
        return !await _context.Debts.AnyAsync(d => d.CreditorId == creditorId);
    }

    public async Task<IEnumerable<Creditor>> SearchCreditorsAsync(string searchTerm)
    {
        return await _context.Creditors
            .Include(c => c.Debts)
            .Where(c => c.Name.Contains(searchTerm) ||
                       (c.Phone != null && c.Phone.Contains(searchTerm)) ||
                       (c.Email != null && c.Email.Contains(searchTerm)) ||
                       (c.AccountNumber != null && c.AccountNumber.Contains(searchTerm)))
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    #endregion

    #region Reporting and Statistics

    public async Task<Dictionary<DebtType, decimal>> GetDebtBalancesByTypeAsync()
    {
        return await _context.Debts
            .Where(d => d.IsActive && d.CurrentBalance > 0)
            .GroupBy(d => d.DebtType)
            .ToDictionaryAsync(g => g.Key, g => g.Sum(d => d.CurrentBalance));
    }

    public async Task<Dictionary<DebtPriority, decimal>> GetDebtBalancesByPriorityAsync()
    {
        return await _context.Debts
            .Where(d => d.IsActive && d.CurrentBalance > 0)
            .GroupBy(d => d.Priority)
            .ToDictionaryAsync(g => g.Key, g => g.Sum(d => d.CurrentBalance));
    }

    public async Task<Dictionary<string, decimal>> GetDebtBalancesByCreditorsAsync()
    {
        return await _context.Debts
            .Include(d => d.Creditor)
            .Where(d => d.IsActive && d.CurrentBalance > 0)
            .GroupBy(d => d.Creditor.Name)
            .ToDictionaryAsync(g => g.Key, g => g.Sum(d => d.CurrentBalance));
    }

    public async Task<decimal> GetDebtToIncomeRatioAsync(decimal monthlyIncome)
    {
        if (monthlyIncome <= 0) return 0;
        
        var monthlyPayments = await GetMonthlyPaymentObligationsAsync();
        return monthlyPayments / monthlyIncome;
    }

    public async Task<(decimal TotalPaid, decimal InterestPaid, decimal PrincipalPaid)> GetPaymentSummaryAsync(
        string? debtId = null, DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.DebtPayments.AsQueryable();

        if (!string.IsNullOrEmpty(debtId))
            query = query.Where(p => p.DebtId == debtId);

        if (startDate.HasValue)
            query = query.Where(p => p.PaymentDate >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(p => p.PaymentDate <= endDate.Value);

        var payments = await query.ToListAsync();

        return (
            TotalPaid: payments.Sum(p => p.Amount),
            InterestPaid: payments.Sum(p => p.InterestAmount ?? 0),
            PrincipalPaid: payments.Sum(p => p.PrincipalAmount ?? 0)
        );
    }

    #endregion

    #region Validation

    public async Task<bool> ValidateDebtNameAsync(string name, string? excludeDebtId = null)
    {
        var query = _context.Debts.Where(d => d.Name == name);
        
        if (!string.IsNullOrEmpty(excludeDebtId))
            query = query.Where(d => d.Id != excludeDebtId);

        return !await query.AnyAsync();
    }

    public async Task<bool> ValidateCreditorNameAsync(string name, string? excludeCreditorId = null)
    {
        var query = _context.Creditors.Where(c => c.Name == name);
        
        if (!string.IsNullOrEmpty(excludeCreditorId))
            query = query.Where(c => c.Id != excludeCreditorId);

        return !await query.AnyAsync();
    }

    public async Task<bool> HasPaymentsAsync(string debtId)
    {
        return await _context.DebtPayments.AnyAsync(p => p.DebtId == debtId);
    }

    public async Task<bool> HasDebtsAsync(string creditorId)
    {
        return await _context.Debts.AnyAsync(d => d.CreditorId == creditorId && d.IsActive);
    }

    #endregion

    #region Advanced Payoff Calculations

    public async Task<PayoffProjection> CalculatePayoffProjectionAsync(string debtId, decimal monthlyPayment)
    {
        var debt = await GetDebtByIdAsync(debtId);
        if (debt == null)
            throw new ArgumentException("Debt not found", nameof(debtId));

        var currentBalance = debt.CurrentBalance;
        var interestRate = (debt.InterestRate ?? 0) / 100m / 12m;
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
            DebtId = debtId,
            MonthsToPayoff = monthsToPayoff,
            PayoffDate = DateTime.Now.AddMonths(monthsToPayoff),
            TotalInterest = totalInterest,
            TotalPayments = currentBalance + totalInterest,
            MinimumPayment = monthlyPayment
        };
    }

    public async Task<DebtStrategy> CalculateDebtStrategyAsync(DebtPriority priority, decimal extraPayment)
    {
        var debts = await GetActiveDebtsAsync();
        var debtList = debts.ToList();

        if (!debtList.Any())
        {
            return new DebtStrategy
             {
                 StrategyName = GetStrategyName(priority),
                 TotalMonths = 0,
                 TotalInterest = 0,
                 TotalPayments = 0,
                 PaymentPlan = new List<DebtPaymentPlan>()
             };
        }

        // Sort debts based on strategy
        var sortedDebts = priority switch
        {
            DebtPriority.Low => debtList.OrderBy(d => d.CurrentBalance).ToList(), // Snowball
            DebtPriority.High => debtList.OrderByDescending(d => d.InterestRate ?? 0).ToList(), // Avalanche
            _ => debtList.OrderBy(d => d.Priority).ToList() // Custom
        };

        var paymentPlans = new List<DebtPaymentPlan>();
        var totalMonths = 0;
        var totalInterest = 0m;
        var totalPayments = 0m;
        var availableExtraPayment = extraPayment;

        foreach (var debt in sortedDebts)
         {
             var monthlyPayment = debt.MinimumPayment ?? 0;
             if (availableExtraPayment > 0)
             {
                 monthlyPayment += availableExtraPayment;
                 availableExtraPayment = 0; // Apply extra payment to first debt only
             }

             var projection = await CalculatePayoffProjectionAsync(debt.Id, monthlyPayment);
             
             paymentPlans.Add(new DebtPaymentPlan
             {
                 DebtId = debt.Id,
                 DebtName = debt.Name,
                 MonthlyPayment = monthlyPayment,
                 MonthsToPayoff = projection.MonthsToPayoff,
                 TotalInterest = projection.TotalInterest
             });

             totalMonths = Math.Max(totalMonths, projection.MonthsToPayoff);
             totalInterest += projection.TotalInterest;
             totalPayments += projection.TotalPayments;
         }

        return new DebtStrategy
        {
            StrategyName = GetStrategyName(priority),
            TotalMonths = totalMonths,
            TotalInterest = totalInterest,
            TotalPayments = totalPayments,
            PaymentPlan = paymentPlans
        };
    }

    public async Task<List<AmortizationPayment>> GetAmortizationScheduleAsync(string debtId, decimal monthlyPayment, bool fullSchedule = false)
    {
        var debt = await GetDebtByIdAsync(debtId);
        if (debt == null)
            throw new ArgumentException("Debt not found", nameof(debtId));

        var schedule = new List<AmortizationPayment>();
        var balance = debt.CurrentBalance;
        var interestRate = (debt.InterestRate ?? 0) / 100m / 12m;
        var paymentNumber = 1;
        var maxPayments = fullSchedule ? 600 : 12; // Show 1 year by default

        while (balance > 0 && paymentNumber <= maxPayments)
        {
            var interestPayment = balance * interestRate;
            var principalPayment = Math.Min(monthlyPayment - interestPayment, balance);
            
            if (principalPayment <= 0)
                break;

            balance -= principalPayment;

            schedule.Add(new AmortizationPayment
            {
                PaymentNumber = paymentNumber,
                PaymentDate = DateTime.Now.AddMonths(paymentNumber - 1),
                PaymentAmount = monthlyPayment,
                PrincipalAmount = principalPayment,
                InterestAmount = interestPayment,
                RemainingBalance = balance
            });

            paymentNumber++;
        }

        return schedule;
    }

    private string GetStrategyName(DebtPriority priority)
    {
        return priority switch
        {
            DebtPriority.Low => "Debt Snowball",
            DebtPriority.High => "Debt Avalanche",
            _ => "Custom Priority"
        };
    }

    #endregion
}