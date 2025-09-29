using Pilgrims.PersonalFinances.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pilgrims.PersonalFinances.Models
{
    public class Account : BaseEntity
    {
        /// <summary>
        /// User ID for the account owner
        /// </summary>
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public AccountType AccountType { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal InitialBalance { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CurrentBalance { get; set; }

        [StringLength(10)]
        public string Currency { get; set; } = "USD";

        public bool IsActive { get; set; } = true;

        [StringLength(7)]
        public string ColorCode { get; set; } = "#3B82F6"; // Default blue color

        public AccountStatus Status { get; set; } = AccountStatus.Active;

        [StringLength(50)]
        public string? AccountNumber { get; set; }

        [StringLength(100)]
        public string? BankName { get; set; }

        // Credit/Loan specific properties
        [Column(TypeName = "decimal(18,2)")]
        public decimal? CreditLimit { get; set; }

        [Column(TypeName = "decimal(5,4)")]
        public decimal? InterestRate { get; set; }

        public DateTime? StatementDate { get; set; }

        public DateTime? DueDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? MinimumPayment { get; set; }

        // Investment specific properties
        [StringLength(100)]
        public string? BrokerName { get; set; }

        [StringLength(100)]
        public string? AccountHolder { get; set; }

        // Reconciliation properties
        public DateTime? LastReconciledDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? ReconciledBalance { get; set; }

        // Navigation properties
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public virtual ICollection<ScheduledTransaction> ScheduledTransactions { get; set; } = new List<ScheduledTransaction>();

        // Computed properties
        [NotMapped]
        public decimal AvailableBalance => AccountType == AccountType.Credit 
            ? (CreditLimit ?? 0) - CurrentBalance 
            : CurrentBalance;

        [NotMapped]
        public decimal UtilizationPercentage => AccountType == AccountType.Credit && CreditLimit > 0 
            ? (CurrentBalance / CreditLimit.Value) * 100 
            : 0;

        [NotMapped]
        public bool IsOverdue => AccountType == AccountType.Credit && DueDate.HasValue && DateTime.UtcNow > DueDate.Value;

        [NotMapped]
        public bool NeedsReconciliation => LastReconciledDate == null || 
            (DateTime.UtcNow - LastReconciledDate.Value).TotalDays > 30;
        
        [NotMapped]
        public decimal Balance 
        { 
            get => CurrentBalance; 
            set => CurrentBalance = value; 
        }

        [NotMapped]
        public string DisplayName => !string.IsNullOrEmpty(BankName) ? $"{Name} ({BankName})" : Name;

        [NotMapped]
        public string FormattedBalance => $"{CurrentBalance:C} {Currency}";

        [NotMapped]
        public string AccountTypeDisplay => AccountType switch
        {
            AccountType.Checking => "Checking Account",
            AccountType.Savings => "Savings Account",
            AccountType.Credit => "Credit Card",
            AccountType.Investment => "Investment Account",
            AccountType.Cash => "Cash",
            AccountType.Loan => "Loan Account",
            _ => "Other"
        };

        public void UpdateBalance(decimal amount)
        {
            CurrentBalance += amount;
        }

        public void UpdateBalance(decimal amount, TransactionType transactionType)
        {
            switch (AccountType)
            {
                case AccountType.Checking:
                case AccountType.Savings:
                case AccountType.Cash:
                case AccountType.Investment:
                    CurrentBalance += transactionType == TransactionType.Income ? amount : -amount;
                    break;
                case AccountType.Credit:
                case AccountType.Loan:
                    CurrentBalance += transactionType == TransactionType.Expense ? amount : -amount;
                    break;
            }
        }

        public bool CanPerformTransaction(decimal amount, TransactionType transactionType)
        {
            switch (AccountType)
            {
                case AccountType.Checking:
                case AccountType.Savings:
                case AccountType.Cash:
                    return transactionType == TransactionType.Income || CurrentBalance >= amount;
                case AccountType.Credit:
                    return transactionType == TransactionType.Expense 
                        ? (CreditLimit == null || CurrentBalance + amount <= CreditLimit) 
                        : true;
                case AccountType.Investment:
                case AccountType.Loan:
                    return true; // Allow all transactions for these types
                default:
                    return false;
            }
        }

        public void Reconcile(decimal bankBalance, DateTime reconciliationDate)
        {
            ReconciledBalance = bankBalance;
            LastReconciledDate = reconciliationDate;
        }

        public decimal GetReconciliationDifference()
        {
            return ReconciledBalance.HasValue ? CurrentBalance - ReconciledBalance.Value : 0;
        }

        public void SetBalance(decimal newBalance)
        {
            CurrentBalance = newBalance;
        }
    }
}