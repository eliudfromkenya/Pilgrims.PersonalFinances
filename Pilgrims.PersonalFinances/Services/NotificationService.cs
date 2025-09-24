using Microsoft.EntityFrameworkCore;
using Pilgrims.PersonalFinances.Data;
using Pilgrims.PersonalFinances.Models;
using Pilgrims.PersonalFinances.Models.Enums;
using Pilgrims.PersonalFinances.Services.Interfaces;

namespace Pilgrims.PersonalFinances.Services;

public class NotificationService : INotificationService
{
    private readonly PersonalFinanceContext _context;
    private readonly IScheduledTransactionService _scheduledTransactionService;

    public NotificationService(PersonalFinanceContext context, IScheduledTransactionService scheduledTransactionService)
    {
        _context = context;
        _scheduledTransactionService = scheduledTransactionService;
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

    #endregion
}