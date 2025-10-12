using Microsoft.EntityFrameworkCore;
using Pilgrims.PersonalFinances.Data;
using Pilgrims.PersonalFinances.Core.Models;
using Pilgrims.PersonalFinances.Core.Models.Enums;
using Pilgrims.PersonalFinances.Core.Services.Interfaces;
using Pilgrims.PersonalFinances.Core.Logging;
using Pilgrims.PersonalFinances.Core.Interfaces;
using System.Text.Json;

namespace Pilgrims.PersonalFinances.Core.Services;

/// <summary>
/// Service implementation for managing transactions with comprehensive CRUD operations
/// </summary>
public class TransactionService : ITransactionService
{
    private readonly PersonalFinanceContext _context;
    private readonly ILoggingService _logger;
    private readonly ICurrencyService _currencyService;

    public TransactionService(PersonalFinanceContext context, ILoggingService logger, ICurrencyService currencyService)
    {
        _context = context;
        _logger = logger;
        _currencyService = currencyService;
    }

    #region Basic CRUD Operations

    public async Task<IEnumerable<Transaction>> GetAllTransactionsAsync()
    {
        return await _context.Transactions
            .Include(t => t.Account)
            .Include(t => t.Category)
            .Include(t => t.TransferToAccount)
            .Include(t => t.SplitTransactions)
            .Include(t => t.Attachments)
            .OrderByDescending(t => t.Date)
            .ToListAsync();
    }

    public async Task<Transaction?> GetTransactionByIdAsync(string id)
    {
        return await _context.Transactions
            .Include(t => t.Account)
            .Include(t => t.Category)
            .Include(t => t.TransferToAccount)
            .Include(t => t.SplitTransactions)
            .Include(t => t.Attachments)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Transaction> CreateTransactionAsync(Transaction transaction)
    {
        // Validate transaction
        var validationErrors = await GetValidationErrorsAsync(transaction);
        if (validationErrors.Any())
        {
            throw new ArgumentException($"Transaction validation failed: {string.Join(", ", validationErrors)}");
        }

        // Set creation timestamp
        transaction.CreatedAt = DateTime.UtcNow;
        transaction.TouchUpdatedAt();

        // Check for potential duplicates
        var duplicates = await FindPotentialDuplicatesAsync(transaction);
        if (duplicates.Any())
        {
            // Log potential duplicate warning
            _logger.LogWarning("Potential duplicate transaction detected for transaction: {Description}", transaction.Description);
        }

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync().ConfigureAwait(false);

        // Log success
        _logger.LogInformation("Transaction Created: {Description} with amount {Amount}", transaction.Description, transaction.Amount);

        return transaction;
    }

    public async Task<Transaction> UpdateTransactionAsync(Transaction transaction)
    {
        var existingTransaction = await GetTransactionByIdAsync(transaction.Id);
        if (existingTransaction == null)
        {
            throw new ArgumentException("Transaction not found");
        }

        // Validate transaction
        var validationErrors = await GetValidationErrorsAsync(transaction);
        if (validationErrors.Any())
        {
            throw new ArgumentException($"Transaction validation failed: {string.Join(", ", validationErrors)}");
        }

        transaction.TouchUpdatedAt();
        _context.Entry(existingTransaction).CurrentValues.SetValues(transaction);
        await _context.SaveChangesAsync().ConfigureAwait(false);

        // Log success
        _logger.LogInformation("Transaction Updated: {Description} with amount {Amount}", transaction.Description, transaction.Amount);

        return transaction;
    }

    public async Task<bool> DeleteTransactionAsync(string id)
    {
        var transaction = await GetTransactionByIdAsync(id);
        if (transaction == null) return false;

        _context.Transactions.Remove(transaction);
        return await _context.SaveChangesAsync().ConfigureAwait(false) > 0;
    }

    public async Task<bool> DeleteTransactionsAsync(IEnumerable<string> ids)
    {
        var transactions = await _context.Transactions
            .Where(t => ids.Contains(t.Id))
            .ToListAsync();

        if (!transactions.Any()) return false;

        _context.Transactions.RemoveRange(transactions);
        var result = await _context.SaveChangesAsync().ConfigureAwait(false);

        return result > 0;
    }

    #endregion

    #region Filtering and Search

    public async Task<IEnumerable<Transaction>> GetTransactionsByAccountAsync(string accountId)
    {
        return await _context.Transactions
            .Include(t => t.Account)
            .Include(t => t.Category)
            .Include(t => t.TransferToAccount)
            .Where(t => t.AccountId == accountId)
            .OrderByDescending(t => t.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Transactions
            .Include(t => t.Account)
            .Include(t => t.Category)
            .Include(t => t.TransferToAccount)
            .Where(t => t.Date >= startDate && t.Date <= endDate)
            .OrderByDescending(t => t.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetTransactionsByCategoryAsync(string categoryId)
    {
        return await _context.Transactions
            .Include(t => t.Account)
            .Include(t => t.Category)
            .Include(t => t.TransferToAccount)
            .Where(t => t.CategoryId == categoryId)
            .OrderByDescending(t => t.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetTransactionsByTypeAsync(TransactionType type)
    {
        return await _context.Transactions
            .Include(t => t.Account)
            .Include(t => t.Category)
            .Include(t => t.TransferToAccount)
            .Where(t => t.Type == type)
            .OrderByDescending(t => t.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetTransactionsByStatusAsync(TransactionStatus status)
    {
        return await _context.Transactions
            .Include(t => t.Account)
            .Include(t => t.Category)
            .Include(t => t.TransferToAccount)
            .Where(t => t.Status == status)
            .OrderByDescending(t => t.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> SearchTransactionsAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetAllTransactionsAsync();

        var lowerSearchTerm = searchTerm.ToLower();
        return await _context.Transactions
            .Include(t => t.Account)
            .Include(t => t.Category)
            .Include(t => t.TransferToAccount)
            .Where(t => t.Description.ToLower().Contains(lowerSearchTerm) ||
                       t.Notes.ToLower().Contains(lowerSearchTerm) ||
                       t.Payee.ToLower().Contains(lowerSearchTerm) ||
                       t.ReferenceNumber.ToLower().Contains(lowerSearchTerm) ||
                       t.Tags.ToLower().Contains(lowerSearchTerm))
            .OrderByDescending(t => t.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetTransactionsByPayeeAsync(string payee)
    {
        return await _context.Transactions
            .Include(t => t.Account)
            .Include(t => t.Category)
            .Include(t => t.TransferToAccount)
            .Where(t => t.Payee.ToLower().Contains(payee.ToLower()))
            .OrderByDescending(t => t.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetTransactionsByTagsAsync(IEnumerable<string> tags)
    {
        var tagList = tags.Select(t => t.ToLower()).ToList();
        return await _context.Transactions
            .Include(t => t.Account)
            .Include(t => t.Category)
            .Include(t => t.TransferToAccount)
            .Where(t => tagList.Any(tag => t.Tags.ToLower().Contains(tag)))
            .OrderByDescending(t => t.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetFilteredTransactionsAsync(
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
        int take = 50)
    {
        var query = _context.Transactions
            .Include(t => t.Account)
            .Include(t => t.Category)
            .Include(t => t.TransferToAccount)
            .AsQueryable();

        if (!string.IsNullOrEmpty(accountId))
            query = query.Where(t => t.AccountId == accountId);

        if (startDate.HasValue)
            query = query.Where(t => t.Date >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(t => t.Date <= endDate.Value);

        if (!string.IsNullOrEmpty(categoryId))
            query = query.Where(t => t.CategoryId == categoryId);

        if (type.HasValue)
            query = query.Where(t => t.Type == type.Value);

        if (status.HasValue)
            query = query.Where(t => t.Status == status.Value);

        if (!string.IsNullOrEmpty(payee))
            query = query.Where(t => t.Payee.ToLower().Contains(payee.ToLower()));

        if (tags != null && tags.Any())
        {
            var tagList = tags.Select(t => t.ToLower()).ToList();
            query = query.Where(t => tagList.Any(tag => t.Tags.ToLower().Contains(tag)));
        }

        if (minAmount.HasValue)
            query = query.Where(t => Math.Abs(t.Amount) >= minAmount.Value);

        if (maxAmount.HasValue)
            query = query.Where(t => Math.Abs(t.Amount) <= maxAmount.Value);

        if (!string.IsNullOrEmpty(searchTerm))
        {
            var lowerSearchTerm = searchTerm.ToLower();
            query = query.Where(t => t.Description.ToLower().Contains(lowerSearchTerm) ||
                                   t.Notes.ToLower().Contains(lowerSearchTerm) ||
                                   t.Payee.ToLower().Contains(lowerSearchTerm) ||
                                   t.ReferenceNumber.ToLower().Contains(lowerSearchTerm));
        }

        return await query
            .OrderByDescending(t => t.Date)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    #endregion

    #region Split Transactions

    public async Task<IEnumerable<SplitTransaction>> GetSplitTransactionsAsync(string parentTransactionId)
    {
        var splitTransactions = await _context.SplitTransactions
            .Include(st => st.Category)
            .Where(st => st.TransactionId == parentTransactionId)
            .ToListAsync();

        foreach (var splitTransaction in splitTransactions)
        {
            splitTransaction.FormattedAmount = await _currencyService.FormatAmountAsync(splitTransaction.Amount);
        }

        return splitTransactions;
    }

    public async Task<SplitTransaction> CreateSplitTransactionAsync(SplitTransaction splitTransaction)
    {
        splitTransaction.CreatedAt = DateTime.UtcNow;
        splitTransaction.TouchUpdatedAt();

        _context.SplitTransactions.Add(splitTransaction);
        await _context.SaveChangesAsync().ConfigureAwait(false);

        return splitTransaction;
    }

    public async Task<SplitTransaction> UpdateSplitTransactionAsync(SplitTransaction splitTransaction)
    {
        var existing = await _context.SplitTransactions.FindAsync(splitTransaction.Id);
        if (existing == null)
            throw new ArgumentException("Split transaction not found");

        splitTransaction.TouchUpdatedAt();
        _context.Entry(existing).CurrentValues.SetValues(splitTransaction);
        await _context.SaveChangesAsync().ConfigureAwait(false);

        return splitTransaction;
    }

    public async Task<bool> DeleteSplitTransactionAsync(string splitTransactionId)
    {
        var splitTransaction = await _context.SplitTransactions.FindAsync(splitTransactionId);
        if (splitTransaction == null) return false;

        _context.SplitTransactions.Remove(splitTransaction);
        return await _context.SaveChangesAsync().ConfigureAwait(false) > 0;
    }

    #endregion

    #region Transaction Templates

    public async Task<List<TransactionTemplate>> GetTransactionTemplatesAsync()
    {
        // Mock data for now - replace with actual database call
        var templates = new List<TransactionTemplate>
        {
            new TransactionTemplate
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Monthly Rent",
                Description = "Monthly apartment rent payment",
                Type = TransactionType.Expense,
                Amount = 1200.00m,
                CategoryId = "housing-category-id",
                Payee = "Property Management",
                Tags = "rent,housing,monthly",
                UsageCount = 12,
                CreatedAt = DateTime.UtcNow.AddMonths(-6),
               //UpdatedAt = DateTime.UtcNow.AddDays(-30)
            },
            new TransactionTemplate
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Salary Deposit",
                Description = "Monthly salary from employer",
                Type = TransactionType.Income,
                Amount = 5000.00m,
                CategoryId = "salary-category-id",
                Payee = "ABC Company",
                Tags = "salary,income,monthly",
                UsageCount = 24,
                CreatedAt = DateTime.UtcNow.AddYears(-1),
                //UpdatedAt = DateTime.UtcNow.AddDays(-15)
            }
        };

        foreach (var template in templates)
        {
            template.FormattedAmount = await _currencyService.FormatAmountAsync(template.Amount);
        }

        return templates;
    }

    public async Task<TransactionTemplate> CreateTransactionTemplateAsync(TransactionTemplate template)
    {
        // Mock implementation - replace with actual database operations
        template.Id = Guid.NewGuid().ToString();
        template.CreatedAt = DateTime.UtcNow;
        template.TouchUpdatedAt();
        
        // In a real implementation, save to database here
        // await _context.TransactionTemplates.AddAsync(template);
        // await _context.SaveChangesAsync();
        
        // Log success
        _logger.LogInformation("Transaction template created successfully with name: {TemplateName}", template.Name);
        return template;
    }

    public async Task<TransactionTemplate> UpdateTransactionTemplateAsync(TransactionTemplate template)
    {
        // Mock implementation - replace with actual database operations
        template.TouchUpdatedAt();
        
        // In a real implementation, update in database here
        // _context.TransactionTemplates.Update(template);
        // await _context.SaveChangesAsync();
        
        // Log success
        _logger.LogInformation("Transaction template updated successfully: {TemplateName}", template.Name);
        return template;
    }

    public async Task<bool> DeleteTransactionTemplateAsync(string? templateId)
    {
        // Mock implementation - replace with actual database operations
        // In a real implementation:
        // var template = await _context.TransactionTemplates.FindAsync(templateId);
        // if (template != null)
        // {
        //     _context.TransactionTemplates.Remove(template);
        //     await _context.SaveChangesAsync();
        //     return true;
        // }
        // return false;
        
        // Log success
        _logger.LogInformation("Transaction template deleted successfully with ID: {TemplateId}", templateId);
        return true;
    }

    public async Task<Transaction> CreateTransactionFromTemplateAsync(string templateId, DateTime? transactionDate = null)
    {
        // Mock implementation - replace with actual database operations
        var templates = await GetTransactionTemplatesAsync();
        var template = templates.FirstOrDefault(t => t.Id == templateId);
        
        if (template == null)
            throw new ArgumentException("Template not found", nameof(templateId));

        var transaction = new Transaction
        {
            Amount = template.Amount,
            Type = template.Type,
            CategoryId = template.CategoryId,
            Description = template.Description,
            Payee = template.Payee,
            Tags = template.Tags,
            Date = transactionDate ?? DateTime.Today,
            Status = TransactionStatus.Pending
        };

        // Increment template usage count
        template.UsageCount++;
        await UpdateTransactionTemplateAsync(template);

        return await CreateTransactionAsync(transaction);
    }

    public async Task<List<Category>> GetCategoriesAsync()
    {
        // Return existing categories from the database
        return await _context.Categories
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    #endregion

    #region Transaction Templates

    #endregion

    #region Bulk Operations

    public async Task<bool> BulkUpdateTransactionStatusAsync(IEnumerable<string> transactionIds, TransactionStatus status)
    {
        var transactions = await _context.Transactions
            .Where(t => transactionIds.Contains(t.Id))
            .ToListAsync();

        if (!transactions.Any()) return false;

        foreach (var transaction in transactions)
        {
            transaction.Status = status;
            transaction.TouchUpdatedAt();
        }

        var result = await _context.SaveChangesAsync().ConfigureAwait(false) > 0;

        if (result)
        {
            // Log success
            _logger.LogInformation("Bulk Update Complete: {Count} transactions updated to status {Status}", transactions.Count, status);
        }

        return result;
    }

    public async Task<bool> BulkUpdateTransactionCategoryAsync(IEnumerable<string> transactionIds, string categoryId)
    {
        var transactions = await _context.Transactions
            .Where(t => transactionIds.Contains(t.Id))
            .ToListAsync();

        if (!transactions.Any()) return false;

        foreach (var transaction in transactions)
        {
            transaction.CategoryId = categoryId;
            transaction.TouchUpdatedAt();
        }

        var result = await _context.SaveChangesAsync().ConfigureAwait(false) > 0;

        if (result)
        {
            // Log success
            _logger.LogInformation("Bulk Update Complete: {Count} transactions updated with new category {CategoryId}", transactions.Count, categoryId);
        }

        return result;
    }

    public async Task<bool> BulkDeleteTransactionsAsync(IEnumerable<string> transactionIds)
    {
        return await DeleteTransactionsAsync(transactionIds);
    }

    #endregion

    #region Duplicate Detection

    public async Task<IEnumerable<Transaction>> FindPotentialDuplicatesAsync(Transaction transaction)
    {
        var dateRange = TimeSpan.FromDays(3); // Look for duplicates within 3 days
        var startDate = transaction.Date.AddDays(-dateRange.Days);
        var endDate = transaction.Date.AddDays(dateRange.Days);

        return await _context.Transactions
            .Where(t => t.Id != transaction.Id &&
                       t.AccountId == transaction.AccountId &&
                       t.Amount == transaction.Amount &&
                       t.Date >= startDate &&
                       t.Date <= endDate &&
                       (t.Payee == transaction.Payee || 
                        t.Description.Contains(transaction.Description) ||
                        transaction.Description.Contains(t.Description)))
            .ToListAsync();
    }

    public async Task<bool> MarkTransactionAsNotDuplicateAsync(string transactionId, string potentialDuplicateId)
    {
        // This could be implemented with a separate table to track non-duplicate pairs
        // For now, we'll just return true as a placeholder
        return await Task.FromResult(true);
    }

    #endregion

    #region Attachments

    public async Task<TransactionAttachment> AddAttachmentAsync(string transactionId, string fileName, string contentType, byte[] fileData)
    {
        var attachment = new TransactionAttachment
        {
            Id = Guid.NewGuid().ToString(),
            TransactionId = transactionId,
            FileName = fileName,
            OriginalFileName = fileName,
            ContentType = contentType,
            FileSize = fileData.Length,
            FilePath = $"attachments/{transactionId}/{fileName}",
            UploadedDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };
        attachment.TouchUpdatedAt();

        // In a real implementation, you would save the file to storage here
        // For now, we'll just save the metadata

        _context.TransactionAttachments.Add(attachment);
        await _context.SaveChangesAsync().ConfigureAwait(false);

        // Log success
        _logger.LogInformation("Attachment Added: {FileName} for transaction {TransactionId}", attachment.FileName, attachment.TransactionId);

        return attachment;
    }

    public async Task<bool> DeleteAttachmentAsync(string attachmentId)
    {
        var attachment = await _context.TransactionAttachments.FindAsync(attachmentId);
        if (attachment == null) return false;

        _context.TransactionAttachments.Remove(attachment);
        var result = await _context.SaveChangesAsync().ConfigureAwait(false) > 0;

        if (result)
        {
            // Log success
            _logger.LogInformation("Attachment Deleted: {FileName} from transaction {TransactionId}", attachment.FileName, attachment.TransactionId);
        }

        return result;
    }

    public async Task<IEnumerable<TransactionAttachment>> GetTransactionAttachmentsAsync(string transactionId)
    {
        return await _context.TransactionAttachments
            .Where(a => a.TransactionId == transactionId)
            .OrderByDescending(a => a.UploadedDate)
            .ToListAsync();
    }

    #endregion

    #region Statistics and Analytics

    public async Task<decimal> GetAccountBalanceAsync(string accountId, DateTime? asOfDate = null)
    {
        var query = _context.Transactions
            .Where(t => t.AccountId == accountId && t.Status != TransactionStatus.Cancelled);

        if (asOfDate.HasValue)
            query = query.Where(t => t.Date <= asOfDate.Value);

        var transactions = await query.ToListAsync();
        return transactions.Sum(t => t.EffectiveAmount);
    }

    public async Task<decimal> GetTotalIncomeAsync(string accountId, DateTime startDate, DateTime endDate)
    {
        return await _context.Transactions
            .Where(t => t.AccountId == accountId &&
                       t.Type == TransactionType.Income &&
                       t.Date >= startDate &&
                       t.Date <= endDate &&
                       t.Status != TransactionStatus.Cancelled)
            .SumAsync(t => t.Amount);
    }

    public async Task<decimal> GetTotalExpensesAsync(string accountId, DateTime startDate, DateTime endDate)
    {
        return await _context.Transactions
            .Where(t => t.AccountId == accountId &&
                       t.Type == TransactionType.Expense &&
                       t.Date >= startDate &&
                       t.Date <= endDate &&
                       t.Status != TransactionStatus.Cancelled)
            .SumAsync(t => Math.Abs(t.Amount));
    }

    public async Task<Dictionary<string, decimal>> GetExpensesByCategoryAsync(string accountId, DateTime startDate, DateTime endDate)
    {
        return await _context.Transactions
            .Include(t => t.Category)
            .Where(t => t.AccountId == accountId &&
                       t.Type == TransactionType.Expense &&
                       t.Date >= startDate &&
                       t.Date <= endDate &&
                       t.Status != TransactionStatus.Cancelled)
            .GroupBy(t => t.Category.Name)
            .Select(g => new { Category = g.Key, Total = g.Sum(t => Math.Abs(t.Amount)) })
            .ToDictionaryAsync(x => x.Category, x => x.Total);
    }

    public async Task<Dictionary<DateTime, decimal>> GetDailyBalanceHistoryAsync(string accountId, DateTime startDate, DateTime endDate)
    {
        var transactions = await _context.Transactions
            .Where(t => t.AccountId == accountId &&
                       t.Date >= startDate &&
                       t.Date <= endDate &&
                       t.Status != TransactionStatus.Cancelled)
            .OrderBy(t => t.Date)
            .ToListAsync();

        var balanceHistory = new Dictionary<DateTime, decimal>();
        var runningBalance = 0m;

        for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
        {
            var dayTransactions = transactions.Where(t => t.Date.Date == date);
            runningBalance += dayTransactions.Sum(t => t.EffectiveAmount);
            balanceHistory[date] = runningBalance;
        }

        return balanceHistory;
    }

    #endregion

    #region Recurring Transactions

    public async Task<IEnumerable<Transaction>> GetRecurringTransactionsAsync()
    {
        return await _context.Transactions
            .Include(t => t.Account)
            .Include(t => t.Category)
            .Where(t => t.IsRecurring)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> ProcessRecurringTransactionsAsync(DateTime processDate)
    {
        var recurringTransactions = await GetRecurringTransactionsAsync();
        var processedTransactions = new List<Transaction>();

        foreach (var recurring in recurringTransactions)
        {
            // Simple logic - in a real implementation, you'd have more sophisticated recurring logic
            if (ShouldCreateRecurringTransaction(recurring, processDate))
            {
                var newTransaction = await CreateTransactionFromRecurringAsync(recurring, processDate);
                processedTransactions.Add(newTransaction);
            }
        }

        return processedTransactions;
    }

    public async Task<Transaction> CreateRecurringTransactionAsync(Transaction recurringTransaction)
    {
        recurringTransaction.IsRecurring = true;
        return await CreateTransactionAsync(recurringTransaction);
    }

    private bool ShouldCreateRecurringTransaction(Transaction recurring, DateTime processDate)
    {
        // Simplified logic - implement proper recurring pattern matching
        return true;
    }

    private async Task<Transaction> CreateTransactionFromRecurringAsync(Transaction recurring, DateTime transactionDate)
    {
        var newTransaction = new Transaction
        {
            Id = Guid.NewGuid().ToString(),
            Amount = recurring.Amount,
            Date = transactionDate,
            Type = recurring.Type,
            Status = TransactionStatus.Pending,
            AccountId = recurring.AccountId,
            CategoryId = recurring.CategoryId,
            TransferToAccountId = recurring.TransferToAccountId,
            Payee = recurring.Payee,
            Description = $"{recurring.Description} (Recurring)",
            Notes = recurring.Notes,
            Tags = recurring.Tags,
            IsRecurring = false,
            CreatedAt = DateTime.UtcNow
        };

        newTransaction.TouchUpdatedAt();

        return await CreateTransactionAsync(newTransaction);
    }

    #endregion

    #region Validation

    public async Task<bool> ValidateTransactionAsync(Transaction transaction)
    {
        var errors = await GetValidationErrorsAsync(transaction);
        return !errors.Any();
    }

    public async Task<IEnumerable<string>> GetValidationErrorsAsync(Transaction transaction)
    {
        var errors = new List<string>();

        if (transaction.Amount == 0)
            errors.Add("Transaction amount cannot be zero");

        if (string.IsNullOrWhiteSpace(transaction.AccountId))
            errors.Add("Account is required");

        if (transaction.Type == TransactionType.Transfer && string.IsNullOrWhiteSpace(transaction.TransferToAccountId))
            errors.Add("Transfer destination account is required for transfer transactions");

        if (transaction.Type == TransactionType.Transfer && transaction.AccountId == transaction.TransferToAccountId)
            errors.Add("Cannot transfer to the same account");

        // Check if account exists
        if (!string.IsNullOrWhiteSpace(transaction.AccountId))
        {
            var accountExists = await _context.Accounts.AnyAsync(a => a.Id == transaction.AccountId);
            if (!accountExists)
                errors.Add("Selected account does not exist");
        }

        // Check if category exists (if specified)
        if (!string.IsNullOrWhiteSpace(transaction.CategoryId))
        {
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == transaction.CategoryId);
            if (!categoryExists)
                errors.Add("Selected category does not exist");
        }

        return errors;
    }

    #endregion

    #region Import/Export

    public async Task<IEnumerable<Transaction>> ImportTransactionsAsync(IEnumerable<Transaction> transactions)
    {
        var importedTransactions = new List<Transaction>();

        foreach (var transaction in transactions)
        {
            try
            {
                var imported = await CreateTransactionAsync(transaction);
                importedTransactions.Add(imported);
            }
            catch (Exception ex)
            {
                // Log error
                _logger.LogError("Import Error: Failed to import transaction: {ErrorMessage}", ex.Message);
            }
        }

        // Log success
        _logger.LogInformation("Import Complete: {Count} transactions imported successfully", importedTransactions.Count);

        return importedTransactions;
    }

    public async Task<byte[]> ExportTransactionsAsync(IEnumerable<string> transactionIds, string format = "csv")
    {
        var transactions = await _context.Transactions
            .Include(t => t.Account)
            .Include(t => t.Category)
            .Where(t => transactionIds.Contains(t.Id))
            .ToListAsync();

        if (format.ToLower() == "json")
        {
            var json = JsonSerializer.Serialize(transactions, new JsonSerializerOptions { WriteIndented = true });
            return System.Text.Encoding.UTF8.GetBytes(json);
        }

        // Default to CSV
        var csv = "Date,Amount,Type,Status,Account,Category,Payee,Description,Notes\n";
        foreach (var transaction in transactions)
        {
            csv += $"{transaction.Date:yyyy-MM-dd},{transaction.Amount},{transaction.Type},{transaction.Status}," +
                   $"{transaction.Account?.Name},{transaction.Category?.Name},{transaction.Payee}," +
                   $"\"{transaction.Description}\",\"{transaction.Notes}\"\n";
        }

        return System.Text.Encoding.UTF8.GetBytes(csv);
    }

    #endregion
}
