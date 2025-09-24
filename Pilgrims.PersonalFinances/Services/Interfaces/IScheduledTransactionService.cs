using Pilgrims.PersonalFinances.Models;
using Pilgrims.PersonalFinances.Models.Enums;

namespace Pilgrims.PersonalFinances.Services.Interfaces;

/// <summary>
/// Service interface for managing scheduled transactions with comprehensive CRUD operations
/// </summary>
public interface IScheduledTransactionService
{
    // Basic CRUD Operations
    Task<IEnumerable<ScheduledTransaction>> GetAllScheduledTransactionsAsync();
    Task<ScheduledTransaction?> GetScheduledTransactionByIdAsync(string id);
    Task<ScheduledTransaction> CreateScheduledTransactionAsync(ScheduledTransaction scheduledTransaction);
    Task<ScheduledTransaction> UpdateScheduledTransactionAsync(ScheduledTransaction scheduledTransaction);
    Task<bool> DeleteScheduledTransactionAsync(string id);
    Task<bool> DeleteScheduledTransactionsAsync(IEnumerable<string> ids);

    // Filtering and Search
    Task<IEnumerable<ScheduledTransaction>> GetActiveScheduledTransactionsAsync();
    Task<IEnumerable<ScheduledTransaction>> GetScheduledTransactionsByAccountAsync(string accountId);
    Task<IEnumerable<ScheduledTransaction>> GetScheduledTransactionsByCategoryAsync(string categoryId);
    Task<IEnumerable<ScheduledTransaction>> GetScheduledTransactionsByTypeAsync(TransactionType type);
    Task<IEnumerable<ScheduledTransaction>> SearchScheduledTransactionsAsync(string searchTerm);

    // Due Date Management
    Task<IEnumerable<ScheduledTransaction>> GetDueScheduledTransactionsAsync(DateTime? asOfDate = null);
    Task<IEnumerable<ScheduledTransaction>> GetDueTransactionsAsync(DateTime? asOfDate = null);
    Task<IEnumerable<ScheduledTransaction>> GetOverdueScheduledTransactionsAsync();
    Task<IEnumerable<ScheduledTransaction>> GetOverdueTransactionsAsync();
    Task<IEnumerable<ScheduledTransaction>> GetUpcomingScheduledTransactionsAsync(int daysAhead = 7);
    Task<IEnumerable<ScheduledTransaction>> GetUpcomingTransactionsAsync(int daysAhead = 7);

    // Transaction Processing
    Task<Transaction> ProcessScheduledTransactionAsync(string scheduledTransactionId, DateTime? processDate = null);
    Task<IEnumerable<Transaction>> ProcessDueScheduledTransactionsAsync(DateTime? asOfDate = null);
    Task<bool> SkipScheduledTransactionOccurrenceAsync(string scheduledTransactionId, DateTime skipDate);
    Task<bool> UnskipScheduledTransactionOccurrenceAsync(string scheduledTransactionId, DateTime unskipDate);
    Task<bool> MarkAsProcessedAsync(string scheduledTransactionId);

    // Notification Management
    Task<IEnumerable<TransactionNotification>> GetNotificationsForScheduledTransactionAsync(string scheduledTransactionId);
    Task<TransactionNotification> CreateReminderNotificationAsync(string scheduledTransactionId, DateTime reminderDate);
    Task<TransactionNotification> CreateOverdueNotificationAsync(string scheduledTransactionId);
    Task<TransactionNotification> CreateApprovalNotificationAsync(string scheduledTransactionId);
    Task ProcessScheduledNotificationsAsync();

    // Bulk Operations
    Task<bool> BulkActivateScheduledTransactionsAsync(IEnumerable<string> scheduledTransactionIds);
    Task<bool> BulkDeactivateScheduledTransactionsAsync(IEnumerable<string> scheduledTransactionIds);
    Task<bool> BulkUpdateScheduledTransactionCategoryAsync(IEnumerable<string> scheduledTransactionIds, string categoryId);

    // Statistics and Analytics
    Task<decimal> GetProjectedMonthlyAmountAsync(string? accountId = null, string? categoryId = null);
    Task<Dictionary<string, decimal>> GetProjectedAmountsByAccountAsync();
    Task<Dictionary<string, decimal>> GetProjectedAmountsByCategoryAsync();
    Task<IEnumerable<ScheduledTransaction>> GetScheduledTransactionsByRecurrenceTypeAsync(RecurrenceType recurrenceType);

    // Validation and Calculation
    Task<List<string>> ValidateScheduledTransactionAsync(ScheduledTransaction scheduledTransaction);
    Task<DateTime?> CalculateNextDueDateAsync(string scheduledTransactionId);
    Task<IEnumerable<DateTime>> GetProjectedDatesAsync(string scheduledTransactionId, int monthsAhead = 12);

    // Additional methods used by Razor components
    Task<Transaction> ProcessDueTransactionAsync(string scheduledTransactionId);
    Task<bool> PauseAsync(string scheduledTransactionId);
    Task<bool> ResumeAsync(string scheduledTransactionId);
    Task<bool> DeleteAsync(string scheduledTransactionId);
}