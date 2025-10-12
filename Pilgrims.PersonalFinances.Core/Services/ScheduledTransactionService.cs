using Microsoft.EntityFrameworkCore;
using Pilgrims.PersonalFinances.Data;
using Pilgrims.PersonalFinances.Core.Models;
using Pilgrims.PersonalFinances.Core.Models.Enums;
using Pilgrims.PersonalFinances.Core.Services.Interfaces;
using Pilgrims.PersonalFinances.Core.Interfaces;
using System.Text.Json;

namespace Pilgrims.PersonalFinances.Core.Services;

/// <summary>
/// Service implementation for managing scheduled transactions with comprehensive CRUD operations
/// </summary>
public class ScheduledTransactionService : IScheduledTransactionService
{
    private readonly PersonalFinanceContext _context;
    private readonly ITransactionService _transactionService;
    private readonly ICurrencyService _currencyService;

    public ScheduledTransactionService(PersonalFinanceContext context, ITransactionService transactionService, ICurrencyService currencyService)
    {
        _context = context;
        _transactionService = transactionService;
        _currencyService = currencyService;
    }

    #region Basic CRUD Operations

    public async Task<IEnumerable<ScheduledTransaction>> GetAllScheduledTransactionsAsync()
    {
        var scheduledTransactions = await _context.ScheduledTransactions
            .Include(st => st.Account)
            .Include(st => st.Category)
            .Include(st => st.TransferToAccount)
            .OrderBy(st => st.Name)
            .ToListAsync();

        foreach (var scheduledTransaction in scheduledTransactions)
        {
            scheduledTransaction.FormattedAmount = await _currencyService.FormatAmountAsync(scheduledTransaction.Amount);
        }

        return scheduledTransactions;
    }

    public async Task<ScheduledTransaction?> GetScheduledTransactionByIdAsync(string id)
    {
        var scheduledTransaction = await _context.ScheduledTransactions
            .Include(st => st.Account)
            .Include(st => st.Category)
            .Include(st => st.TransferToAccount)
            .Include(st => st.GeneratedTransactions)
            .FirstOrDefaultAsync(st => st.Id == id);

        if (scheduledTransaction != null)
        {
            scheduledTransaction.FormattedAmount = await _currencyService.FormatAmountAsync(scheduledTransaction.Amount);
        }

        return scheduledTransaction;
    }

    public async Task<ScheduledTransaction> CreateScheduledTransactionAsync(ScheduledTransaction scheduledTransaction)
    {
        // Validate the scheduled transaction
        var validationErrors = await ValidateScheduledTransactionAsync(scheduledTransaction);
        if (validationErrors.Any())
        {
            throw new ArgumentException($"Validation failed: {string.Join(", ", validationErrors)}");
        }

        // Set initial values
        scheduledTransaction.Id = Guid.NewGuid().ToString();
        scheduledTransaction.CreatedAt = DateTime.UtcNow;
        scheduledTransaction.NextDueDate = scheduledTransaction.CalculateNextDueDate();

        _context.ScheduledTransactions.Add(scheduledTransaction);
        await _context.SaveChangesAsync();

        return scheduledTransaction;
    }

    public async Task<ScheduledTransaction> UpdateScheduledTransactionAsync(ScheduledTransaction scheduledTransaction)
    {
        var existingTransaction = await GetScheduledTransactionByIdAsync(scheduledTransaction.Id);
        if (existingTransaction == null)
        {
            throw new ArgumentException($"Scheduled transaction with ID {scheduledTransaction.Id} not found");
        }

        // Validate the updated scheduled transaction
        var validationErrors = await ValidateScheduledTransactionAsync(scheduledTransaction);
        if (validationErrors.Any())
        {
            throw new ArgumentException($"Validation failed: {string.Join(", ", validationErrors)}");
        }

        // Update properties
        _context.Entry(existingTransaction).CurrentValues.SetValues(scheduledTransaction);
        existingTransaction.NextDueDate = existingTransaction.CalculateNextDueDate();
        existingTransaction.MarkAsDirty();

        await _context.SaveChangesAsync();
        return existingTransaction;
    }

    public async Task<bool> DeleteScheduledTransactionAsync(string id)
    {
        var scheduledTransaction = await GetScheduledTransactionByIdAsync(id);
        if (scheduledTransaction == null) return false;

        _context.ScheduledTransactions.Remove(scheduledTransaction);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteScheduledTransactionsAsync(IEnumerable<string> ids)
    {
        var scheduledTransactions = await _context.ScheduledTransactions
            .Where(st => ids.Contains(st.Id))
            .ToListAsync();

        if (!scheduledTransactions.Any()) return false;

        _context.ScheduledTransactions.RemoveRange(scheduledTransactions);
        await _context.SaveChangesAsync();
        return true;
    }

    #endregion

    #region Filtering and Search

    public async Task<IEnumerable<ScheduledTransaction>> GetActiveScheduledTransactionsAsync()
    {
        return await _context.ScheduledTransactions
            .Include(st => st.Account)
            .Include(st => st.Category)
            .Include(st => st.TransferToAccount)
            .Where(st => st.IsActive)
            .OrderBy(st => st.NextDueDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<ScheduledTransaction>> GetScheduledTransactionsByAccountAsync(string accountId)
    {
        return await _context.ScheduledTransactions
            .Include(st => st.Account)
            .Include(st => st.Category)
            .Include(st => st.TransferToAccount)
            .Where(st => st.AccountId == accountId)
            .OrderBy(st => st.NextDueDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<ScheduledTransaction>> GetScheduledTransactionsByCategoryAsync(string categoryId)
    {
        return await _context.ScheduledTransactions
            .Include(st => st.Account)
            .Include(st => st.Category)
            .Include(st => st.TransferToAccount)
            .Where(st => st.CategoryId == categoryId)
            .OrderBy(st => st.NextDueDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<ScheduledTransaction>> GetScheduledTransactionsByTypeAsync(TransactionType type)
    {
        return await _context.ScheduledTransactions
            .Include(st => st.Account)
            .Include(st => st.Category)
            .Include(st => st.TransferToAccount)
            .Where(st => st.TransactionType == type)
            .OrderBy(st => st.NextDueDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<ScheduledTransaction>> SearchScheduledTransactionsAsync(string searchTerm)
    {
        var lowerSearchTerm = searchTerm.ToLower();
        return await _context.ScheduledTransactions
            .Include(st => st.Account)
            .Include(st => st.Category)
            .Include(st => st.TransferToAccount)
            .Where(st => st.Name.ToLower().Contains(lowerSearchTerm) ||
                        (st.Description != null && st.Description.ToLower().Contains(lowerSearchTerm)) ||
                        (st.Payee != null && st.Payee.ToLower().Contains(lowerSearchTerm)))
            .OrderBy(st => st.NextDueDate)
            .ToListAsync();
    }

    #endregion

    #region Due Date Management

    public async Task<IEnumerable<ScheduledTransaction>> GetDueScheduledTransactionsAsync(DateTime? asOfDate = null)
    {
        var targetDate = asOfDate ?? DateTime.Today;
        return await _context.ScheduledTransactions
            .Include(st => st.Account)
            .Include(st => st.Category)
            .Include(st => st.TransferToAccount)
            .Where(st => st.IsActive && st.NextDueDate.HasValue && st.NextDueDate.Value.Date <= targetDate)
            .OrderBy(st => st.NextDueDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<ScheduledTransaction>> GetOverdueScheduledTransactionsAsync()
    {
        return await _context.ScheduledTransactions
            .Include(st => st.Account)
            .Include(st => st.Category)
            .Include(st => st.TransferToAccount)
            .Where(st => st.IsActive && st.NextDueDate.HasValue && st.NextDueDate.Value.Date < DateTime.Today)
            .OrderBy(st => st.NextDueDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<ScheduledTransaction>> GetUpcomingScheduledTransactionsAsync(int daysAhead = 7)
    {
        var endDate = DateTime.Today.AddDays(daysAhead);
        return await _context.ScheduledTransactions
            .Include(st => st.Account)
            .Include(st => st.Category)
            .Include(st => st.TransferToAccount)
            .Where(st => st.IsActive && st.NextDueDate.HasValue && 
                        st.NextDueDate.Value.Date > DateTime.Today && 
                        st.NextDueDate.Value.Date <= endDate)
            .OrderBy(st => st.NextDueDate)
            .ToListAsync();
    }

    #endregion

    #region Transaction Processing

    public async Task<Transaction> ProcessScheduledTransactionAsync(string scheduledTransactionId, DateTime? processDate = null)
    {
        var scheduledTransaction = await GetScheduledTransactionByIdAsync(scheduledTransactionId);
        if (scheduledTransaction == null)
        {
            throw new ArgumentException($"Scheduled transaction with ID {scheduledTransactionId} not found");
        }

        var transactionDate = processDate ?? scheduledTransaction.NextDueDate ?? DateTime.Today;

        // Check if this date should be skipped
        var skippedDates = scheduledTransaction.SkippedDatesList;
        if (skippedDates.Contains(transactionDate.Date))
        {
            throw new InvalidOperationException($"Transaction date {transactionDate:yyyy-MM-dd} is marked as skipped");
        }

        // Create the transaction
        var transaction = scheduledTransaction.CreateTransaction(transactionDate);
        
        // Set status based on scheduling mode
        transaction.Status = scheduledTransaction.SchedulingMode == SchedulingMode.AutoPost 
            ? TransactionStatus.Cleared 
            : TransactionStatus.Pending;

        // Save the transaction
        var createdTransaction = await _transactionService.CreateTransactionAsync(transaction);

        // Update the scheduled transaction
        scheduledTransaction.MarkAsGenerated(transactionDate);
        await _context.SaveChangesAsync();

        // Create notification for successful processing
        var notification = TransactionNotification.CreateTransactionCreatedNotification(scheduledTransaction, createdTransaction);
        _context.TransactionNotifications.Add(notification);
        await _context.SaveChangesAsync();

        return createdTransaction;
    }

    public async Task<IEnumerable<Transaction>> ProcessDueScheduledTransactionsAsync(DateTime? asOfDate = null)
    {
        var dueTransactions = await GetDueScheduledTransactionsAsync(asOfDate);
        var processedTransactions = new List<Transaction>();

        foreach (var scheduledTransaction in dueTransactions)
        {
            try
            {
                if (scheduledTransaction.SchedulingMode == SchedulingMode.AutoPost)
                {
                    var transaction = await ProcessScheduledTransactionAsync(scheduledTransaction.Id, scheduledTransaction.NextDueDate);
                    processedTransactions.Add(transaction);
                }
                else if (scheduledTransaction.SchedulingMode == SchedulingMode.ManualApproval)
                {
                    // Create approval notification
                    await CreateApprovalNotificationAsync(scheduledTransaction.Id);
                }
            }
            catch (Exception ex)
            {
                // Log error and create error notification
                var errorNotification = TransactionNotification.CreateErrorNotification(scheduledTransaction, ex.Message);
                _context.TransactionNotifications.Add(errorNotification);
            }
        }

        await _context.SaveChangesAsync();
        return processedTransactions;
    }

    public async Task<bool> SkipScheduledTransactionOccurrenceAsync(string scheduledTransactionId, DateTime skipDate)
    {
        var scheduledTransaction = await GetScheduledTransactionByIdAsync(scheduledTransactionId);
        if (scheduledTransaction == null) return false;

        scheduledTransaction.SkipOccurrence(skipDate);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UnskipScheduledTransactionOccurrenceAsync(string scheduledTransactionId, DateTime unskipDate)
    {
        var scheduledTransaction = await GetScheduledTransactionByIdAsync(scheduledTransactionId);
        if (scheduledTransaction == null) return false;

        scheduledTransaction.UnskipOccurrence(unskipDate);
        await _context.SaveChangesAsync();
        return true;
    }

    #endregion

    #region Notification Management

    public async Task<IEnumerable<TransactionNotification>> GetNotificationsForScheduledTransactionAsync(string scheduledTransactionId)
    {
        return await _context.TransactionNotifications
            .Where(tn => tn.ScheduledTransactionId == scheduledTransactionId)
            .OrderByDescending(tn => tn.ScheduledDate)
            .ToListAsync();
    }

    public async Task<TransactionNotification> CreateReminderNotificationAsync(string scheduledTransactionId, DateTime reminderDate)
    {
        var scheduledTransaction = await GetScheduledTransactionByIdAsync(scheduledTransactionId);
        if (scheduledTransaction == null)
        {
            throw new ArgumentException($"Scheduled transaction with ID {scheduledTransactionId} not found");
        }

        var notification = TransactionNotification.CreateReminder(scheduledTransaction, reminderDate);
        _context.TransactionNotifications.Add(notification);
        await _context.SaveChangesAsync();

        return notification;
    }

    public async Task<TransactionNotification> CreateOverdueNotificationAsync(string scheduledTransactionId)
    {
        var scheduledTransaction = await GetScheduledTransactionByIdAsync(scheduledTransactionId);
        if (scheduledTransaction == null)
        {
            throw new ArgumentException($"Scheduled transaction with ID {scheduledTransactionId} not found");
        }

        var notification = TransactionNotification.CreateOverdueAlert(scheduledTransaction);
        _context.TransactionNotifications.Add(notification);
        await _context.SaveChangesAsync();

        return notification;
    }

    public async Task<TransactionNotification> CreateApprovalNotificationAsync(string scheduledTransactionId)
    {
        var scheduledTransaction = await GetScheduledTransactionByIdAsync(scheduledTransactionId);
        if (scheduledTransaction == null)
        {
            throw new ArgumentException($"Scheduled transaction with ID {scheduledTransactionId} not found");
        }

        var notification = TransactionNotification.CreateApprovalRequest(scheduledTransaction);
        _context.TransactionNotifications.Add(notification);
        await _context.SaveChangesAsync();

        return notification;
    }

    public async Task ProcessScheduledNotificationsAsync()
    {
        var activeScheduledTransactions = await GetActiveScheduledTransactionsAsync();
        var today = DateTime.Today;

        foreach (var scheduledTransaction in activeScheduledTransactions)
        {
            if (!scheduledTransaction.NextDueDate.HasValue) continue;

            var dueDate = scheduledTransaction.NextDueDate.Value.Date;
            
            // Check for overdue notifications
            if (dueDate < today)
            {
                var existingOverdueNotification = await _context.TransactionNotifications
                    .AnyAsync(tn => tn.ScheduledTransactionId == scheduledTransaction.Id &&
                                   tn.NotificationType == Pilgrims.PersonalFinances.Core.Models.Enums.AppNotificationType.BudgetAlert &&
                                   !tn.IsSent);

                if (!existingOverdueNotification)
                {
                    await CreateOverdueNotificationAsync(scheduledTransaction.Id);
                }
            }
            else
            {
                // Check for reminder notifications based on timing
                var reminderDates = GetReminderDates(dueDate, scheduledTransaction.NotificationTiming);
                
                foreach (var reminderDate in reminderDates)
                {
                    if (reminderDate.Date == today)
                    {
                        var existingReminder = await _context.TransactionNotifications
                            .AnyAsync(tn => tn.ScheduledTransactionId == scheduledTransaction.Id &&
                                           tn.NotificationType == Pilgrims.PersonalFinances.Core.Models.Enums.AppNotificationType.BillReminder &&
                                           tn.ScheduledDate.Date == reminderDate.Date);

                        if (!existingReminder)
                        {
                            await CreateReminderNotificationAsync(scheduledTransaction.Id, reminderDate);
                        }
                    }
                }
            }
        }
    }

    #endregion

    #region Bulk Operations

    public async Task<bool> BulkActivateScheduledTransactionsAsync(IEnumerable<string> scheduledTransactionIds)
    {
        var scheduledTransactions = await _context.ScheduledTransactions
            .Where(st => scheduledTransactionIds.Contains(st.Id))
            .ToListAsync();

        if (!scheduledTransactions.Any()) return false;

        foreach (var scheduledTransaction in scheduledTransactions)
        {
            scheduledTransaction.IsActive = true;
            scheduledTransaction.NextDueDate = scheduledTransaction.CalculateNextDueDate();
            scheduledTransaction.MarkAsDirty();
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> BulkDeactivateScheduledTransactionsAsync(IEnumerable<string> scheduledTransactionIds)
    {
        var scheduledTransactions = await _context.ScheduledTransactions
            .Where(st => scheduledTransactionIds.Contains(st.Id))
            .ToListAsync();

        if (!scheduledTransactions.Any()) return false;

        foreach (var scheduledTransaction in scheduledTransactions)
        {
            scheduledTransaction.IsActive = false;
            scheduledTransaction.MarkAsDirty();
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> BulkUpdateScheduledTransactionCategoryAsync(IEnumerable<string> scheduledTransactionIds, string categoryId)
    {
        var scheduledTransactions = await _context.ScheduledTransactions
            .Where(st => scheduledTransactionIds.Contains(st.Id))
            .ToListAsync();

        if (!scheduledTransactions.Any()) return false;

        foreach (var scheduledTransaction in scheduledTransactions)
        {
            scheduledTransaction.CategoryId = categoryId;
            scheduledTransaction.MarkAsDirty();
        }

        await _context.SaveChangesAsync();
        return true;
    }

    #endregion

    #region Statistics and Analytics

    public async Task<decimal> GetProjectedMonthlyAmountAsync(string? accountId = null, string? categoryId = null)
    {
        var query = _context.ScheduledTransactions.Where(st => st.IsActive);

        if (!string.IsNullOrEmpty(accountId))
            query = query.Where(st => st.AccountId == accountId);

        if (!string.IsNullOrEmpty(categoryId))
            query = query.Where(st => st.CategoryId == categoryId);

        var scheduledTransactions = await query.ToListAsync();
        decimal totalMonthlyAmount = 0;

        foreach (var scheduledTransaction in scheduledTransactions)
        {
            var monthlyAmount = CalculateMonthlyAmount(scheduledTransaction);
            totalMonthlyAmount += monthlyAmount;
        }

        return totalMonthlyAmount;
    }

    public async Task<Dictionary<string, decimal>> GetProjectedAmountsByAccountAsync()
    {
        var scheduledTransactions = await GetActiveScheduledTransactionsAsync();
        var result = new Dictionary<string, decimal>();

        foreach (var scheduledTransaction in scheduledTransactions)
        {
            var monthlyAmount = CalculateMonthlyAmount(scheduledTransaction);
            var accountName = scheduledTransaction.Account?.Name ?? "Unknown Account";
            
            if (result.ContainsKey(accountName))
                result[accountName] += monthlyAmount;
            else
                result[accountName] = monthlyAmount;
        }

        return result;
    }

    public async Task<Dictionary<string, decimal>> GetProjectedAmountsByCategoryAsync()
    {
        var scheduledTransactions = await GetActiveScheduledTransactionsAsync();
        var result = new Dictionary<string, decimal>();

        foreach (var scheduledTransaction in scheduledTransactions)
        {
            var monthlyAmount = CalculateMonthlyAmount(scheduledTransaction);
            var categoryName = scheduledTransaction.Category?.Name ?? "Uncategorized";
            
            if (result.ContainsKey(categoryName))
                result[categoryName] += monthlyAmount;
            else
                result[categoryName] = monthlyAmount;
        }

        return result;
    }

    public async Task<IEnumerable<ScheduledTransaction>> GetScheduledTransactionsByRecurrenceTypeAsync(RecurrenceType recurrenceType)
    {
        return await _context.ScheduledTransactions
            .Include(st => st.Account)
            .Include(st => st.Category)
            .Include(st => st.TransferToAccount)
            .Where(st => st.RecurrenceType == recurrenceType)
            .OrderBy(st => st.NextDueDate)
            .ToListAsync();
    }

    #endregion

    #region Validation and Calculation

    public async Task<List<string>> ValidateScheduledTransactionAsync(ScheduledTransaction scheduledTransaction)
    {
        var errors = scheduledTransaction.Validate();

        // Additional async validations
        if (!string.IsNullOrEmpty(scheduledTransaction.AccountId))
        {
            var account = await _context.Accounts.FindAsync(scheduledTransaction.AccountId);
            if (account == null)
                errors.Add("Selected account does not exist");
        }

        if (!string.IsNullOrEmpty(scheduledTransaction.CategoryId))
        {
            var category = await _context.Categories.FindAsync(scheduledTransaction.CategoryId);
            if (category == null)
                errors.Add("Selected category does not exist");
        }

        if (!string.IsNullOrEmpty(scheduledTransaction.TransferToAccountId))
        {
            var transferAccount = await _context.Accounts.FindAsync(scheduledTransaction.TransferToAccountId);
            if (transferAccount == null)
                errors.Add("Selected transfer account does not exist");
        }

        return errors;
    }

    public async Task<DateTime?> CalculateNextDueDateAsync(string scheduledTransactionId)
    {
        var scheduledTransaction = await GetScheduledTransactionByIdAsync(scheduledTransactionId);
        return scheduledTransaction?.CalculateNextDueDate();
    }

    public async Task<IEnumerable<DateTime>> GetProjectedDatesAsync(string scheduledTransactionId, int monthsAhead = 12)
    {
        var scheduledTransaction = await GetScheduledTransactionByIdAsync(scheduledTransactionId);
        if (scheduledTransaction == null) return Enumerable.Empty<DateTime>();

        var projectedDates = new List<DateTime>();
        var currentDate = scheduledTransaction.NextDueDate ?? scheduledTransaction.StartDate;
        var endDate = DateTime.Today.AddMonths(monthsAhead);

        while (currentDate <= endDate && projectedDates.Count < 100) // Safety limit
        {
            if (currentDate >= DateTime.Today)
            {
                projectedDates.Add(currentDate);
            }

            // Calculate next occurrence
            var tempTransaction = new ScheduledTransaction
            {
                RecurrenceType = scheduledTransaction.RecurrenceType,
                RecurrenceInterval = scheduledTransaction.RecurrenceInterval,
                DaysOfWeek = scheduledTransaction.DaysOfWeek,
                DayOfMonth = scheduledTransaction.DayOfMonth,
                AdjustForWeekends = scheduledTransaction.AdjustForWeekends,
                LastGeneratedDate = currentDate,
                EndType = RecurrenceEndType.Never // Ignore end conditions for projection
            };

            var nextDate = tempTransaction.CalculateNextDueDate();
            if (nextDate == null || nextDate <= currentDate) break;

            currentDate = nextDate.Value;
        }

        return projectedDates;
    }

    #endregion

    #region Private Helper Methods

    private decimal CalculateMonthlyAmount(ScheduledTransaction scheduledTransaction)
    {
        return scheduledTransaction.RecurrenceType switch
        {
            RecurrenceType.Daily => scheduledTransaction.Amount * 30 / scheduledTransaction.RecurrenceInterval,
            RecurrenceType.Weekly => scheduledTransaction.Amount * 4.33m / scheduledTransaction.RecurrenceInterval,
            RecurrenceType.BiWeekly => scheduledTransaction.Amount * 2.17m / scheduledTransaction.RecurrenceInterval,
            RecurrenceType.Monthly => scheduledTransaction.Amount / scheduledTransaction.RecurrenceInterval,
            RecurrenceType.Quarterly => scheduledTransaction.Amount / (3 * scheduledTransaction.RecurrenceInterval),
            RecurrenceType.SemiAnnually => scheduledTransaction.Amount / (6 * scheduledTransaction.RecurrenceInterval),
            RecurrenceType.Annually => scheduledTransaction.Amount / (12 * scheduledTransaction.RecurrenceInterval),
            _ => 0
        };
    }

    private List<DateTime> GetReminderDates(DateTime dueDate, NotificationTiming timing)
    {
        var reminderDates = new List<DateTime>();

        switch (timing)
        {
            case NotificationTiming.OneDayBefore:
                reminderDates.Add(dueDate.AddDays(-1));
                break;
            case NotificationTiming.ThreeDaysBefore:
                reminderDates.Add(dueDate.AddDays(-3));
                break;
            case NotificationTiming.OneWeekBefore:
                reminderDates.Add(dueDate.AddDays(-7));
                break;

        }

        return reminderDates.Where(d => d >= DateTime.Today).ToList();
    }

    #endregion

    #region Additional Interface Methods

    public async Task<IEnumerable<ScheduledTransaction>> GetDueTransactionsAsync(DateTime? asOfDate = null)
    {
        return await GetDueScheduledTransactionsAsync(asOfDate);
    }

    public async Task<IEnumerable<ScheduledTransaction>> GetOverdueTransactionsAsync()
    {
        return await GetOverdueScheduledTransactionsAsync();
    }

    public async Task<IEnumerable<ScheduledTransaction>> GetUpcomingTransactionsAsync(int daysAhead)
    {
        return await GetUpcomingScheduledTransactionsAsync(daysAhead);
    }

    public async Task<bool> MarkAsProcessedAsync(string scheduledTransactionId)
    {
        var scheduledTransaction = await GetScheduledTransactionByIdAsync(scheduledTransactionId);
        if (scheduledTransaction == null)
            return false;

        scheduledTransaction.LastProcessedDate = DateTime.UtcNow;
        scheduledTransaction.NextDueDate = scheduledTransaction.CalculateNextDueDate();
        
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Transaction> ProcessDueTransactionAsync(string scheduledTransactionId)
    {
        return await ProcessScheduledTransactionAsync(scheduledTransactionId);
    }

    public async Task<bool> PauseAsync(string scheduledTransactionId)
    {
        var scheduledTransaction = await GetScheduledTransactionByIdAsync(scheduledTransactionId);
        if (scheduledTransaction == null)
            return false;

        scheduledTransaction.IsActive = false;
        scheduledTransaction.MarkAsDirty();
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ResumeAsync(string scheduledTransactionId)
    {
        var scheduledTransaction = await GetScheduledTransactionByIdAsync(scheduledTransactionId);
        if (scheduledTransaction == null)
            return false;

        scheduledTransaction.IsActive = true;
        scheduledTransaction.NextDueDate = scheduledTransaction.CalculateNextDueDate();
        scheduledTransaction.MarkAsDirty();
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(string scheduledTransactionId)
    {
        return await DeleteScheduledTransactionAsync(scheduledTransactionId);
    }

    #endregion
}


