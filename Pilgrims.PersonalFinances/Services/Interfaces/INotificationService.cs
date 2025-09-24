using Pilgrims.PersonalFinances.Models;
using Pilgrims.PersonalFinances.Models.Enums;

namespace Pilgrims.PersonalFinances.Services.Interfaces;

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
    
    // Notification Statistics
    Task<int> GetUnreadCountAsync();
    Task<int> GetUnreadCountByTypeAsync(Pilgrims.PersonalFinances.Models.Enums.AppNotificationType type);
    Task<Dictionary<Pilgrims.PersonalFinances.Models.Enums.AppNotificationType, int>> GetNotificationCountsByTypeAsync();
    
    // Notification Settings
    Task<bool> IsNotificationEnabledAsync(Pilgrims.PersonalFinances.Models.Enums.AppNotificationType type);
    Task SetNotificationEnabledAsync(Pilgrims.PersonalFinances.Models.Enums.AppNotificationType type, bool enabled);
    
    // Real-time Notifications (for future implementation)
    event EventHandler<TransactionNotification>? NotificationCreated;
    event EventHandler<string>? NotificationRead;
    event EventHandler<string>? NotificationDismissed;
}