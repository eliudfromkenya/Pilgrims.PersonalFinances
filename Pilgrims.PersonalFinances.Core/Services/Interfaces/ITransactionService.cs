using Pilgrims.PersonalFinances.Core.Models;
using Pilgrims.PersonalFinances.Core.Models.Enums;

namespace Pilgrims.PersonalFinances.Core.Services.Interfaces;

/// <summary>
/// Service interface for managing transactions with comprehensive CRUD operations
/// </summary>
public interface ITransactionService
{
    // Basic CRUD Operations
    Task<IEnumerable<Transaction>> GetAllTransactionsAsync();
    Task<Transaction?> GetTransactionByIdAsync(string id);
    Task<Transaction> CreateTransactionAsync(Transaction transaction);
    Task<Transaction> UpdateTransactionAsync(Transaction transaction);
    Task<bool> DeleteTransactionAsync(string id);
    Task<bool> DeleteTransactionsAsync(IEnumerable<string> ids);

    // Filtering and Search
    Task<IEnumerable<Transaction>> GetTransactionsByAccountAsync(string accountId);
    Task<IEnumerable<Transaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<Transaction>> GetTransactionsByCategoryAsync(string categoryId);
    Task<IEnumerable<Transaction>> GetTransactionsByTypeAsync(TransactionType type);
    Task<IEnumerable<Transaction>> GetTransactionsByStatusAsync(TransactionStatus status);
    Task<IEnumerable<Transaction>> SearchTransactionsAsync(string searchTerm);
    Task<IEnumerable<Transaction>> GetTransactionsByPayeeAsync(string payee);
    Task<IEnumerable<Transaction>> GetTransactionsByTagsAsync(IEnumerable<string> tags);

    // Advanced Filtering
    Task<IEnumerable<Transaction>> GetFilteredTransactionsAsync(
        string? accountId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? categoryId = null,
        TransactionType? type = null,
        TransactionStatus? status = null,
        string? payee = null,
        IEnumerable<string>? tags = null,
        decimal? minAmount = null,
        decimal? maxAmount = null,
        string? searchTerm = null,
        int skip = 0,
        int take = 50);

    // Split Transactions
    Task<IEnumerable<SplitTransaction>> GetSplitTransactionsAsync(string parentTransactionId);
    Task<SplitTransaction> CreateSplitTransactionAsync(SplitTransaction splitTransaction);
    Task<SplitTransaction> UpdateSplitTransactionAsync(SplitTransaction splitTransaction);
    Task<bool> DeleteSplitTransactionAsync(string splitTransactionId);

    // Transaction Templates
    Task<List<TransactionTemplate>> GetTransactionTemplatesAsync();
    Task<TransactionTemplate> CreateTransactionTemplateAsync(TransactionTemplate template);
    Task<TransactionTemplate> UpdateTransactionTemplateAsync(TransactionTemplate template);
    Task<bool> DeleteTransactionTemplateAsync(string? templateId);
    Task<Transaction> CreateTransactionFromTemplateAsync(string templateId, DateTime? transactionDate = null);
    
    // Categories
    Task<List<Category>> GetCategoriesAsync();

    // Bulk Operations
    Task<bool> BulkUpdateTransactionStatusAsync(IEnumerable<string> transactionIds, TransactionStatus status);
    Task<bool> BulkUpdateTransactionCategoryAsync(IEnumerable<string> transactionIds, string categoryId);
    Task<bool> BulkDeleteTransactionsAsync(IEnumerable<string> transactionIds);

    // Duplicate Detection
    Task<IEnumerable<Transaction>> FindPotentialDuplicatesAsync(Transaction transaction);
    Task<bool> MarkTransactionAsNotDuplicateAsync(string transactionId, string potentialDuplicateId);

    // Attachments
    Task<TransactionAttachment> AddAttachmentAsync(string transactionId, string fileName, string contentType, byte[] fileData);
    Task<bool> DeleteAttachmentAsync(string attachmentId);
    Task<IEnumerable<TransactionAttachment>> GetTransactionAttachmentsAsync(string transactionId);

    // Statistics and Analytics
    Task<decimal> GetAccountBalanceAsync(string accountId, DateTime? asOfDate = null);
    Task<decimal> GetTotalIncomeAsync(string accountId, DateTime startDate, DateTime endDate);
    Task<decimal> GetTotalExpensesAsync(string accountId, DateTime startDate, DateTime endDate);
    Task<Dictionary<string, decimal>> GetExpensesByCategoryAsync(string accountId, DateTime startDate, DateTime endDate);
    Task<Dictionary<DateTime, decimal>> GetDailyBalanceHistoryAsync(string accountId, DateTime startDate, DateTime endDate);

    // Recurring Transactions
    Task<IEnumerable<Transaction>> GetRecurringTransactionsAsync();
    Task<IEnumerable<Transaction>> ProcessRecurringTransactionsAsync(DateTime processDate);
    Task<Transaction> CreateRecurringTransactionAsync(Transaction recurringTransaction);

    // Validation
    Task<bool> ValidateTransactionAsync(Transaction transaction);
    Task<IEnumerable<string>> GetValidationErrorsAsync(Transaction transaction);

    // Import/Export
    Task<IEnumerable<Transaction>> ImportTransactionsAsync(IEnumerable<Transaction> transactions);
    Task<byte[]> ExportTransactionsAsync(IEnumerable<string> transactionIds, string format = "csv");
}
