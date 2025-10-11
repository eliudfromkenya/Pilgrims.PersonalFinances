using Microsoft.EntityFrameworkCore;
using Pilgrims.PersonalFinances.Data;
using Pilgrims.PersonalFinances.Core.Models;
using Pilgrims.PersonalFinances.Core.Models.Enums;
using Pilgrims.PersonalFinances.Core.Models.DTOs;
using Pilgrims.PersonalFinances.Core.Services.Interfaces;

namespace Pilgrims.PersonalFinances.Core.Services;

/// <summary>
/// Service implementation for comprehensive budget management with tracking and alerts
/// </summary>
public class BudgetService : IBudgetService
{
    private readonly PersonalFinanceContext _context;
    private readonly INotificationService _notificationService;

    public BudgetService(PersonalFinanceContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    #region Basic CRUD Operations

    public async Task<IEnumerable<Budget>> GetAllBudgetsAsync()
    {
        return await _context.Budgets
            .Include(b => b.Category)
            .Include(b => b.Account)
            .Include(b => b.BudgetAlerts)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<Budget?> GetBudgetByIdAsync(string id)
    {
        return await _context.Budgets
            .Include(b => b.Category)
            .Include(b => b.Account)
            .Include(b => b.BudgetAlerts)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<Budget> CreateBudgetAsync(Budget budget)
    {
        if (budget == null)
            throw new ArgumentNullException(nameof(budget));

        // Validate budget
        if (!await ValidateBudgetAsync(budget))
            throw new InvalidOperationException("Budget validation failed");

        // Set default values
        budget.Id = Guid.NewGuid().ToString();
        budget.CreatedAt = DateTime.UtcNow;
        budget.MarkAsDirty();
        budget.SpentAmount = 0;

        _context.Budgets.Add(budget);
        await _context.SaveChangesAsync();

        return budget;
    }

    public async Task<Budget> UpdateBudgetAsync(Budget budget)
    {
        if (budget == null)
            throw new ArgumentNullException(nameof(budget));

        var existingBudget = await GetBudgetByIdAsync(budget.Id);
        if (existingBudget == null)
            throw new InvalidOperationException("Budget not found");

        // Validate budget
        if (!await ValidateBudgetAsync(budget))
            throw new InvalidOperationException("Budget validation failed");

        // Update properties
        existingBudget.Name = budget.Name;
        existingBudget.Description = budget.Description;
        existingBudget.BudgetType = budget.BudgetType;
        existingBudget.Period = budget.Period;
        existingBudget.LimitAmount = budget.LimitAmount;
        existingBudget.StartDate = budget.StartDate;
        existingBudget.EndDate = budget.EndDate;
        existingBudget.IsActive = budget.IsActive;
        existingBudget.AllowOverspend = budget.AllowOverspend;
        existingBudget.EnableRollover = budget.EnableRollover;
        existingBudget.CategoryId = budget.CategoryId;
        existingBudget.AccountId = budget.AccountId;
        existingBudget.Tag = budget.Tag;
        existingBudget.AlertLevels = budget.AlertLevels;
        existingBudget.AlertsEnabled = budget.AlertsEnabled;
        existingBudget.MarkAsDirty();

        await _context.SaveChangesAsync();
        return existingBudget;
    }

    public async Task<bool> DeleteBudgetAsync(string id)
    {
        var budget = await GetBudgetByIdAsync(id);
        if (budget == null)
            return false;

        if (!await CanDeleteBudgetAsync(id))
            return false;

        _context.Budgets.Remove(budget);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteBudgetsAsync(IEnumerable<string> ids)
    {
        var budgets = await _context.Budgets
            .Where(b => ids.Contains(b.Id))
            .ToListAsync();

        if (!budgets.Any())
            return false;

        _context.Budgets.RemoveRange(budgets);
        await _context.SaveChangesAsync();
        return true;
    }

    #endregion

    #region Budget Filtering and Search

    public async Task<IEnumerable<Budget>> GetActiveBudgetsAsync()
    {
        return await _context.Budgets
            .Include(b => b.Category)
            .Include(b => b.Account)
            .Where(b => b.IsActive)
            .OrderBy(b => b.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Budget>> GetBudgetsByTypeAsync(BudgetType budgetType)
    {
        return await _context.Budgets
            .Include(b => b.Category)
            .Include(b => b.Account)
            .Where(b => b.BudgetType == budgetType)
            .OrderBy(b => b.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Budget>> GetBudgetsByPeriodAsync(BudgetPeriod period)
    {
        return await _context.Budgets
            .Include(b => b.Category)
            .Include(b => b.Account)
            .Where(b => b.Period == period)
            .OrderBy(b => b.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Budget>> GetBudgetsByPeriodAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Budgets
            .Include(b => b.Category)
            .Include(b => b.Account)
            .Where(b => b.StartDate <= endDate && b.EndDate >= startDate)
            .OrderBy(b => b.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Budget>> GetBudgetsWithTrackingAsync()
    {
        return await _context.Budgets
            .Include(b => b.Category)
            .Include(b => b.Account)
            .Include(b => b.BudgetAlerts)
            .Where(b => b.IsActive)
            .OrderBy(b => b.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Budget>> GetBudgetsByCategoryAsync(string categoryId)
    {
        return await _context.Budgets
            .Include(b => b.Category)
            .Include(b => b.Account)
            .Where(b => b.CategoryId == categoryId)
            .OrderBy(b => b.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Budget>> GetBudgetsByAccountAsync(string accountId)
    {
        return await _context.Budgets
            .Include(b => b.Category)
            .Include(b => b.Account)
            .Where(b => b.AccountId == accountId)
            .OrderBy(b => b.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Budget>> GetBudgetsByTagAsync(string tag)
    {
        return await _context.Budgets
            .Include(b => b.Category)
            .Include(b => b.Account)
            .Where(b => b.Tag == tag)
            .OrderBy(b => b.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Budget>> GetCurrentBudgetsAsync()
    {
        var today = DateTime.Today;
        return await _context.Budgets
            .Include(b => b.Category)
            .Include(b => b.Account)
            .Where(b => b.IsActive && b.StartDate <= today && b.EndDate >= today)
            .OrderBy(b => b.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Budget>> GetOverBudgetsAsync()
    {
        return await _context.Budgets
            .Include(b => b.Category)
            .Include(b => b.Account)
            .Where(b => b.IsActive && b.SpentAmount > (b.LimitAmount + b.RolloverAmount))
            .OrderByDescending(b => b.SpentAmount - (b.LimitAmount + b.RolloverAmount))
            .ToListAsync();
    }

    #endregion

    #region Budget Tracking and Calculations

    public async Task<decimal> CalculateBudgetUtilizationAsync(string budgetId)
    {
        var budget = await GetBudgetByIdAsync(budgetId);
        if (budget == null)
            return 0;

        var totalBudget = budget.LimitAmount + budget.RolloverAmount;
        return totalBudget > 0 ? (budget.SpentAmount / totalBudget) * 100 : 0;
    }

    public async Task<decimal> GetBudgetSpentAmountAsync(string budgetId)
    {
        var budget = await GetBudgetByIdAsync(budgetId);
        return budget?.SpentAmount ?? 0;
    }

    public async Task<decimal> GetBudgetRemainingAmountAsync(string budgetId)
    {
        var budget = await GetBudgetByIdAsync(budgetId);
        if (budget == null)
            return 0;

        return (budget.LimitAmount + budget.RolloverAmount) - budget.SpentAmount;
    }

    public async Task<bool> UpdateBudgetSpentAmountAsync(string budgetId, decimal amount)
    {
        var budget = await GetBudgetByIdAsync(budgetId);
        if (budget == null)
            return false;

        budget.SpentAmount += amount;
        budget.MarkAsDirty();

        await _context.SaveChangesAsync();

        // Check for alerts
        await CheckAndCreateBudgetAlertsAsync(budgetId);

        return true;
    }

    public async Task<bool> RecalculateBudgetSpentAmountAsync(string budgetId)
    {
        var budget = await GetBudgetByIdAsync(budgetId);
        if (budget == null)
            return false;

        decimal spentAmount = 0;

        // Calculate spent amount based on budget type
        switch (budget.BudgetType)
        {
            case BudgetType.Category:
                if (!string.IsNullOrEmpty(budget.CategoryId))
                {
                    spentAmount = await _context.Transactions
                        .Where(t => t.CategoryId == budget.CategoryId &&
                                   t.Date >= budget.StartDate &&
                t.Date <= budget.EndDate &&
                                   t.Type == TransactionType.Expense)
                        .SumAsync(t => t.Amount);
                }
                break;

            case BudgetType.Account:
                if (!string.IsNullOrEmpty(budget.AccountId))
                {
                    spentAmount = await _context.Transactions
                        .Where(t => t.AccountId == budget.AccountId &&
                                   t.Date >= budget.StartDate &&
                t.Date <= budget.EndDate &&
                                   t.Type == TransactionType.Expense)
                        .SumAsync(t => t.Amount);
                }
                break;

            case BudgetType.Tag:
                if (!string.IsNullOrEmpty(budget.Tag))
                {
                    spentAmount = await _context.Transactions
                        .Where(t => t.Tags != null && t.Tags.Contains(budget.Tag) &&
                                   t.Date >= budget.StartDate &&
                t.Date <= budget.EndDate &&
                                   t.Type == TransactionType.Expense)
                        .SumAsync(t => t.Amount);
                }
                break;

            case BudgetType.TimePeriod:
                spentAmount = await _context.Transactions
                    .Where(t => t.Date >= budget.StartDate &&
                    t.Date <= budget.EndDate &&
                               t.Type == TransactionType.Expense)
                    .SumAsync(t => t.Amount);
                break;
        }

        budget.SpentAmount = spentAmount;
        budget.MarkAsDirty();

        await _context.SaveChangesAsync();

        // Check for alerts
        await CheckAndCreateBudgetAlertsAsync(budgetId);

        return true;
    }

    public async Task<bool> RecalculateAllBudgetSpentAmountsAsync()
    {
        var budgets = await GetActiveBudgetsAsync();
        
        foreach (var budget in budgets)
        {
            await RecalculateBudgetSpentAmountAsync(budget.Id);
        }

        return true;
    }

    #endregion

    #region Budget Alerts

    public async Task<IEnumerable<BudgetAlert>> GetBudgetAlertsAsync(string budgetId)
    {
        return await _context.BudgetAlerts
            .Where(ba => ba.BudgetId == budgetId)
            .OrderByDescending(ba => ba.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<BudgetAlert>> GetActiveBudgetAlertsAsync()
    {
        return await _context.BudgetAlerts
            .Include(ba => ba.Budget)
            .Where(ba => !ba.IsRead)
            .OrderByDescending(ba => ba.CreatedAt)
            .ToListAsync();
    }

    public async Task<BudgetAlert> CreateBudgetAlertAsync(BudgetAlert alert)
    {
        if (alert == null)
            throw new ArgumentNullException(nameof(alert));

        alert.Id = Guid.NewGuid().ToString();
        alert.CreatedAt = DateTime.UtcNow;

        _context.BudgetAlerts.Add(alert);
        await _context.SaveChangesAsync();

        // Send notification
        await _notificationService.CreateBudgetAlertNotificationAsync(alert);

        return alert;
    }

    public async Task<BudgetAlert> UpdateBudgetAlertAsync(BudgetAlert alert)
    {
        if (alert == null)
            throw new ArgumentNullException(nameof(alert));

        var existingAlert = await _context.BudgetAlerts.FindAsync(alert.Id);
        if (existingAlert == null)
            throw new ArgumentException($"Budget alert with ID {alert.Id} not found.");

        existingAlert.IsActive = alert.IsActive;
        existingAlert.Severity = alert.Severity;
        existingAlert.ThresholdPercentage = alert.ThresholdPercentage;
        existingAlert.Message = alert.Message;
        existingAlert.IsRead = alert.IsRead;
        existingAlert.ReadDate = alert.ReadDate;
        existingAlert.MarkAsDirty();

        await _context.SaveChangesAsync();
        return existingAlert;
    }

    public async Task<bool> DeleteBudgetAlertAsync(string alertId)
    {
        var alert = await _context.BudgetAlerts.FindAsync(alertId);
        if (alert == null)
            return false;

        _context.BudgetAlerts.Remove(alert);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CheckAndCreateBudgetAlertsAsync(string budgetId)
    {
        var budget = await GetBudgetByIdAsync(budgetId);
        if (budget == null || !budget.AlertsEnabled)
            return false;

        var utilization = await CalculateBudgetUtilizationAsync(budgetId);
        var alertLevels = budget.AlertLevelsList;

        foreach (var level in alertLevels)
        {
            if (utilization >= level && (budget.LastAlertLevel == null || budget.LastAlertLevel < level))
            {
                var alert = new BudgetAlert
                {
                    BudgetId = budgetId,
                    AlertLevel = level,
                    BudgetAmount = budget.LimitAmount + budget.RolloverAmount,
                    SpentAmount = budget.SpentAmount,
                    UsedPercentage = utilization,
                    Message = $"Budget '{budget.Name}' has reached {level}% utilization",
                    Severity = level >= 90 ? AlertSeverity.Critical : AlertSeverity.Warning
                };

                await CreateBudgetAlertAsync(alert);

                // Update last alert level
                budget.LastAlertLevel = level;
                await _context.SaveChangesAsync();

                break; // Only create one alert per check
            }
        }

        return true;
    }

    public async Task<bool> CheckAllBudgetAlertsAsync()
    {
        var budgets = await GetCurrentBudgetsAsync();
        
        foreach (var budget in budgets)
        {
            await CheckAndCreateBudgetAlertsAsync(budget.Id);
        }

        return true;
    }

    public async Task<bool> DismissBudgetAlertAsync(string alertId)
    {
        var alert = await _context.BudgetAlerts.FindAsync(alertId);
        if (alert == null)
            return false;

        alert.IsRead = true;
        alert.ReadDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    #endregion

    #region Budget Templates

    public async Task<Budget> CreateBudgetFromTemplateAsync(string templateId, DateTime startDate)
    {
        var template = await GetBudgetByIdAsync(templateId);
        if (template == null)
            throw new InvalidOperationException("Template not found");

        var newBudget = new Budget
        {
            Name = template.Name,
            Description = template.Description,
            BudgetType = template.BudgetType,
            Period = template.Period,
            LimitAmount = template.LimitAmount,
            StartDate = startDate,
            EndDate = await GetNextPeriodEndDateAsync(template),
            IsActive = true,
            AllowOverspend = template.AllowOverspend,
            EnableRollover = template.EnableRollover,
            CategoryId = template.CategoryId,
            AccountId = template.AccountId,
            Tag = template.Tag,
            AlertLevels = template.AlertLevels,
            AlertsEnabled = template.AlertsEnabled
        };

        return await CreateBudgetAsync(newBudget);
    }

    public async Task<Budget> SaveBudgetAsTemplateAsync(string budgetId, string templateName)
    {
        var budget = await GetBudgetByIdAsync(budgetId);
        if (budget == null)
            throw new InvalidOperationException("Budget not found");

        var template = new Budget
        {
            Name = templateName,
            Description = $"Template based on {budget.Name}",
            BudgetType = budget.BudgetType,
            Period = budget.Period,
            LimitAmount = budget.LimitAmount,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(30),
            IsActive = false, // Templates are inactive
            AllowOverspend = budget.AllowOverspend,
            EnableRollover = budget.EnableRollover,
            CategoryId = budget.CategoryId,
            AccountId = budget.AccountId,
            Tag = budget.Tag,
            AlertLevels = budget.AlertLevels,
            AlertsEnabled = budget.AlertsEnabled
        };

        return await CreateBudgetAsync(template);
    }

    public async Task<IEnumerable<Budget>> GetBudgetTemplatesAsync()
    {
        return await _context.Budgets
            .Include(b => b.Category)
            .Include(b => b.Account)
            .Where(b => !b.IsActive) // Templates are inactive
            .OrderBy(b => b.Name)
            .ToListAsync();
    }

    public async Task<bool> DeleteBudgetTemplateAsync(string templateId)
    {
        return await DeleteBudgetAsync(templateId);
    }

    #endregion

    #region Budget Rollover

    public async Task<bool> ProcessBudgetRolloverAsync(string budgetId)
    {
        var budget = await GetBudgetByIdAsync(budgetId);
        if (budget == null || !budget.EnableRollover)
            return false;

        var rolloverAmount = await CalculateRolloverAmountAsync(budgetId);
        if (rolloverAmount <= 0)
            return false;

        // Create next period budget
        var nextBudget = await CreateNextPeriodBudgetAsync(budgetId);
        if (nextBudget)
        {
            var newBudget = await _context.Budgets
                .Where(b => b.Name == budget.Name && 
                           b.StartDate > budget.EndDate)
                .OrderBy(b => b.StartDate)
                .FirstOrDefaultAsync();

            if (newBudget != null)
            {
                newBudget.RolloverAmount = rolloverAmount;
                await _context.SaveChangesAsync();
            }
        }

        return true;
    }

    public async Task<bool> ProcessAllBudgetRolloversAsync()
    {
        var expiredBudgets = await _context.Budgets
            .Where(b => b.IsActive && b.EnableRollover && b.EndDate < DateTime.Today)
            .ToListAsync();

        foreach (var budget in expiredBudgets)
        {
            await ProcessBudgetRolloverAsync(budget.Id);
        }

        return true;
    }

    public async Task<decimal> CalculateRolloverAmountAsync(string budgetId)
    {
        var remainingAmount = await GetBudgetRemainingAmountAsync(budgetId);
        return Math.Max(0, remainingAmount);
    }

    #endregion

    #region Budget Analytics and Reporting

    public async Task<BudgetSummaryDto> GetBudgetSummaryAsync()
    {
        var currentBudgets = await GetCurrentBudgetsAsync();
        
        return new BudgetSummaryDto
        {
            TotalBudgets = currentBudgets.Count(),
            TotalBudgetAmount = currentBudgets.Sum(b => b.LimitAmount + b.RolloverAmount),
            TotalSpentAmount = currentBudgets.Sum(b => b.SpentAmount),
            OverBudgetCount = currentBudgets.Count(b => b.IsOverBudget),
            AverageUtilization = currentBudgets.Any() ? 
                currentBudgets.Average(b => b.UsedPercentage) : 0
        };
    }

    public async Task<BudgetSummaryDto> GetBudgetSummaryByPeriodAsync(DateTime startDate, DateTime endDate)
    {
        var budgets = await _context.Budgets
            .Where(b => b.StartDate <= endDate && b.EndDate >= startDate)
            .ToListAsync();

        return new BudgetSummaryDto
        {
            TotalBudgets = budgets.Count,
            TotalBudgetAmount = budgets.Sum(b => b.LimitAmount + b.RolloverAmount),
            TotalSpentAmount = budgets.Sum(b => b.SpentAmount),
            OverBudgetCount = budgets.Count(b => b.IsOverBudget),
            AverageUtilization = budgets.Any() ? 
                budgets.Average(b => b.UsedPercentage) : 0
        };
    }

    public async Task<IEnumerable<MonthlyBudgetPerformanceDto>> GetMonthlyBudgetPerformanceAsync(int year)
    {
        var performances = new List<MonthlyBudgetPerformanceDto>();

        for (int month = 1; month <= 12; month++)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var budgets = await _context.Budgets
                .Where(b => b.StartDate <= endDate && b.EndDate >= startDate)
                .ToListAsync();

            var budgetedAmount = budgets.Sum(b => b.LimitAmount + b.RolloverAmount);
            var actualAmount = budgets.Sum(b => b.SpentAmount);

            performances.Add(new MonthlyBudgetPerformanceDto
            {
                Year = year,
                Month = month,
                MonthName = startDate.ToString("MMMM"),
                BudgetedAmount = budgetedAmount,
                ActualAmount = actualAmount,
                Variance = budgetedAmount - actualAmount,
                VariancePercentage = budgetedAmount > 0 ? 
                    ((budgetedAmount - actualAmount) / budgetedAmount) * 100 : 0,
                IsOverBudget = actualAmount > budgetedAmount
            });
        }

        return performances;
    }

    public async Task<BudgetVarianceReportDto> GetBudgetVarianceReportAsync(DateTime startDate, DateTime endDate)
    {
        var budgets = await _context.Budgets
            .Include(b => b.Category)
            .Where(b => b.StartDate <= endDate && b.EndDate >= startDate)
            .ToListAsync();

        var variances = budgets.Select(b => new BudgetVarianceItem
        {
            CategoryName = b.Category?.Name ?? b.Name,
            BudgetedAmount = b.LimitAmount + b.RolloverAmount,
            ActualAmount = b.SpentAmount,
            Variance = (b.LimitAmount + b.RolloverAmount) - b.SpentAmount,
            VariancePercentage = (b.LimitAmount + b.RolloverAmount) > 0 ?
                (((b.LimitAmount + b.RolloverAmount) - b.SpentAmount) / (b.LimitAmount + b.RolloverAmount)) * 100 : 0,
            IsFavorable = b.SpentAmount <= (b.LimitAmount + b.RolloverAmount),
            VarianceReason = b.SpentAmount > (b.LimitAmount + b.RolloverAmount) ? "Over budget" : "Under budget"
        }).ToList();

        return new BudgetVarianceReportDto
        {
            StartDate = startDate,
            EndDate = endDate,
            TotalBudgetedAmount = variances.Sum(v => v.BudgetedAmount),
            TotalActualAmount = variances.Sum(v => v.ActualAmount),
            TotalVariance = variances.Sum(v => v.Variance),
            FavorableVariances = variances.Where(v => v.IsFavorable).ToList(),
            UnfavorableVariances = variances.Where(v => !v.IsFavorable).ToList()
        };
    }

    public async Task<IEnumerable<Budget>> GetBudgetPerformanceHistoryAsync(string budgetId, int months = 12)
    {
        var budget = await GetBudgetByIdAsync(budgetId);
        if (budget == null)
            return new List<Budget>();

        // This would typically return historical budget data
        // For now, return the current budget
        return new List<Budget> { budget };
    }

    #endregion

    #region Budget Validation

    public async Task<bool> ValidateBudgetAsync(Budget budget)
    {
        if (budget == null)
            return false;

        // Check required fields
        if (string.IsNullOrWhiteSpace(budget.Name))
            return false;

        if (budget.LimitAmount <= 0)
            return false;

        if (budget.StartDate >= budget.EndDate)
            return false;

        // Check budget type specific validations
        switch (budget.BudgetType)
        {
            case BudgetType.Category:
                if (string.IsNullOrWhiteSpace(budget.CategoryId))
                    return false;
                break;

            case BudgetType.Account:
                if (string.IsNullOrWhiteSpace(budget.AccountId))
                    return false;
                break;

            case BudgetType.Tag:
                if (string.IsNullOrWhiteSpace(budget.Tag))
                    return false;
                break;
        }

        // Check name uniqueness
        if (!await IsBudgetNameUniqueAsync(budget.Name, budget.Id))
            return false;

        return true;
    }

    public async Task<bool> CanDeleteBudgetAsync(string budgetId)
    {
        // Check if budget has any alerts or related data
        var hasAlerts = await _context.BudgetAlerts
            .AnyAsync(ba => ba.BudgetId == budgetId);

        // For now, allow deletion even with alerts
        return true;
    }

    public async Task<bool> IsBudgetNameUniqueAsync(string name, string? excludeBudgetId = null)
    {
        var query = _context.Budgets.Where(b => b.Name == name);
        
        if (!string.IsNullOrEmpty(excludeBudgetId))
        {
            query = query.Where(b => b.Id != excludeBudgetId);
        }

        return !await query.AnyAsync();
    }

    #endregion

    #region Transaction Integration

    public async Task<bool> ProcessTransactionForBudgetsAsync(Transaction transaction)
    {
        if (transaction == null || transaction.Type != TransactionType.Expense)
            return false;

        var affectedBudgets = await GetAffectedBudgetsAsync(transaction);
        
        foreach (var budget in affectedBudgets)
        {
            await UpdateBudgetSpentAmountAsync(budget.Id, transaction.Amount);
        }

        return true;
    }

    public async Task<bool> RemoveTransactionFromBudgetsAsync(Transaction transaction)
    {
        if (transaction == null || transaction.Type != TransactionType.Expense)
            return false;

        var affectedBudgets = await GetAffectedBudgetsAsync(transaction);
        
        foreach (var budget in affectedBudgets)
        {
            await UpdateBudgetSpentAmountAsync(budget.Id, -transaction.Amount);
        }

        return true;
    }

    public async Task<IEnumerable<Budget>> GetAffectedBudgetsAsync(Transaction transaction)
    {
        var budgets = new List<Budget>();

        // Category-based budgets
        if (!string.IsNullOrEmpty(transaction.CategoryId))
        {
            var categoryBudgets = await GetBudgetsByCategoryAsync(transaction.CategoryId);
            budgets.AddRange(categoryBudgets.Where(b => b.IsCurrentlyActive));
        }

        // Account-based budgets
        if (!string.IsNullOrEmpty(transaction.AccountId))
        {
            var accountBudgets = await GetBudgetsByAccountAsync(transaction.AccountId);
            budgets.AddRange(accountBudgets.Where(b => b.IsCurrentlyActive));
        }

        // Tag-based budgets
        if (!string.IsNullOrEmpty(transaction.Tags))
        {
            var tags = transaction.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var tag in tags)
            {
                var tagBudgets = await GetBudgetsByTagAsync(tag.Trim());
                budgets.AddRange(tagBudgets.Where(b => b.IsCurrentlyActive));
            }
        }

        // Time period budgets
        var timePeriodBudgets = await GetBudgetsByTypeAsync(BudgetType.TimePeriod);
        budgets.AddRange(timePeriodBudgets.Where(b => b.IsCurrentlyActive &&
            transaction.Date >= b.StartDate &&
                transaction.Date <= b.EndDate));

        return budgets.Distinct();
    }

    #endregion

    #region Budget Period Management

    public async Task<bool> CreateNextPeriodBudgetAsync(string budgetId)
    {
        var budget = await GetBudgetByIdAsync(budgetId);
        if (budget == null)
            return false;

        var nextStartDate = await GetNextPeriodStartDateAsync(budget);
        var nextEndDate = await GetNextPeriodEndDateAsync(budget);

        var nextBudget = new Budget
        {
            Name = budget.Name,
            Description = budget.Description,
            BudgetType = budget.BudgetType,
            Period = budget.Period,
            LimitAmount = budget.LimitAmount,
            StartDate = nextStartDate,
            EndDate = nextEndDate,
            IsActive = true,
            AllowOverspend = budget.AllowOverspend,
            EnableRollover = budget.EnableRollover,
            CategoryId = budget.CategoryId,
            AccountId = budget.AccountId,
            Tag = budget.Tag,
            AlertLevels = budget.AlertLevels,
            AlertsEnabled = budget.AlertsEnabled
        };

        await CreateBudgetAsync(nextBudget);
        return true;
    }

    public async Task<bool> ArchiveExpiredBudgetsAsync()
    {
        var expiredBudgets = await _context.Budgets
            .Where(b => b.IsActive && b.EndDate < DateTime.Today)
            .ToListAsync();

        foreach (var budget in expiredBudgets)
        {
            budget.IsActive = false;
            budget.MarkAsDirty();
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<DateTime> GetNextPeriodStartDateAsync(Budget budget)
    {
        return budget.Period switch
        {
            BudgetPeriod.Weekly => budget.EndDate.AddDays(1),
            BudgetPeriod.Monthly => budget.EndDate.AddDays(1),
            BudgetPeriod.Quarterly => budget.EndDate.AddDays(1),
            BudgetPeriod.Yearly => budget.EndDate.AddDays(1),
            _ => budget.EndDate.AddDays(1)
        };
    }

    public async Task<DateTime> GetNextPeriodEndDateAsync(Budget budget)
    {
        var startDate = await GetNextPeriodStartDateAsync(budget);
        
        return budget.Period switch
        {
            BudgetPeriod.Weekly => startDate.AddDays(6),
            BudgetPeriod.Monthly => startDate.AddMonths(1).AddDays(-1),
            BudgetPeriod.Quarterly => startDate.AddMonths(3).AddDays(-1),
            BudgetPeriod.Yearly => startDate.AddYears(1).AddDays(-1),
            _ => startDate.AddMonths(1).AddDays(-1)
        };
    }

    #endregion

    #region Dashboard Data

    public async Task<object> GetBudgetDashboardDataAsync()
    {
        var summary = await GetBudgetSummaryAsync();
        var currentBudgets = await GetCurrentBudgetsAsync();
        var alerts = await GetActiveBudgetAlertsAsync();

        return new
        {
            Summary = summary,
            CurrentBudgets = currentBudgets.Take(5),
            RecentAlerts = alerts.Take(5),
            OverBudgetCount = currentBudgets.Count(b => b.IsOverBudget),
            NearLimitCount = currentBudgets.Count(b => b.UsedPercentage >= 75 && !b.IsOverBudget)
        };
    }

    public async Task<object> GetBudgetProgressDataAsync()
    {
        var currentBudgets = await GetCurrentBudgetsAsync();
        
        return currentBudgets.Select(b => new
        {
            b.Id,
            b.Name,
            b.BudgetTypeDisplay,
            b.FormattedLimitAmount,
            b.FormattedSpentAmount,
            b.FormattedRemainingAmount,
            b.UsedPercentage,
            b.IsOverBudget,
            CategoryName = b.Category?.Name,
            AccountName = b.Account?.Name
        });
    }

    public async Task<object> GetBudgetAlertsDataAsync()
    {
        var alerts = await GetActiveBudgetAlertsAsync();
        
        return alerts.Select(a => new
        {
            a.Id,
            a.Message,
            a.Severity,
            a.UsedPercentage,
            a.CreatedAt,
            BudgetName = a.Budget?.Name
        });
    }

    public async Task<BudgetAnalyticsData> GetBudgetAnalyticsAsync(DateTime startDate, DateTime endDate)
    {
        var budgets = await _context.Budgets
            .Where(b => b.StartDate <= endDate && b.EndDate >= startDate)
            .ToListAsync();

        var totalBudgets = budgets.Count;
        var averageUtilization = budgets.Any() ? budgets.Average(b => b.UsedPercentage) : 0;
        var overBudgetCount = budgets.Count(b => b.IsOverBudget);
        var totalVariance = budgets.Sum(b => (b.LimitAmount + b.RolloverAmount) - b.SpentAmount);

        return new BudgetAnalyticsData
        {
            TotalBudgets = totalBudgets,
            AverageUtilization = averageUtilization,
            OverBudgetCount = overBudgetCount,
            TotalVariance = totalVariance
        };
    }

    public async Task<List<BudgetAnalysisDto>> GetBudgetAnalysisAsync(DateTime startDate, DateTime endDate)
    {
        var budgets = await _context.Budgets
            .Include(b => b.Category)
            .Include(b => b.Account)
            .Where(b => b.StartDate <= endDate && b.EndDate >= startDate)
            .ToListAsync();

        return budgets.Select(b => new BudgetAnalysisDto
        {
            BudgetId = b.Id,
            BudgetName = b.Name,
            BudgetType = b.BudgetType.ToString(),
            StartDate = b.StartDate,
            EndDate = b.EndDate,
            BudgetedAmount = b.LimitAmount,
            SpentAmount = b.SpentAmount,
            RemainingAmount = b.RemainingAmount,
            Variance = b.LimitAmount - b.SpentAmount,
            UtilizationPercentage = b.UsedPercentage,
            IsOverBudget = b.IsOverBudget,
            TransactionCount = b.RelatedTransactions?.Count ?? 0,
            CategoryName = b.Category?.Name ?? string.Empty,
            AccountName = b.Account?.Name ?? string.Empty,
            Tags = b.Tag ?? string.Empty
        }).ToList();
    }

    public async Task<BudgetPerformanceDto> GetBudgetPerformanceAsync(string budgetId, DateTime startDate, DateTime endDate)
    {
        var budget = await _context.Budgets
            .Include(b => b.RelatedTransactions)
            .FirstOrDefaultAsync(b => b.Id == budgetId);

        if (budget == null)
            throw new ArgumentException($"Budget with ID {budgetId} not found.");

        var transactions = budget.RelatedTransactions?
            .Where(t => t.Date >= startDate && t.Date <= endDate)
            .ToList() ?? new List<Transaction>();

        var actualAmount = transactions.Sum(t => t.Amount);
        var variance = budget.LimitAmount - actualAmount;
        var variancePercentage = budget.LimitAmount > 0 ? (variance / budget.LimitAmount) * 100 : 0;
        var utilizationPercentage = budget.LimitAmount > 0 ? (actualAmount / budget.LimitAmount) * 100 : 0;

        var performanceRating = utilizationPercentage switch
        {
            <= 75 => "Excellent",
            <= 90 => "Good",
            <= 100 => "Fair",
            _ => "Poor"
        };

        var recommendations = new List<string>();
        if (utilizationPercentage > 100)
            recommendations.Add("Consider reducing spending or increasing budget limit");
        if (utilizationPercentage < 50)
            recommendations.Add("Budget may be too high for this category");

        return new BudgetPerformanceDto
        {
            BudgetId = budget.Id,
            BudgetName = budget.Name,
            StartDate = startDate,
            EndDate = endDate,
            BudgetedAmount = budget.LimitAmount,
            ActualAmount = actualAmount,
            Variance = variance,
            VariancePercentage = variancePercentage,
            UtilizationPercentage = utilizationPercentage,
            IsOverBudget = actualAmount > budget.LimitAmount,
            IsOnTrack = utilizationPercentage <= 100,
            TotalTransactions = transactions.Count,
            AverageTransactionAmount = transactions.Any() ? transactions.Average(t => t.Amount) : 0,
            LastTransactionDate = transactions.Any() ? transactions.Max(t => t.Date) : DateTime.MinValue,
            PerformanceRating = performanceRating,
            Recommendations = recommendations
        };
    }

    public async Task<List<BudgetVarianceDto>> GetBudgetVarianceAnalysisAsync(DateTime startDate, DateTime endDate)
    {
        var budgets = await _context.Budgets
            .Include(b => b.Category)
            .Include(b => b.Account)
            .Include(b => b.RelatedTransactions)
            .Where(b => b.StartDate <= endDate && b.EndDate >= startDate)
            .ToListAsync();

        return budgets.Select(b =>
        {
            var transactions = b.RelatedTransactions?
                .Where(t => t.Date >= startDate && t.Date <= endDate)
                .ToList() ?? new List<Transaction>();

            var actualAmount = transactions.Sum(t => t.Amount);
            var variance = b.LimitAmount - actualAmount;
            var variancePercentage = b.LimitAmount > 0 ? (variance / b.LimitAmount) * 100 : 0;

            var varianceCategory = Math.Abs(variancePercentage) switch
            {
                <= 10 => "Minor",
                <= 25 => "Moderate",
                _ => "Significant"
            };

            var recommendedActions = new List<string>();
            if (variance < 0)
            {
                recommendedActions.Add("Review spending patterns");
                recommendedActions.Add("Consider budget adjustment");
            }
            else if (variance > b.LimitAmount * 0.5m)
            {
                recommendedActions.Add("Budget may be too conservative");
            }

            return new BudgetVarianceDto
            {
                BudgetId = b.Id,
                BudgetName = b.Name,
                CategoryName = b.Category?.Name ?? string.Empty,
                AccountName = b.Account?.Name ?? string.Empty,
                AnalysisDate = DateTime.Now,
                PeriodStart = startDate,
                PeriodEnd = endDate,
                BudgetedAmount = b.LimitAmount,
                ActualAmount = actualAmount,
                Variance = variance,
                VariancePercentage = variancePercentage,
                IsFavorable = variance >= 0,
                VarianceType = variance >= 0 ? "Favorable" : "Unfavorable",
                VarianceCategory = varianceCategory,
                Reason = variance < 0 ? "Overspending detected" : "Under budget",
                Impact = varianceCategory == "Significant" ? "High impact on financial goals" : "Low to moderate impact",
                RecommendedActions = recommendedActions,
                TransactionCount = transactions.Count,
                AverageTransactionAmount = transactions.Any() ? transactions.Average(t => t.Amount) : 0
            };
        }).ToList();
    }

    #endregion
}
