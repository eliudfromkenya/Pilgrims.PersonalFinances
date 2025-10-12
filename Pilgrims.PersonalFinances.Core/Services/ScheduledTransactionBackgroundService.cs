using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Pilgrims.PersonalFinances.Core.Models;
using Pilgrims.PersonalFinances.Data;
using Pilgrims.PersonalFinances.Core.Services.Interfaces;
using Pilgrims.PersonalFinances.Core.Models.Enums;

namespace Pilgrims.PersonalFinances.Core.Services;

/// <summary>
/// Background service that handles scheduled transaction processing and notifications
/// </summary>
public class ScheduledTransactionBackgroundService : BackgroundService, IScheduledTransactionBackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ScheduledTransactionBackgroundService> _logger;
    private readonly Timer _processingTimer;
    private readonly Timer _notificationTimer;

    public event EventHandler<ScheduledTransaction>? TransactionProcessed;
    public event EventHandler<TransactionNotification>? NotificationSent;
    public event EventHandler<string>? ProcessingError;

    public ScheduledTransactionBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<ScheduledTransactionBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;

        // Process transactions every 5 minutes
        _processingTimer = new Timer(async _ => await ProcessDueTransactionsAsync(), 
            null, TimeSpan.Zero, TimeSpan.FromMinutes(5));

        // Check for notifications every hour
        _notificationTimer = new Timer(async _ => await CheckNotificationsAsync(), 
            null, TimeSpan.Zero, TimeSpan.FromHours(1));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Scheduled Transaction Background Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }

        _logger.LogInformation("Scheduled Transaction Background Service stopped");
    }

    public async Task ProcessDueTransactionsAsync()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var scheduledTransactionService = scope.ServiceProvider.GetRequiredService<IScheduledTransactionService>();
            var transactionService = scope.ServiceProvider.GetRequiredService<ITransactionService>();

            var dueTransactions = await scheduledTransactionService.GetDueTransactionsAsync();

            foreach (var scheduledTransaction in dueTransactions)
            {
                try
                {
                    if (scheduledTransaction.SchedulingMode == SchedulingMode.AutoPost)
                    {
                        // Auto-post the transaction
                        var transaction = new Transaction
                        {
                            Id = Guid.NewGuid().ToString(),
                            AccountId = scheduledTransaction.AccountId,
                            CategoryId = scheduledTransaction.CategoryId,
                            Amount = scheduledTransaction.Amount,
                            Description = scheduledTransaction.Description,
                            Type = scheduledTransaction.TransactionType,
                            Date = DateTime.Now,
                            CreatedAt = DateTime.Now,
                            Tags = scheduledTransaction.Tags,
                            Notes = $"Auto-posted from scheduled transaction: {scheduledTransaction.Name}"
                        };

                        await transactionService.CreateTransactionAsync(transaction);
                        await scheduledTransactionService.MarkAsProcessedAsync(scheduledTransaction.Id);

                        _logger.LogInformation($"Auto-posted transaction for scheduled transaction {scheduledTransaction.Name}");
                        TransactionProcessed?.Invoke(this, scheduledTransaction);
                    }
                    else
                    {
                        // Create approval notification
                        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
                        await notificationService.CreateApprovalRequestNotificationAsync(scheduledTransaction);
                        
                        _logger.LogInformation($"Created approval notification for scheduled transaction {scheduledTransaction.Name}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error processing scheduled transaction {scheduledTransaction.Id}");
                    ProcessingError?.Invoke(this, $"Error processing {scheduledTransaction.Name}: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ProcessDueTransactionsAsync");
            ProcessingError?.Invoke(this, $"Error processing due transactions: {ex.Message}");
        }
    }

    public async Task SendUpcomingNotificationsAsync()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var scheduledTransactionService = scope.ServiceProvider.GetRequiredService<IScheduledTransactionService>();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

            var upcomingTransactions = await scheduledTransactionService.GetUpcomingTransactionsAsync(7); // Next 7 days

            foreach (var scheduledTransaction in upcomingTransactions)
            {
                try
                {
                    var daysUntilDue = scheduledTransaction.NextDueDate.HasValue 
                        ? (scheduledTransaction.NextDueDate.Value - DateTime.Now).Days 
                        : 0;
                    
                    // Check if we should send notification based on user preferences
                    if (ShouldSendUpcomingNotification(scheduledTransaction, daysUntilDue))
                    {
                        var notification = await notificationService.CreateUpcomingNotificationAsync(scheduledTransaction.Id);
                        _logger.LogInformation($"Sent upcoming notification for {scheduledTransaction.Name}");
                        NotificationSent?.Invoke(this, notification);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error sending upcoming notification for {scheduledTransaction.Id}");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in SendUpcomingNotificationsAsync");
        }
    }

    public async Task SendOverdueNotificationsAsync()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var scheduledTransactionService = scope.ServiceProvider.GetRequiredService<IScheduledTransactionService>();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

            var overdueTransactions = await scheduledTransactionService.GetOverdueTransactionsAsync();

            foreach (var scheduledTransaction in overdueTransactions)
            {
                try
                {
                    var notification = await notificationService.CreateOverdueNotificationAsync(scheduledTransaction.Id);
                    _logger.LogInformation($"Sent overdue notification for {scheduledTransaction.Name}");
                    NotificationSent?.Invoke(this, notification);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error sending overdue notification for {scheduledTransaction.Id}");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in SendOverdueNotificationsAsync");
        }
    }

    private async Task CheckNotificationsAsync()
    {
        await SendUpcomingNotificationsAsync();
        await SendOverdueNotificationsAsync();
    }

    private bool ShouldSendUpcomingNotification(ScheduledTransaction scheduledTransaction, int daysUntilDue)
    {
        // Check notification timing preferences
        return scheduledTransaction.NotificationTiming switch
        {
            NotificationTiming.OneDayBefore => daysUntilDue == 1,
            NotificationTiming.ThreeDaysBefore => daysUntilDue == 3,
            NotificationTiming.OneWeekBefore => daysUntilDue == 7,
            NotificationTiming.SameDay => daysUntilDue == 0,
            _ => false
        };
    }

    public new async Task StartAsync(CancellationToken cancellationToken = default)
    {
        await base.StartAsync(cancellationToken);
    }

    public new async Task StopAsync(CancellationToken cancellationToken = default)
    {
        _processingTimer?.Dispose();
        _notificationTimer?.Dispose();
        await base.StopAsync(cancellationToken);
    }

    public override void Dispose()
    {
        _processingTimer?.Dispose();
        _notificationTimer?.Dispose();
        base.Dispose();
    }
}
