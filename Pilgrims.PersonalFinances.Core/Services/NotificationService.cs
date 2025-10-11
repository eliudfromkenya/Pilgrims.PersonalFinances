using Microsoft.EntityFrameworkCore;
using Pilgrims.PersonalFinances.Data;
using Pilgrims.PersonalFinances.Models;
using Pilgrims.PersonalFinances.Models.Enums;
using Pilgrims.PersonalFinances.Services.Interfaces;
using Pilgrims.PersonalFinances.Core.Logging;

namespace Pilgrims.PersonalFinances.Services;

public class NotificationService : INotificationService
{
    private readonly PersonalFinanceContext _context;
    private readonly IScheduledTransactionService _scheduledTransactionService;
    private readonly ILoggingService _logger;

    public NotificationService(PersonalFinanceContext context, IScheduledTransactionService scheduledTransactionService, ILoggingService logger)
    {
        _context = context;
        _scheduledTransactionService = scheduledTransactionService;
        _logger = logger;
    }

    public event EventHandler<TransactionNotification>? NotificationCreated;
    public event EventHandler<string>? NotificationRead;
    public event EventHandler<string>? NotificationDismissed;

    #region Notification Management

    public async Task<List<TransactionNotification>> GetAllNotificationsAsync()
    {
        return await _context.TransactionNotifications
            .Include(n => n.ScheduledTransaction)
            .OrderByDescending(n => n.ScheduledDate)
            .ToListAsync();
    }

    public async Task<List<TransactionNotification>> GetUnreadNotificationsAsync()
    {
        return await _context.TransactionNotifications
            .Include(n => n.ScheduledTransaction)
            .Where(n => !n.IsRead && !n.IsDismissed)
            .OrderByDescending(n => n.Priority)
            .ThenByDescending(n => n.ScheduledDate)
            .ToListAsync();
    }

    public async Task<List<TransactionNotification>> GetRecentNotificationsAsync(string? count)
    {
        int countValue = int.TryParse(count, out int parsed) ? parsed : 10; // Default to 10 if parsing fails
        return await _context.TransactionNotifications
            .Include(n => n.ScheduledTransaction)
            .OrderByDescending(n => n.ScheduledDate)
            .Take(countValue)
            .ToListAsync();
    }

    public async Task<List<TransactionNotification>> GetNotificationsByTypeAsync(Pilgrims.PersonalFinances.Models.Enums.AppNotificationType type)
    {
        return await _context.TransactionNotifications
            .Include(n => n.ScheduledTransaction)
            .Where(n => n.NotificationType == type)
            .OrderByDescending(n => n.ScheduledDate)
            .ToListAsync();
    }

    public async Task<List<TransactionNotification>> GetNotificationsByPriorityAsync(Pilgrims.PersonalFinances.Models.Enums.NotificationPriority priority)
    {
        return await _context.TransactionNotifications
            .Include(n => n.ScheduledTransaction)
            .Where(n => n.Priority == priority)
            .OrderByDescending(n => n.ScheduledDate)
            .ToListAsync();
    }

    public async Task<TransactionNotification?> GetNotificationByIdAsync(string id)
    {
        return await _context.TransactionNotifications
            .Include(n => n.ScheduledTransaction)
            .FirstOrDefaultAsync(n => n.Id == id);
    }

    #endregion

    #region Notification Actions

    public async Task MarkAsReadAsync(string notificationId)
    {
        var notification = await _context.TransactionNotifications.FindAsync(notificationId);
        if (notification != null)
        {
            notification.MarkAsRead();
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Notification marked as read: {notification.Title}");
            NotificationRead?.Invoke(this, notificationId);
        }
    }

    public async Task MarkAllAsReadAsync()
    {
        var unreadNotifications = await _context.TransactionNotifications
            .Where(n => !n.IsRead)
            .ToListAsync();

        foreach (var notification in unreadNotifications)
        {
            notification.MarkAsRead();
        }

        await _context.SaveChangesAsync();
    }

    public async Task DismissNotificationAsync(string notificationId)
    {
        var notification = await _context.TransactionNotifications.FindAsync(notificationId);
        if (notification != null)
        {
            notification.Dismiss();
            await _context.SaveChangesAsync();
            NotificationDismissed?.Invoke(this, notificationId);
        }
    }

    public async Task DeleteNotificationAsync(string notificationId)
    {
        var notification = await _context.TransactionNotifications.FindAsync(notificationId);
        if (notification != null)
        {
            _context.TransactionNotifications.Remove(notification);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteOldNotificationsAsync(int daysOld = 30)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-daysOld);
        var oldNotifications = await _context.TransactionNotifications
            .Where(n => n.ScheduledDate < cutoffDate && (n.IsRead || n.IsDismissed))
            .ToListAsync();

        _context.TransactionNotifications.RemoveRange(oldNotifications);
        await _context.SaveChangesAsync();
    }

    #endregion

    #region Scheduled Transaction Notifications

    public async Task CreateUpcomingTransactionNotificationsAsync()
    {
        var upcomingTransactions = await _scheduledTransactionService.GetUpcomingTransactionsAsync(7); // Next 7 days

        foreach (var transaction in upcomingTransactions)
        {
            // Check if notification already exists for this transaction and date
            var existingNotification = await _context.TransactionNotifications
                .FirstOrDefaultAsync(n => 
                    n.ScheduledTransactionId == transaction.Id &&
                    n.NotificationType == AppNotificationType.BillReminder &&
                    n.ScheduledDate.Date == GetNotificationDate(transaction).Date);

            if (existingNotification == null)
            {
                var notification = CreateUpcomingTransactionNotification(transaction);
                _context.TransactionNotifications.Add(notification);
                NotificationCreated?.Invoke(this, notification);
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task CreateOverdueTransactionNotificationsAsync()
    {
        var overdueTransactions = await _scheduledTransactionService.GetOverdueTransactionsAsync();

        foreach (var transaction in overdueTransactions)
        {
            // Check if overdue notification already exists for today
            var existingNotification = await _context.TransactionNotifications
                .FirstOrDefaultAsync(n => 
                    n.ScheduledTransactionId == transaction.Id &&
                    n.NotificationType == AppNotificationType.BudgetAlert &&
                    n.ScheduledDate.Date == DateTime.Today);

            if (existingNotification == null)
            {
                var notification = TransactionNotification.CreateOverdueAlert(transaction);

                _context.TransactionNotifications.Add(notification);
                NotificationCreated?.Invoke(this, notification);
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task CreateTransactionProcessedNotificationAsync(ScheduledTransaction scheduledTransaction, Transaction processedTransaction)
    {
        var notification = TransactionNotification.CreateTransactionCreatedNotification(
            scheduledTransaction,
            processedTransaction
        );

        _context.TransactionNotifications.Add(notification);
        await _context.SaveChangesAsync();
        NotificationCreated?.Invoke(this, notification);
    }

    public async Task CreateTransactionFailedNotificationAsync(ScheduledTransaction scheduledTransaction, string errorMessage)
    {
        var notification = TransactionNotification.CreateErrorNotification(
            scheduledTransaction,
            errorMessage
        );

        _context.TransactionNotifications.Add(notification);
        await _context.SaveChangesAsync();
        NotificationCreated?.Invoke(this, notification);
    }

    public async Task CreateApprovalRequestNotificationAsync(ScheduledTransaction scheduledTransaction)
    {
        var notification = TransactionNotification.CreateApprovalRequest(
            scheduledTransaction
        );

        _context.TransactionNotifications.Add(notification);
        await _context.SaveChangesAsync();
        NotificationCreated?.Invoke(this, notification);
    }

    #endregion

    #region Notification Statistics

    public async Task<int> GetUnreadCountAsync()
    {
        return await _context.TransactionNotifications
            .CountAsync(n => !n.IsRead && !n.IsDismissed);
    }

    public async Task<int> GetUnreadCountByTypeAsync(Pilgrims.PersonalFinances.Models.Enums.AppNotificationType type)
    {
        return await _context.TransactionNotifications
            .CountAsync(n => n.NotificationType == type && !n.IsRead && !n.IsDismissed);
    }

    public async Task<Dictionary<Pilgrims.PersonalFinances.Models.Enums.AppNotificationType, int>> GetNotificationCountsByTypeAsync()
    {
        return await _context.TransactionNotifications
            .Where(n => !n.IsDismissed)
            .GroupBy(n => n.NotificationType)
            .ToDictionaryAsync(g => g.Key, g => g.Count());
    }

    #endregion

    #region Notification Settings

    public async Task<bool> IsNotificationEnabledAsync(Pilgrims.PersonalFinances.Models.Enums.AppNotificationType type)
    {
        // For now, return true for all types. In the future, this could be configurable per user.
        return await Task.FromResult(true);
    }

    public async Task SetNotificationEnabledAsync(Pilgrims.PersonalFinances.Models.Enums.AppNotificationType type, bool enabled)
    {
        // For now, this is a no-op. In the future, this could store user preferences.
        await Task.CompletedTask;
    }

    #endregion

    #region Private Helper Methods

    private TransactionNotification CreateUpcomingTransactionNotification(ScheduledTransaction transaction)
    {
        var daysUntilDue = (transaction.NextDueDate!.Value.Date - DateTime.Today).Days;
        var priority = daysUntilDue switch
        {
            0 => Pilgrims.PersonalFinances.Models.Enums.NotificationPriority.High,
            1 => Pilgrims.PersonalFinances.Models.Enums.NotificationPriority.Medium,
            _ => Pilgrims.PersonalFinances.Models.Enums.NotificationPriority.Low
        };

        var timeDescription = daysUntilDue switch
        {
            0 => "today",
            1 => "tomorrow",
            _ => $"in {daysUntilDue} days"
        };

        return TransactionNotification.CreateReminder(
            transaction,
            GetNotificationDate(transaction)
        );
    }

    private DateTime GetNotificationDate(ScheduledTransaction transaction)
    {
        if (transaction.NextDueDate == null)
            return DateTime.Today;

        return transaction.NotificationTiming switch
        {
            NotificationTiming.SameDay => transaction.NextDueDate.Value.Date,
            NotificationTiming.OneDayBefore => transaction.NextDueDate.Value.Date.AddDays(-1),
            NotificationTiming.ThreeDaysBefore => transaction.NextDueDate.Value.Date.AddDays(-3),
            NotificationTiming.OneWeekBefore => transaction.NextDueDate.Value.Date.AddDays(-7),
            _ => transaction.NextDueDate.Value.Date
        };
    }

    #endregion

    #region Additional Interface Methods

    public async Task<TransactionNotification> CreateNotificationAsync(string title, string message, AppNotificationType type, NotificationPriority priority = NotificationPriority.Medium)
    {
        var notification = new TransactionNotification
        {
            Id = Guid.NewGuid().ToString(),
            Title = title,
            Message = message,
            NotificationType = type,
            Priority = priority,
            ScheduledDate = DateTime.UtcNow,
            IsRead = false,
            IsDismissed = false
        };

        _context.TransactionNotifications.Add(notification);
        await _context.SaveChangesAsync();

        _logger.LogNotification(type.ToString(), message, title);
        NotificationCreated?.Invoke(this, notification);
        return notification;
    }

    public async Task<TransactionNotification> CreateUpcomingNotificationAsync(string scheduledTransactionId)
    {
        var scheduledTransaction = await _context.ScheduledTransactions
            .FirstOrDefaultAsync(st => st.Id == scheduledTransactionId);

        if (scheduledTransaction == null)
            throw new ArgumentException("Scheduled transaction not found", nameof(scheduledTransactionId));

        var notification = TransactionNotification.CreateReminder(
            scheduledTransaction,
            GetNotificationDate(scheduledTransaction)
        );

        _context.TransactionNotifications.Add(notification);
        await _context.SaveChangesAsync();
        NotificationCreated?.Invoke(this, notification);
        return notification;
    }

    public async Task<TransactionNotification> CreateOverdueNotificationAsync(string scheduledTransactionId)
    {
        var scheduledTransaction = await _context.ScheduledTransactions
            .FirstOrDefaultAsync(st => st.Id == scheduledTransactionId);

        if (scheduledTransaction == null)
            throw new ArgumentException("Scheduled transaction not found", nameof(scheduledTransactionId));

        var notification = TransactionNotification.CreateOverdueAlert(scheduledTransaction);

        _context.TransactionNotifications.Add(notification);
        await _context.SaveChangesAsync();
        NotificationCreated?.Invoke(this, notification);
        return notification;
    }

    public async Task CreateBudgetAlertNotificationAsync(BudgetAlert budgetAlert)
    {
        var budget = await _context.Budgets.FindAsync(budgetAlert.BudgetId);
        if (budget == null) return;

        var notification = new TransactionNotification
        {
            Id = Guid.NewGuid().ToString(),
            Title = $"Budget Alert: {budget.Name}",
            Message = budgetAlert.Message,
            NotificationType = AppNotificationType.BudgetAlert,
            Priority = budgetAlert.AlertLevel switch
            {
                50 => NotificationPriority.Low,
                75 => NotificationPriority.Medium,
                90 => NotificationPriority.High,
                100 => NotificationPriority.Critical,
                _ => NotificationPriority.Medium
            },
            ScheduledDate = DateTime.UtcNow,
            IsRead = false,
            IsDismissed = false
        };

        _context.TransactionNotifications.Add(notification);
        await _context.SaveChangesAsync();
        NotificationCreated?.Invoke(this, notification);
    }

    #endregion

    #region Goal and Debt Notifications

    public async Task CreateGoalReminderNotificationAsync(Goal goal)
    {
        if (!goal.IsReminderDue()) return;

        var notification = new TransactionNotification
        {
            Id = Guid.NewGuid().ToString(),
            Title = $"Goal Progress: {goal.Name}",
            Message = $"You're {goal.ProgressPercentage:F1}% towards your {goal.Name} goal. {goal.FormattedRemainingAmount} remaining to reach {goal.FormattedTargetAmount}",
            NotificationType = AppNotificationType.SystemAlert,
            Priority = NotificationPriority.Medium,
            ScheduledDate = DateTime.UtcNow,
            IsRead = false,
            IsDismissed = false
        };

        _context.TransactionNotifications.Add(notification);
        await _context.SaveChangesAsync();
        
        goal.MarkReminderSent();
        _context.Goals.Update(goal);
        await _context.SaveChangesAsync();
        
        NotificationCreated?.Invoke(this, notification);
    }

    public async Task CreateDebtPaymentReminderAsync(Debt debt, int advanceDays = 3)
    {
        var nextPaymentDate = DateTime.Now.AddDays((debt.PaymentDueDay ?? 1) - DateTime.Now.Day);
        if (nextPaymentDate <= DateTime.Now)
            nextPaymentDate = nextPaymentDate.AddMonths(1);

        // Check if notification already exists for this debt and payment date
        var existingNotification = await _context.TransactionNotifications
            .FirstOrDefaultAsync(n => 
                n.Message.Contains(debt.Name) &&
                n.NotificationType == AppNotificationType.DebtPayment &&
                n.ScheduledDate.Date == nextPaymentDate.AddDays(-advanceDays).Date);

        if (existingNotification != null) return;

        var notification = new TransactionNotification
        {
            Id = Guid.NewGuid().ToString(),
            Title = $"Debt Payment Due: {debt.Name}",
            Message = $"Your {debt.Name} payment of {debt.MinimumPayment:C} is due on {nextPaymentDate:MMM dd, yyyy}. Current balance: {debt.CurrentBalance:C}",
            NotificationType = AppNotificationType.DebtPayment,
            Priority = NotificationPriority.High,
            ScheduledDate = nextPaymentDate.AddDays(-advanceDays),
            IsRead = false,
            IsDismissed = false
        };

        _context.TransactionNotifications.Add(notification);
        await _context.SaveChangesAsync();
        NotificationCreated?.Invoke(this, notification);
    }

    public async Task CreateReconciliationReminderAsync(string accountId)
    {
        var account = await _context.Accounts.FindAsync(accountId);
        if (account == null) return;

        // Check if reconciliation reminder already exists for this month
        var existingNotification = await _context.TransactionNotifications
            .FirstOrDefaultAsync(n => 
                n.Message.Contains(account.Name) &&
                n.NotificationType == AppNotificationType.ReconciliationReminder &&
                n.ScheduledDate.Month == DateTime.Now.Month &&
                n.ScheduledDate.Year == DateTime.Now.Year);

        if (existingNotification != null) return;

        var notification = new TransactionNotification
        {
            Id = Guid.NewGuid().ToString(),
            Title = $"Reconciliation Due: {account.Name}",
            Message = $"It's time to reconcile your {account.Name} account. Last reconciliation was over 30 days ago.",
            NotificationType = AppNotificationType.ReconciliationReminder,
            Priority = NotificationPriority.Medium,
            ScheduledDate = DateTime.UtcNow,
            IsRead = false,
            IsDismissed = false
        };

        _context.TransactionNotifications.Add(notification);
        await _context.SaveChangesAsync();
        NotificationCreated?.Invoke(this, notification);
    }

    public async Task ProcessGoalRemindersAsync()
    {
        var activeGoals = await _context.Goals
            .Where(g => g.IsActive && !g.IsCompleted && g.EnableReminders)
            .ToListAsync();

        foreach (var goal in activeGoals)
        {
            if (goal.IsReminderDue())
            {
                await CreateGoalReminderNotificationAsync(goal);
            }
        }
    }

    public async Task ProcessDebtPaymentRemindersAsync()
    {
        var activeDebts = await _context.Debts
            .Where(d => d.IsActive && d.CurrentBalance > 0)
            .ToListAsync();

        foreach (var debt in activeDebts)
        {
            await CreateDebtPaymentReminderAsync(debt);
        }
    }

    public async Task ProcessReconciliationRemindersAsync()
    {
        var accounts = await _context.Accounts
            .Where(a => a.IsActive)
            .ToListAsync();

        foreach (var account in accounts)
        {
            // Check if account needs reconciliation (simplified logic)
            var lastReconciliation = await _context.ReconciliationSessions
                .Where(r => r.AccountId == account.Id && r.Status == ReconciliationStatus.Completed)
                .OrderByDescending(r => r.CreatedAt)
                .FirstOrDefaultAsync();

            if (lastReconciliation == null || 
                (DateTime.Now - lastReconciliation.CreatedAt).TotalDays > 30)
            {
                await CreateReconciliationReminderAsync(account.Id);
            }
        }
    }

    #endregion

    #region Enhanced Notification Management

    public async Task SnoozeNotificationAsync(string notificationId, int minutes)
    {
        var notification = await _context.TransactionNotifications.FindAsync(notificationId);
        if (notification != null)
        {
            // For TransactionNotification, we'll update the scheduled date
            notification.ScheduledDate = DateTime.UtcNow.AddMinutes(minutes);
            await _context.SaveChangesAsync();
        }
    }

    public async Task SnoozeNotificationAsync(string notificationId, TimeSpan duration)
    {
        var notification = await _context.TransactionNotifications.FindAsync(notificationId);
        if (notification != null)
        {
            notification.SnoozedUntil = DateTime.UtcNow.Add(duration);
            await _context.SaveChangesAsync();
        }
    }

    public async Task UnsnoozeNotificationAsync(string notificationId)
    {
        var notification = await _context.TransactionNotifications.FindAsync(notificationId);
        if (notification != null)
        {
            notification.SnoozedUntil = null;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<TransactionNotification>> GetSnoozedNotificationsAsync()
    {
        return await _context.TransactionNotifications
            .Where(n => n.SnoozedUntil.HasValue && n.SnoozedUntil > DateTime.UtcNow)
            .OrderBy(n => n.SnoozedUntil)
            .ToListAsync();
    }

    public async Task ClearAllNotificationsAsync()
    {
        var notifications = await _context.TransactionNotifications.ToListAsync();
        _context.TransactionNotifications.RemoveRange(notifications);
        await _context.SaveChangesAsync();
    }

    public async Task EnableAllNotificationsAsync()
    {
        // This would typically update user preferences in a settings table
        // For now, we'll implement a placeholder
        await Task.CompletedTask;
    }

    public async Task DisableAllNotificationsAsync()
    {
        // This would typically update user preferences in a settings table
        // For now, we'll implement a placeholder
        await Task.CompletedTask;
    }

    public async Task<NotificationSettings> GetNotificationSettingsAsync(AppNotificationType type)
    {
        // This would typically retrieve from a settings table
        // For now, return default settings
        return new NotificationSettings
        {
            NotificationType = type,
            IsEnabled = true,
            DaysInAdvance = 3,
            PreferredChannels = NotificationChannel.InApp
        };
    }

    public async Task SaveNotificationSettingsAsync(NotificationSettings settings)
    {
        // This would typically save to a settings table
        // For now, we'll implement a placeholder
        await Task.CompletedTask;
    }

    public async Task<List<NotificationSettings>> GetAllNotificationSettingsAsync()
    {
        // This would typically retrieve all settings from a settings table
        // For now, return default settings for all types
        var allTypes = Enum.GetValues<AppNotificationType>();
        var settings = new List<NotificationSettings>();
        
        foreach (var type in allTypes)
        {
            settings.Add(new NotificationSettings
            {
                NotificationType = type,
                IsEnabled = true,
                DaysInAdvance = 3,
                PreferredChannels = NotificationChannel.InApp
            });
        }
        
        return settings;
    }

    public async Task SaveAllNotificationSettingsAsync(List<NotificationSettings> settings)
    {
        // This would typically save all settings to a settings table
        // For now, we'll implement a placeholder
        await Task.CompletedTask;
    }

    public async Task CreateGoalReminderAsync(string goalId)
    {
        var goal = await _context.Goals.FindAsync(goalId);
        if (goal != null)
        {
            await CreateNotificationAsync(
                $"Goal Reminder: {goal.Name}",
                $"Don't forget about your goal '{goal.Name}'. Target amount: {goal.TargetAmount:C}",
                AppNotificationType.SystemAlert,
                NotificationPriority.Medium
            );
        }
    }

    public async Task CreateGoalDeadlineNotificationAsync(string goalId)
    {
        var goal = await _context.Goals.FindAsync(goalId);
        if (goal != null)
        {
            await CreateNotificationAsync(
                $"Goal Deadline Approaching: {goal.Name}",
                $"Your goal '{goal.Name}' deadline is approaching on {goal.TargetDate:MMM dd, yyyy}",
                AppNotificationType.SystemAlert,
                NotificationPriority.High
            );
        }
    }

    public async Task CreateGoalAchievedNotificationAsync(string goalId)
    {
        var goal = await _context.Goals.FindAsync(goalId);
        if (goal != null)
        {
            await CreateNotificationAsync(
                $"🎉 Goal Achieved: {goal.Name}",
                $"Congratulations! You've achieved your goal '{goal.Name}' with a target of {goal.TargetAmount:C}",
                AppNotificationType.SystemAlert,
                NotificationPriority.High
            );
        }
    }

    public async Task CreateGoalProgressNotificationAsync(string goalId, decimal progressPercentage)
    {
        var goal = await _context.Goals.FindAsync(goalId);
        if (goal != null)
        {
            await CreateNotificationAsync(
                $"Goal Progress: {goal.Name}",
                $"You're {progressPercentage:F1}% towards your goal '{goal.Name}'. Keep it up!",
                AppNotificationType.SystemAlert,
                NotificationPriority.Low
            );
        }
    }

    public async Task ProcessAllPendingNotificationsAsync()
    {
        await CreateUpcomingTransactionNotificationsAsync();
        await CreateOverdueTransactionNotificationsAsync();
        await ProcessGoalRemindersAsync();
        await ProcessDebtPaymentRemindersAsync();
        await ProcessReconciliationRemindersAsync();
    }

    #endregion
}