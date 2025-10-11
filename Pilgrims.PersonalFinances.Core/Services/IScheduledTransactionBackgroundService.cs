using Pilgrims.PersonalFinances.Models;

namespace Pilgrims.PersonalFinances.Services;

/// <summary>
/// Interface for background service that handles scheduled transaction processing
/// </summary>
public interface IScheduledTransactionBackgroundService
{
    /// <summary>
    /// Start the background service
    /// </summary>
    Task StartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Stop the background service
    /// </summary>
    Task StopAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Process all due scheduled transactions
    /// </summary>
    Task ProcessDueTransactionsAsync();

    /// <summary>
    /// Send notifications for upcoming transactions
    /// </summary>
    Task SendUpcomingNotificationsAsync();

    /// <summary>
    /// Send notifications for overdue transactions
    /// </summary>
    Task SendOverdueNotificationsAsync();

    /// <summary>
    /// Event fired when a scheduled transaction is processed
    /// </summary>
    event EventHandler<ScheduledTransaction>? TransactionProcessed;

    /// <summary>
    /// Event fired when a notification is sent
    /// </summary>
    event EventHandler<TransactionNotification>? NotificationSent;

    /// <summary>
    /// Event fired when an error occurs during processing
    /// </summary>
    event EventHandler<string>? ProcessingError;
}