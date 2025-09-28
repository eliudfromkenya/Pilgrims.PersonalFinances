using Pilgrims.PersonalFinances.Models;
using Pilgrims.PersonalFinances.Models.Enums;

namespace Pilgrims.PersonalFinances.Services.Interfaces
{
    public interface IReconciliationService
    {
        // Reconciliation Session Management
        Task<ReconciliationSession> CreateReconciliationSessionAsync(string accountId, string sessionName, DateTime statementStartDate, DateTime statementEndDate, decimal statementStartingBalance, decimal statementEndingBalance, string? createdBy = null);
        Task<ReconciliationSession?> GetReconciliationSessionAsync(string sessionId);
        Task<IEnumerable<ReconciliationSession>> GetReconciliationSessionsAsync(string accountId, int pageNumber = 1, int pageSize = 20);
        Task<IEnumerable<ReconciliationSession>> GetRecentReconciliationSessionsAsync(string accountId, int count = 5);
        Task<ReconciliationSession> UpdateReconciliationSessionAsync(ReconciliationSession session);
        Task<bool> DeleteReconciliationSessionAsync(string sessionId);

        // Reconciliation Items Management
        Task<ReconciliationItem> AddReconciliationItemAsync(string sessionId, string? transactionId, DateTime transactionDate, string description, decimal amount, ReconciliationItemType itemType, string? reference = null, bool isStatementOnly = false, bool isBookOnly = false);
        Task<ReconciliationItem> UpdateReconciliationItemAsync(ReconciliationItem item);
        Task<bool> DeleteReconciliationItemAsync(string itemId);
        Task<IEnumerable<ReconciliationItem>> GetReconciliationItemsAsync(string sessionId);
        Task<IEnumerable<ReconciliationItem>> GetUnmatchedItemsAsync(string sessionId);
        Task<IEnumerable<ReconciliationItem>> GetClearedItemsAsync(string sessionId);

        // Transaction Matching
        Task<bool> MatchTransactionAsync(string itemId, string transactionId, string? matchedBy = null);
        Task<bool> UnmatchTransactionAsync(string itemId, string? unmatchedBy = null);
        Task<IEnumerable<ReconciliationItem>> AutoMatchTransactionsAsync(string sessionId, decimal toleranceAmount = 0.01m);
        Task<IEnumerable<Transaction>> GetUnreconciledTransactionsAsync(string accountId, DateTime startDate, DateTime endDate);

        // Reconciliation Process
        Task<ReconciliationSession> StartReconciliationAsync(string accountId, DateTime statementDate, decimal statementBalance, string? createdBy = null);
        Task<ReconciliationSession> CompleteReconciliationAsync(string sessionId, string? completedBy = null);
        Task<ReconciliationSession> CancelReconciliationAsync(string sessionId, string? cancelledBy = null, string? reason = null);
        Task<bool> MarkItemAsClearedAsync(string itemId, string? clearedBy = null);
        Task<bool> MarkItemAsDisputedAsync(string itemId, string? notes = null);

        // Adjustments and Corrections
        Task<Transaction> CreateAdjustmentTransactionAsync(string accountId, decimal amount, string description, DateTime transactionDate, string? createdBy = null);
        Task<ReconciliationItem> CreateAdjustmentItemAsync(string sessionId, decimal amount, string description, DateTime transactionDate, string? createdBy = null);

        // Reporting and Analytics
        Task<ReconciliationSummaryDto> GetReconciliationSummaryAsync(string accountId, DateTime? startDate = null, DateTime? endDate = null);
        Task<IEnumerable<ReconciliationSession>> GetReconciliationHistoryAsync(string accountId, DateTime? startDate = null, DateTime? endDate = null);
        Task<IEnumerable<ReconciliationSession>> GetReconciliationHistoryAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<ReconciliationReportDto> GenerateReconciliationReportAsync(string sessionId);
        Task<IEnumerable<ReconciliationItem>> GetDisputedItemsAsync(string? accountId = null);

        // Balance Verification
        Task<decimal> CalculateBookBalanceAsync(string accountId, DateTime asOfDate);
        Task<bool> ValidateReconciliationAsync(string sessionId);
        Task<ReconciliationValidationResult> GetReconciliationValidationAsync(string sessionId);

        // Bulk Operations
        Task<int> BulkMarkItemsAsClearedAsync(IEnumerable<string> itemIds, string? clearedBy = null);
        Task<int> BulkMatchTransactionsAsync(Dictionary<string, string> itemTransactionPairs, string? matchedBy = null);
        Task<IEnumerable<ReconciliationItem>> ImportBankStatementItemsAsync(string sessionId, IEnumerable<BankStatementItemDto> statementItems);
    }

    // DTOs for reconciliation operations
    public class ReconciliationSummaryDto
    {
        public string AccountId { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public int TotalSessions { get; set; }
        public int CompletedSessions { get; set; }
        public int PendingSessions { get; set; }
        public decimal TotalReconciled { get; set; }
        public decimal TotalDifferences { get; set; }
        public DateTime? LastReconciliationDate { get; set; }
        public decimal CurrentBookBalance { get; set; }
        public int UnreconciledTransactions { get; set; }
    }

    public class ReconciliationReportDto
    {
        public ReconciliationSession Session { get; set; } = null!;
        public IEnumerable<ReconciliationItem> ClearedItems { get; set; } = new List<ReconciliationItem>();
        public IEnumerable<ReconciliationItem> UnmatchedItems { get; set; } = new List<ReconciliationItem>();
        public IEnumerable<ReconciliationItem> DisputedItems { get; set; } = new List<ReconciliationItem>();
        public decimal TotalClearedAmount { get; set; }
        public decimal TotalUnmatchedAmount { get; set; }
        public decimal TotalDisputedAmount { get; set; }
        public int ClearedItemsCount { get; set; }
        public int UnmatchedItemsCount { get; set; }
        public int DisputedItemsCount { get; set; }
    }

    public class ReconciliationValidationResult
    {
        public bool IsValid { get; set; }
        public decimal CalculatedDifference { get; set; }
        public decimal ReportedDifference { get; set; }
        public IEnumerable<string> ValidationErrors { get; set; } = new List<string>();
        public IEnumerable<string> Warnings { get; set; } = new List<string>();
    }

    public class BankStatementItemDto
    {
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? Reference { get; set; }
        public ReconciliationItemType ItemType { get; set; }
    }
}