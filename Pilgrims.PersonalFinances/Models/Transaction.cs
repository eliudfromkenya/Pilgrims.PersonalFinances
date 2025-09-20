using System.ComponentModel.DataAnnotations;
using Pilgrims.PersonalFinances.Models.Enums;

namespace Pilgrims.PersonalFinances.Models;

/// <summary>
/// Represents a financial transaction with comprehensive tracking capabilities
/// </summary>
public class Transaction : BaseEntity
{
    private decimal _amount;
    private DateTime _date = DateTime.Today;
    private TransactionType _type = TransactionType.Expense;
    private TransactionStatus _status = TransactionStatus.Pending;
    private int _accountId;
    private Account? _account;
    private int? _categoryId;
    private Category? _category;
    private int? _transferToAccountId;
    private Account? _transferToAccount;
    private string? _payee;
    private string? _description;
    private string? _notes;
    private string _tags = string.Empty;
    private string? _referenceNumber;
    private DateTime? _clearedDate;
    private DateTime? _reconciledDate;
    private bool _isRecurring = false;
    private string? _recurringPattern;
    private bool _isSplit = false;
    private int? _parentTransactionId;
    private Transaction? _parentTransaction;
    private List<Transaction> _splitTransactions = new();
    private List<TransactionAttachment> _attachments = new();

    /// <summary>
    /// Transaction amount (positive for income, negative for expenses)
    /// </summary>
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount
    {
        get => _amount;
        set => SetProperty(ref _amount, value);
    }

    /// <summary>
    /// Date when the transaction occurred or is scheduled to occur
    /// </summary>
    [Required]
    public DateTime Date
    {
        get => _date;
        set => SetProperty(ref _date, value);
    }

    /// <summary>
    /// Type of transaction (Income, Expense, Transfer, Adjustment)
    /// </summary>
    [Required]
    public TransactionType Type
    {
        get => _type;
        set => SetProperty(ref _type, value);
    }

    /// <summary>
    /// Current status of the transaction
    /// </summary>
    [Required]
    public TransactionStatus Status
    {
        get => _status;
        set => SetProperty(ref _status, value);
    }

    /// <summary>
    /// ID of the source account for this transaction
    /// </summary>
    [Required]
    public int AccountId
    {
        get => _accountId;
        set => SetProperty(ref _accountId, value);
    }

    /// <summary>
    /// Navigation property to the source account
    /// </summary>
    public Account? Account
    {
        get => _account;
        set => SetProperty(ref _account, value);
    }

    /// <summary>
    /// ID of the category for this transaction (optional for transfers)
    /// </summary>
    public int? CategoryId
    {
        get => _categoryId;
        set => SetProperty(ref _categoryId, value);
    }

    /// <summary>
    /// Navigation property to the transaction category
    /// </summary>
    public Category? Category
    {
        get => _category;
        set => SetProperty(ref _category, value);
    }

    /// <summary>
    /// ID of the destination account for transfer transactions
    /// </summary>
    public int? TransferToAccountId
    {
        get => _transferToAccountId;
        set => SetProperty(ref _transferToAccountId, value);
    }

    /// <summary>
    /// Navigation property to the destination account for transfers
    /// </summary>
    public Account? TransferToAccount
    {
        get => _transferToAccount;
        set => SetProperty(ref _transferToAccount, value);
    }

    /// <summary>
    /// Name of the payee or payer (max 200 characters)
    /// </summary>
    [StringLength(200)]
    public string? Payee
    {
        get => _payee;
        set => SetProperty(ref _payee, value);
    }

    /// <summary>
    /// Brief description of the transaction (max 200 characters)
    /// </summary>
    [StringLength(200)]
    public string? Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    /// <summary>
    /// Additional notes or details (max 500 characters)
    /// </summary>
    [StringLength(500)]
    public string? Notes
    {
        get => _notes;
        set => SetProperty(ref _notes, value);
    }

    /// <summary>
    /// Comma-separated tags for searching and filtering
    /// </summary>
    public string Tags
    {
        get => _tags;
        set => SetProperty(ref _tags, value ?? string.Empty);
    }

    /// <summary>
    /// Reference number or check number
    /// </summary>
    [StringLength(50)]
    public string? ReferenceNumber
    {
        get => _referenceNumber;
        set => SetProperty(ref _referenceNumber, value);
    }

    /// <summary>
    /// Date when the transaction was cleared by the bank
    /// </summary>
    public DateTime? ClearedDate
    {
        get => _clearedDate;
        set => SetProperty(ref _clearedDate, value);
    }

    /// <summary>
    /// Date when the transaction was reconciled
    /// </summary>
    public DateTime? ReconciledDate
    {
        get => _reconciledDate;
        set => SetProperty(ref _reconciledDate, value);
    }

    /// <summary>
    /// Whether this is a recurring transaction
    /// </summary>
    public bool IsRecurring
    {
        get => _isRecurring;
        set => SetProperty(ref _isRecurring, value);
    }

    /// <summary>
    /// Pattern for recurring transactions (JSON format)
    /// </summary>
    public string? RecurringPattern
    {
        get => _recurringPattern;
        set => SetProperty(ref _recurringPattern, value);
    }

    /// <summary>
    /// Whether this transaction is split across multiple categories
    /// </summary>
    public bool IsSplit
    {
        get => _isSplit;
        set => SetProperty(ref _isSplit, value);
    }

    /// <summary>
    /// ID of the parent transaction (for split transaction children)
    /// </summary>
    public int? ParentTransactionId
    {
        get => _parentTransactionId;
        set => SetProperty(ref _parentTransactionId, value);
    }

    /// <summary>
    /// Navigation property to the parent transaction
    /// </summary>
    public Transaction? ParentTransaction
    {
        get => _parentTransaction;
        set => SetProperty(ref _parentTransaction, value);
    }

    /// <summary>
    /// Navigation property to split transaction children
    /// </summary>
    public List<Transaction> SplitTransactions
    {
        get => _splitTransactions;
        set => SetProperty(ref _splitTransactions, value);
    }

    /// <summary>
    /// Navigation property to transaction attachments (receipts, etc.)
    /// </summary>
    public List<TransactionAttachment> Attachments
    {
        get => _attachments;
        set => SetProperty(ref _attachments, value);
    }

    /// <summary>
    /// Gets the effective amount based on transaction type
    /// (negative for expenses, positive for income)
    /// </summary>
    public decimal EffectiveAmount
    {
        get
        {
            return Type switch
            {
                TransactionType.Income => Amount,
                TransactionType.Expense => -Amount,
                TransactionType.Transfer => Type == TransactionType.Transfer ? -Amount : Amount,
                TransactionType.Adjustment => Amount,
                _ => Amount
            };
        }
    }

    /// <summary>
    /// Gets whether this transaction is in the future
    /// </summary>
    public bool IsFuture => Date > DateTime.Today;

    /// <summary>
    /// Gets whether this transaction is overdue (future date but past due)
    /// </summary>
    public bool IsOverdue => IsFuture && Status == TransactionStatus.Pending && Date < DateTime.Today;

    /// <summary>
    /// Gets the list of tags as a string array
    /// </summary>
    public string[] TagList => Tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                   .Select(t => t.Trim())
                                   .Where(t => !string.IsNullOrEmpty(t))
                                   .ToArray();

    /// <summary>
    /// Gets the display name for the transaction
    /// </summary>
    public string DisplayName
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(Description))
                return Description;
            
            if (!string.IsNullOrWhiteSpace(Payee))
                return Payee;
            
            return Type switch
            {
                TransactionType.Transfer => $"Transfer to {TransferToAccount?.Name ?? "Unknown Account"}",
                TransactionType.Adjustment => "Balance Adjustment",
                _ => Category?.Name ?? "Uncategorized"
            };
        }
    }

    /// <summary>
    /// Gets the total amount of split transactions
    /// </summary>
    public decimal SplitTotal => SplitTransactions.Sum(st => st.Amount);

    /// <summary>
    /// Gets whether the split amounts match the parent amount
    /// </summary>
    public bool IsSplitBalanced => !IsSplit || Math.Abs(Amount - SplitTotal) < 0.01m;

    /// <summary>
    /// Validates the transaction data
    /// </summary>
    /// <returns>List of validation errors</returns>
    public List<string> Validate()
    {
        var errors = new List<string>();

        if (Amount <= 0)
            errors.Add("Amount must be greater than 0");

        if (AccountId <= 0)
            errors.Add("Account is required");

        if (Type == TransactionType.Transfer && TransferToAccountId == null)
            errors.Add("Transfer destination account is required for transfer transactions");

        if (Type == TransactionType.Transfer && TransferToAccountId == AccountId)
            errors.Add("Cannot transfer to the same account");

        if (Type != TransactionType.Transfer && CategoryId == null)
            errors.Add("Category is required for non-transfer transactions");

        if (Payee?.Length > 200)
            errors.Add("Payee name cannot exceed 200 characters");

        if (Description?.Length > 200)
            errors.Add("Description cannot exceed 200 characters");

        if (Notes?.Length > 500)
            errors.Add("Notes cannot exceed 500 characters");

        if (ReferenceNumber?.Length > 50)
            errors.Add("Reference number cannot exceed 50 characters");

        if (IsSplit && !IsSplitBalanced)
            errors.Add("Split transaction amounts must equal the total amount");

        if (ParentTransactionId != null && !IsSplit)
            errors.Add("Only split transactions can have a parent transaction");

        if (Status == TransactionStatus.Cleared && ClearedDate == null)
            errors.Add("Cleared date is required for cleared transactions");

        if (Status == TransactionStatus.Reconciled && ReconciledDate == null)
            errors.Add("Reconciled date is required for reconciled transactions");

        return errors;
    }

    /// <summary>
    /// Marks the transaction as cleared
    /// </summary>
    public void MarkAsCleared()
    {
        Status = TransactionStatus.Cleared;
        ClearedDate = DateTime.UtcNow;
        TouchUpdatedAt();
    }

    /// <summary>
    /// Marks the transaction as reconciled
    /// </summary>
    public void MarkAsReconciled()
    {
        Status = TransactionStatus.Reconciled;
        ReconciledDate = DateTime.UtcNow;
        if (ClearedDate == null)
            ClearedDate = DateTime.UtcNow;
        TouchUpdatedAt();
    }

    /// <summary>
    /// Adds a tag to the transaction
    /// </summary>
    /// <param name="tag">Tag to add</param>
    public void AddTag(string tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
            return;

        var currentTags = TagList.ToList();
        if (!currentTags.Contains(tag.Trim(), StringComparer.OrdinalIgnoreCase))
        {
            currentTags.Add(tag.Trim());
            Tags = string.Join(", ", currentTags);
        }
    }

    /// <summary>
    /// Removes a tag from the transaction
    /// </summary>
    /// <param name="tag">Tag to remove</param>
    public void RemoveTag(string tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
            return;

        var currentTags = TagList.ToList();
        currentTags.RemoveAll(t => string.Equals(t, tag.Trim(), StringComparison.OrdinalIgnoreCase));
        Tags = string.Join(", ", currentTags);
    }

    /// <summary>
    /// Creates a duplicate of this transaction
    /// </summary>
    /// <returns>New transaction with copied properties</returns>
    public Transaction Duplicate()
    {
        return new Transaction
        {
            Amount = Amount,
            Date = DateTime.Today, // Use today's date for duplicates
            Type = Type,
            Status = TransactionStatus.Pending, // Reset status
            AccountId = AccountId,
            CategoryId = CategoryId,
            TransferToAccountId = TransferToAccountId,
            Payee = Payee,
            Description = Description,
            Notes = Notes,
            Tags = Tags,
            ReferenceNumber = null, // Clear reference number
            IsRecurring = IsRecurring,
            RecurringPattern = RecurringPattern
        };
    }
}