using Pilgrims.PersonalFinances.Core.Models;
using Pilgrims.PersonalFinances.Core.Models.Enums;

namespace Pilgrims.PersonalFinances.Core.Services.Interfaces;

public interface INotificationService
{
    // Notification Management
    Task<List<TransactionNotification>> GetAllNotificationsAsync();
    Task<List<TransactionNotification>> GetUnreadNotificationsAsync();
    Task<TransactionNotification> CreateNotificationAsync(string title, string message, AppNotificationType type, NotificationPriority priority = NotificationPriority.Medium);
    Task<TransactionNotification> CreateUpcomingNotificationAsync(string scheduledTransactionId);
    Task<TransactionNotification> CreateOverdueNotificationAsync(string scheduledTransactionId);
    Task<List<TransactionNotification>> GetRecentNotificationsAsync(string? count);
    Task<List<TransactionNotification>> GetNotificationsByTypeAsync(AppNotificationType type);
    Task<List<TransactionNotification>> GetNotificationsByPriorityAsync(NotificationPriority priority);
    Task<TransactionNotification?> GetNotificationByIdAsync(string id);
    
    // Notification Actions
    Task MarkAsReadAsync(string notificationId);
    Task MarkAllAsReadAsync();
    Task DismissNotificationAsync(string notificationId);
    Task DeleteNotificationAsync(string notificationId);
    Task DeleteOldNotificationsAsync(int daysOld = 30);
    
    // Scheduled Transaction Notifications
    Task CreateUpcomingTransactionNotificationsAsync();
    Task CreateOverdueTransactionNotificationsAsync();
    Task CreateTransactionProcessedNotificationAsync(ScheduledTransaction scheduledTransaction, Transaction processedTransaction);
    Task CreateTransactionFailedNotificationAsync(ScheduledTransaction scheduledTransaction, string errorMessage);
    Task CreateApprovalRequestNotificationAsync(ScheduledTransaction scheduledTransaction);
    
    // Budget Alert Notifications
    Task CreateBudgetAlertNotificationAsync(BudgetAlert budgetAlert);
    
    // Notification Statistics
    Task<int> GetUnreadCountAsync();
    Task<int> GetUnreadCountByTypeAsync(AppNotificationType type);
    Task<Dictionary<AppNotificationType, int>> GetNotificationCountsByTypeAsync();
    
    // Notification Settings
    Task<bool> IsNotificationEnabledAsync(AppNotificationType type);
    Task SetNotificationEnabledAsync(AppNotificationType type, bool enabled);
    Task<NotificationSettings> GetNotificationSettingsAsync(AppNotificationType type);
    Task SaveNotificationSettingsAsync(NotificationSettings settings);
    Task<List<NotificationSettings>> GetAllNotificationSettingsAsync();
    Task SaveAllNotificationSettingsAsync(List<NotificationSettings> settings);
    
    // Snooze Management
    Task SnoozeNotificationAsync(string notificationId, TimeSpan duration);
    Task UnsnoozeNotificationAsync(string notificationId);
    Task<List<TransactionNotification>> GetSnoozedNotificationsAsync();
    
    // Bulk Operations
    Task ClearAllNotificationsAsync();
    Task EnableAllNotificationsAsync();
    Task DisableAllNotificationsAsync();
    
    // Goal-related Notifications
    Task CreateGoalReminderAsync(string goalId);
    Task CreateGoalDeadlineNotificationAsync(string goalId);
    Task CreateGoalAchievedNotificationAsync(string goalId);
    Task CreateGoalProgressNotificationAsync(string goalId, decimal progressPercentage);
    
    // Account Reconciliation Notifications
    Task CreateReconciliationReminderAsync(string accountId);
    
    // Real-time Notifications (for future implementation)
    event EventHandler<TransactionNotification>? NotificationCreated;
    event EventHandler<string>? NotificationRead;
    event EventHandler<string>? NotificationDismissed;
}


