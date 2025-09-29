using Microsoft.EntityFrameworkCore;
using Pilgrims.PersonalFinances.Data;
using Pilgrims.PersonalFinances.Models;
using Pilgrims.PersonalFinances.Models.DTOs;
using Pilgrims.PersonalFinances.Models.Enums;
using Pilgrims.PersonalFinances.Services.Interfaces;
using System.Linq;

namespace Pilgrims.PersonalFinances.Services
{
    public class ReconciliationService : IReconciliationService
    {
        private readonly PersonalFinanceContext _context;
        private readonly INotificationService _notificationService;

        public ReconciliationService(PersonalFinanceContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        #region Reconciliation Session Management

        public async Task<ReconciliationSession> CreateReconciliationSessionAsync(string accountId, string sessionName, DateTime statementStartDate, DateTime statementEndDate, decimal statementStartingBalance, decimal statementEndingBalance, string? createdBy = null)
        {
            var bookStartingBalance = await CalculateBookBalanceAsync(accountId, statementStartDate);
            var bookEndingBalance = await CalculateBookBalanceAsync(accountId, statementEndDate);

            var session = new ReconciliationSession
            {
                Id = Guid.NewGuid().ToString(),
                AccountId = accountId,
                SessionName = sessionName,
                ReconciliationDate = DateTime.UtcNow,
                StatementStartDate = statementStartDate,
                StatementEndDate = statementEndDate,
                StatementStartingBalance = statementStartingBalance,
                StatementEndingBalance = statementEndingBalance,
                BookStartingBalance = bookStartingBalance,
                BookEndingBalance = bookEndingBalance,
                Status = ReconciliationStatus.InProgress,
                CreatedBy = createdBy ?? "System",
                CreatedAt = DateTime.UtcNow
            };

            session.CalculateDifference();

            _context.ReconciliationSessions.Add(session);
            await _context.SaveChangesAsync();

            // Auto-populate with unreconciled transactions
            await PopulateSessionWithTransactionsAsync(session.Id, accountId, statementStartDate, statementEndDate);

            return session;
        }

        public async Task<ReconciliationSession?> GetReconciliationSessionAsync(string sessionId)
        {
            return await _context.ReconciliationSessions
                .Include(rs => rs.ReconciliationItems)
                .FirstOrDefaultAsync(rs => rs.Id == sessionId);
        }

        public async Task<IEnumerable<ReconciliationSession>> GetReconciliationSessionsAsync(string accountId, int pageNumber = 1, int pageSize = 20)
        {
            return await _context.ReconciliationSessions
                .Where(rs => rs.AccountId == accountId)
                .OrderByDescending(rs => rs.ReconciliationDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(rs => rs.ReconciliationItems)
                .ToListAsync();
        }

        public async Task<IEnumerable<ReconciliationSession>> GetRecentReconciliationSessionsAsync(string accountId, int count = 5)
        {
            return await _context.ReconciliationSessions
                .Where(rs => rs.AccountId == accountId)
                .OrderByDescending(rs => rs.ReconciliationDate)
                .Take(count)
                .Include(rs => rs.ReconciliationItems)
                .ToListAsync();
        }

        public async Task<ReconciliationSession> UpdateReconciliationSessionAsync(ReconciliationSession session)
        {
            session.MarkAsDirty();
            session.CalculateDifference();
            
            _context.ReconciliationSessions.Update(session);
            await _context.SaveChangesAsync();
            return session;
        }

        public async Task<bool> DeleteReconciliationSessionAsync(string sessionId)
        {
            var session = await _context.ReconciliationSessions
                .Include(rs => rs.ReconciliationItems)
                .FirstOrDefaultAsync(rs => rs.Id == sessionId);

            if (session == null) return false;

            _context.ReconciliationItems.RemoveRange(session.ReconciliationItems);
            _context.ReconciliationSessions.Remove(session);
            await _context.SaveChangesAsync();
            return true;
        }

        #endregion

        #region Reconciliation Items Management

        public async Task<ReconciliationItem> AddReconciliationItemAsync(string sessionId, string? transactionId, DateTime transactionDate, string description, decimal amount, ReconciliationItemType itemType, string? reference = null, bool isStatementOnly = false, bool isBookOnly = false)
        {
            var item = new ReconciliationItem
            {
                Id = Guid.NewGuid().ToString(),
                ReconciliationSessionId = sessionId,
                TransactionId = transactionId,
                TransactionDate = transactionDate,
                Description = description,
                Amount = amount,
                Reference = reference,
                ItemType = itemType,
                Status = ReconciliationItemStatus.Unmatched,
                IsStatementOnly = isStatementOnly,
                IsBookOnly = isBookOnly,
                CreatedAt = DateTime.UtcNow
            };

            _context.ReconciliationItems.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<ReconciliationItem> UpdateReconciliationItemAsync(ReconciliationItem item)
        {
            item.MarkAsDirty();
            _context.ReconciliationItems.Update(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<bool> DeleteReconciliationItemAsync(string itemId)
        {
            var item = await _context.ReconciliationItems.FindAsync(itemId);
            if (item == null) return false;

            _context.ReconciliationItems.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ReconciliationItem>> GetReconciliationItemsAsync(string sessionId)
        {
            return await _context.ReconciliationItems
                .Where(ri => ri.ReconciliationSessionId == sessionId)
                .OrderBy(ri => ri.TransactionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ReconciliationItem>> GetUnmatchedItemsAsync(string sessionId)
        {
            return await _context.ReconciliationItems
                .Where(ri => ri.ReconciliationSessionId == sessionId && ri.Status == ReconciliationItemStatus.Unmatched)
                .OrderBy(ri => ri.TransactionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ReconciliationItem>> GetClearedItemsAsync(string sessionId)
        {
            return await _context.ReconciliationItems
                .Where(ri => ri.ReconciliationSessionId == sessionId && ri.IsCleared)
                .OrderBy(ri => ri.TransactionDate)
                .ToListAsync();
        }

        #endregion

        #region Transaction Matching

        public async Task<bool> MatchTransactionAsync(string itemId, string transactionId, string? matchedBy = null)
        {
            var item = await _context.ReconciliationItems.FindAsync(itemId);
            if (item == null) return false;

            item.TransactionId = transactionId;
            item.Status = ReconciliationItemStatus.Matched;
            item.UpdatedBy = matchedBy;
            item.MarkAsDirty();

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnmatchTransactionAsync(string itemId, string? unmatchedBy = null)
        {
            var item = await _context.ReconciliationItems.FindAsync(itemId);
            if (item == null) return false;

            item.TransactionId = null;
            item.Status = ReconciliationItemStatus.Unmatched;
            item.UpdatedBy = unmatchedBy;
            item.MarkAsDirty();

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ReconciliationItem>> AutoMatchTransactionsAsync(string sessionId, decimal toleranceAmount = 0.01m)
        {
            var session = await GetReconciliationSessionAsync(sessionId);
            if (session == null) return new List<ReconciliationItem>();

            var unmatchedItems = await GetUnmatchedItemsAsync(sessionId);
            var transactions = await GetUnreconciledTransactionsAsync(session.AccountId, session.StatementStartDate, session.StatementEndDate);

            var matchedItems = new List<ReconciliationItem>();

            foreach (var item in unmatchedItems.Where(i => !i.IsStatementOnly))
            {
                var matchingTransaction = transactions.FirstOrDefault(t => 
                    Math.Abs(t.Amount - item.Amount) <= toleranceAmount &&
                    t.Date == item.TransactionDate.Date);

                if (matchingTransaction != null)
                {
                    await MatchTransactionAsync(item.Id, matchingTransaction.Id, "AutoMatch");
                    matchedItems.Add(item);
                }
            }

            return matchedItems;
        }

        public async Task<IEnumerable<Transaction>> GetUnreconciledTransactionsAsync(string accountId, DateTime startDate, DateTime endDate)
        {
            return await _context.Transactions
                .Where(t => t.AccountId == accountId && 
                           t.Date >= startDate && 
                           t.Date <= endDate &&
                           t.Status != TransactionStatus.Reconciled)
                .OrderBy(t => t.Date)
                .ToListAsync();
        }

        #endregion

        #region Reconciliation Process

        public async Task<ReconciliationSession> StartReconciliationAsync(string accountId, DateTime statementDate, decimal statementBalance, string? createdBy = null)
        {
            var sessionName = $"Reconciliation - {statementDate:yyyy-MM-dd}";
            var startDate = statementDate.AddDays(-30); // Default to 30 days back
            
            return await CreateReconciliationSessionAsync(accountId, sessionName, startDate, statementDate, 0, statementBalance, createdBy);
        }

        public async Task<ReconciliationSession> CompleteReconciliationAsync(string sessionId, string? completedBy = null)
        {
            var session = await GetReconciliationSessionAsync(sessionId);
            if (session == null) throw new ArgumentException("Session not found");

            if (!session.CanBeCompleted())
            {
                throw new InvalidOperationException("Session cannot be completed. There are unmatched items or the reconciliation is not balanced.");
            }

            session.MarkAsCompleted();
            session.ReconciledBy = completedBy;
            session.UpdatedBy = completedBy;

            // Mark all matched transactions as reconciled
            var matchedItems = session.ReconciliationItems.Where(ri => ri.Status == ReconciliationItemStatus.Matched && !string.IsNullOrEmpty(ri.TransactionId));
            foreach (var item in matchedItems)
            {
                if (!string.IsNullOrEmpty(item.TransactionId))
                {
                    var transaction = await _context.Transactions.FindAsync(item.TransactionId);
                    if (transaction != null)
                    {
                        transaction.Status = TransactionStatus.Reconciled;
                        transaction.MarkAsDirty();
                    }
                }
            }

            await _context.SaveChangesAsync();

            // Send notification
            await _notificationService.CreateNotificationAsync(
                "Reconciliation Completed",
                $"Reconciliation session '{session.SessionName}' has been completed successfully.",
                AppNotificationType.SystemAlert
            );

            return session;
        }

        public async Task<ReconciliationSession> CancelReconciliationAsync(string sessionId, string? cancelledBy = null, string? reason = null)
        {
            var session = await GetReconciliationSessionAsync(sessionId);
            if (session == null) throw new ArgumentException("Session not found");

            session.Status = ReconciliationStatus.Cancelled;
            session.Notes = string.IsNullOrEmpty(reason) ? session.Notes : $"{session.Notes}\nCancelled: {reason}";
            session.UpdatedBy = cancelledBy;
            session.MarkAsDirty();

            await _context.SaveChangesAsync();
            return session;
        }

        public async Task<bool> MarkItemAsClearedAsync(string itemId, string? clearedBy = null)
        {
            var item = await _context.ReconciliationItems.FindAsync(itemId);
            if (item == null) return false;

            item.MarkAsCleared();
            item.UpdatedBy = clearedBy;
            item.MarkAsDirty();

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkItemAsDisputedAsync(string itemId, string? notes = null)
        {
            var item = await _context.ReconciliationItems.FindAsync(itemId);
            if (item == null) return false;

            item.MarkAsDisputed();
            if (!string.IsNullOrEmpty(notes))
            {
                item.Notes = notes;
            }
            item.MarkAsDirty();

            await _context.SaveChangesAsync();
            return true;
        }

        #endregion

        #region Adjustments and Corrections

        public async Task<Transaction> CreateAdjustmentTransactionAsync(string accountId, decimal amount, string description, DateTime transactionDate, string? createdBy = null)
        {
            var transaction = new Transaction
            {
                Id = Guid.NewGuid().ToString(),
                AccountId = accountId,
                Amount = amount,
                Description = $"Reconciliation Adjustment: {description}",
                Date = transactionDate,
                CategoryId = "ADJUSTMENT", // Assuming there's an adjustment category
                Status = TransactionStatus.Reconciled,
                CreatedBy = createdBy ?? "System",
                CreatedAt = DateTime.UtcNow
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<ReconciliationItem> CreateAdjustmentItemAsync(string sessionId, decimal amount, string description, DateTime transactionDate, string? createdBy = null)
        {
            // Create the adjustment transaction first
            var session = await GetReconciliationSessionAsync(sessionId);
            if (session == null) throw new ArgumentException("Session not found");

            var transaction = await CreateAdjustmentTransactionAsync(session.AccountId, amount, description, transactionDate, createdBy);

            // Create the reconciliation item
            var item = await AddReconciliationItemAsync(
                sessionId,
                transaction.Id,
                transactionDate,
                description,
                amount,
                ReconciliationItemType.Adjustment,
                "ADJ-" + DateTime.Now.Ticks,
                false,
                false
            );

            // Automatically match and clear the item
            item.Status = ReconciliationItemStatus.Matched;
            item.MarkAsCleared();
            await UpdateReconciliationItemAsync(item);

            return item;
        }

        #endregion

        #region Reporting and Analytics

        public async Task<ReconciliationSummaryDto> GetReconciliationSummaryAsync(string accountId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.ReconciliationSessions.Where(rs => rs.AccountId == accountId);

            if (startDate.HasValue)
                query = query.Where(rs => rs.ReconciliationDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(rs => rs.ReconciliationDate <= endDate.Value);

            var sessions = await query.ToListAsync();
            var account = await _context.Accounts.FindAsync(accountId);

            return new ReconciliationSummaryDto
            {
                AccountId = accountId,
                AccountName = account?.Name ?? "Unknown Account",
                TotalSessions = sessions.Count,
                CompletedSessions = sessions.Count(s => s.Status == ReconciliationStatus.Completed),
                PendingSessions = sessions.Count(s => s.Status == ReconciliationStatus.InProgress),
                TotalReconciled = sessions.Where(s => s.Status == ReconciliationStatus.Completed).Sum(s => s.StatementEndingBalance),
                TotalDifferences = sessions.Sum(s => s.Difference),
                LastReconciliationDate = sessions.OrderByDescending(s => s.ReconciliationDate).FirstOrDefault()?.ReconciliationDate,
                CurrentBookBalance = await CalculateBookBalanceAsync(accountId, DateTime.UtcNow),
                UnreconciledTransactions = await _context.Transactions.CountAsync(t => t.AccountId == accountId && t.Status != TransactionStatus.Reconciled)
            };
        }

        public async Task<IEnumerable<ReconciliationSession>> GetReconciliationHistoryAsync(string accountId, DateTime? startDate = null, DateTime? endDate = null)
        {
            IQueryable<ReconciliationSession> query = _context.ReconciliationSessions
                .Where(rs => rs.AccountId == accountId)
                .Include(rs => rs.ReconciliationItems);

            if (startDate.HasValue)
                query = query.Where(rs => rs.ReconciliationDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(rs => rs.ReconciliationDate <= endDate.Value);

            return await query.OrderByDescending(rs => rs.ReconciliationDate).ToListAsync();
        }

        public async Task<IEnumerable<ReconciliationSession>> GetReconciliationHistoryAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            IQueryable<ReconciliationSession> query = _context.ReconciliationSessions
                .Include(rs => rs.ReconciliationItems);

            if (startDate.HasValue)
                query = query.Where(rs => rs.ReconciliationDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(rs => rs.ReconciliationDate <= endDate.Value);

            return await query.OrderByDescending(rs => rs.ReconciliationDate).ToListAsync();
        }

        public async Task<ReconciliationReportDto> GenerateReconciliationReportAsync(string sessionId)
        {
            var session = await GetReconciliationSessionAsync(sessionId);
            if (session == null) throw new ArgumentException("Session not found");

            var clearedItems = session.ReconciliationItems.Where(ri => ri.IsCleared).ToList();
            var unmatchedItems = session.ReconciliationItems.Where(ri => ri.Status == ReconciliationItemStatus.Unmatched).ToList();
            var disputedItems = session.ReconciliationItems.Where(ri => ri.Status == ReconciliationItemStatus.Disputed).ToList();

            return new ReconciliationReportDto
            {
                Session = session,
                ClearedItems = clearedItems,
                UnmatchedItems = unmatchedItems,
                DisputedItems = disputedItems.AsEnumerable(),
                TotalClearedAmount = clearedItems.Sum(i => i.Amount),
                TotalUnmatchedAmount = unmatchedItems.Sum(i => i.Amount),
                TotalDisputedAmount = disputedItems.Sum(i => i.Amount),
                ClearedItemsCount = clearedItems.Count,
                UnmatchedItemsCount = unmatchedItems.Count,
                DisputedItemsCount = disputedItems.Count
            };
        }

        public async Task<IEnumerable<ReconciliationItem>> GetDisputedItemsAsync(string? accountId = null)
        {
            var query = _context.ReconciliationItems
                .Include(ri => ri.ReconciliationSession)
                .Where(ri => ri.Status == ReconciliationItemStatus.Disputed);

            if (!string.IsNullOrEmpty(accountId))
            {
                query = query.Where(ri => ri.ReconciliationSession.AccountId == accountId);
            }

            return await query.OrderByDescending(ri => ri.UpdatedAt).ToListAsync();
        }

        #endregion

        #region Balance Verification

        public async Task<decimal> CalculateBookBalanceAsync(string accountId, DateTime asOfDate)
        {
            var transactions = await _context.Transactions
                .Where(t => t.AccountId == accountId && t.Date <= asOfDate)
                .ToListAsync();

            return transactions.Sum(t => t.Amount);
        }

        public async Task<bool> ValidateReconciliationAsync(string sessionId)
        {
            var validation = await GetReconciliationValidationAsync(sessionId);
            return validation.IsValid;
        }

        public async Task<ReconciliationValidationResult> GetReconciliationValidationAsync(string sessionId)
        {
            var session = await GetReconciliationSessionAsync(sessionId);
            if (session == null)
            {
                return new ReconciliationValidationResult
                {
                    IsValid = false,
                    ValidationErrors = new[] { "Session not found" }
                };
            }

            var errors = new List<string>();
            var warnings = new List<string>();

            // Recalculate difference
            var calculatedDifference = session.StatementEndingBalance - session.BookEndingBalance;
            var reportedDifference = session.Difference;

            if (Math.Abs(calculatedDifference - reportedDifference) > 0.01m)
            {
                errors.Add($"Calculated difference ({calculatedDifference:C}) does not match reported difference ({reportedDifference:C})");
            }

            // Check for unmatched items
            var unmatchedItems = session.ReconciliationItems.Where(ri => ri.Status == ReconciliationItemStatus.Unmatched).ToList();
            if (unmatchedItems.Any())
            {
                warnings.Add($"{unmatchedItems.Count} unmatched items found");
            }

            // Check for disputed items
            var disputedItems = session.ReconciliationItems.Where(ri => ri.Status == ReconciliationItemStatus.Disputed).ToList();
            if (disputedItems.Any())
            {
                warnings.Add($"{disputedItems.Count} disputed items found");
            }

            return new ReconciliationValidationResult
            {
                IsValid = !errors.Any(),
                CalculatedDifference = calculatedDifference,
                ReportedDifference = reportedDifference,
                ValidationErrors = errors,
                Warnings = warnings
            };
        }

        #endregion

        #region Bulk Operations

        public async Task<int> BulkMarkItemsAsClearedAsync(IEnumerable<string> itemIds, string? clearedBy = null)
        {
            var items = await _context.ReconciliationItems
                .Where(ri => itemIds.Contains(ri.Id))
                .ToListAsync();

            foreach (var item in items)
            {
                item.MarkAsCleared();
                item.UpdatedBy = clearedBy;
                item.MarkAsDirty();
            }

            await _context.SaveChangesAsync();
            return items.Count;
        }

        public async Task<int> BulkMatchTransactionsAsync(Dictionary<string, string> itemTransactionPairs, string? matchedBy = null)
        {
            var itemIds = itemTransactionPairs.Keys;
            var items = await _context.ReconciliationItems
                .Where(ri => itemIds.Contains(ri.Id))
                .ToListAsync();

            foreach (var item in items)
            {
                if (itemTransactionPairs.TryGetValue(item.Id, out var transactionId))
                {
                    item.TransactionId = transactionId;
                    item.Status = ReconciliationItemStatus.Matched;
                    item.UpdatedBy = matchedBy;
                    item.MarkAsDirty();
                }
            }

            await _context.SaveChangesAsync();
            return items.Count;
        }

        public async Task<IEnumerable<ReconciliationItem>> ImportBankStatementItemsAsync(string sessionId, IEnumerable<BankStatementItemDto> statementItems)
        {
            var items = new List<ReconciliationItem>();

            foreach (var statementItem in statementItems)
            {
                var item = await AddReconciliationItemAsync(
                    sessionId,
                    null,
                    statementItem.TransactionDate,
                    statementItem.Description,
                    statementItem.Amount,
                    statementItem.ItemType,
                    statementItem.Reference,
                    true, // isStatementOnly
                    false
                );

                items.Add(item);
            }

            return items;
        }

        #endregion

        #region Private Helper Methods

        private async Task PopulateSessionWithTransactionsAsync(string sessionId, string accountId, DateTime startDate, DateTime endDate)
        {
            var transactions = await GetUnreconciledTransactionsAsync(accountId, startDate, endDate);

            foreach (var transaction in transactions)
            {
                await AddReconciliationItemAsync(
                    sessionId,
                    transaction.Id,
                    transaction.Date,
                    transaction.Description,
                    transaction.Amount,
                    DetermineItemType(transaction),
                    transaction.Id,
                    false, // isStatementOnly
                    true   // isBookOnly
                );
            }
        }

        private ReconciliationItemType DetermineItemType(Transaction transaction)
        {
            if (transaction.Amount > 0)
                return ReconciliationItemType.Deposit;
            else if (transaction.Amount < 0)
                return ReconciliationItemType.Withdrawal;
            else
                return ReconciliationItemType.Other;
        }

        #endregion
    }
}