using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pilgrims.PersonalFinances.Models.Enums;

namespace Pilgrims.PersonalFinances.Models
{
    /// <summary>
    /// Represents a debt or loan obligation
    /// </summary>
    public class Debt
    {
        /// <summary>
        /// Unique identifier for the debt
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name or description of the debt
        /// </summary>
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Type of debt (credit card, loan, etc.)
        /// </summary>
        [Required]
        public DebtType DebtType { get; set; }

        /// <summary>
        /// Foreign key to the creditor
        /// </summary>
        public int CreditorId { get; set; }

        /// <summary>
        /// Navigation property to the creditor
        /// </summary>
        public virtual Creditor Creditor { get; set; } = null!;

        /// <summary>
        /// Original principal amount of the debt
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrincipalAmount { get; set; }

        /// <summary>
        /// Current outstanding balance
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CurrentBalance { get; set; }

        /// <summary>
        /// Annual interest rate (as percentage, e.g., 5.5 for 5.5%)
        /// </summary>
        [Column(TypeName = "decimal(5,2)")]
        public decimal? InterestRate { get; set; }

        /// <summary>
        /// Minimum monthly payment required
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? MinimumPayment { get; set; }

        /// <summary>
        /// Payment due date (day of month, e.g., 15 for 15th of each month)
        /// </summary>
        [Range(1, 31)]
        public int? PaymentDueDay { get; set; }

        /// <summary>
        /// Frequency of payments
        /// </summary>
        public PaymentFrequency PaymentFrequency { get; set; } = PaymentFrequency.Monthly;

        /// <summary>
        /// Priority level for debt management
        /// </summary>
        public DebtPriority Priority { get; set; } = DebtPriority.Medium;

        /// <summary>
        /// Date when the debt was originated
        /// </summary>
        public DateTime? OriginationDate { get; set; }

        /// <summary>
        /// Maturity or payoff date (if applicable)
        /// </summary>
        public DateTime? MaturityDate { get; set; }

        /// <summary>
        /// Account number for this debt
        /// </summary>
        [StringLength(50)]
        public string? AccountNumber { get; set; }

        /// <summary>
        /// Additional notes about the debt
        /// </summary>
        [StringLength(1000)]
        public string? Notes { get; set; }

        /// <summary>
        /// Whether the debt is currently active
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Date when the debt was paid off (if applicable)
        /// </summary>
        public DateTime? PaidOffDate { get; set; }

        /// <summary>
        /// Date when the debt record was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Date when the debt record was last updated
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Collection of payments made toward this debt
        /// </summary>
        public virtual ICollection<DebtPayment> Payments { get; set; } = new List<DebtPayment>();

        /// <summary>
        /// Data Linking: Related transactions for this debt
        /// </summary>
        public virtual ICollection<Transaction> RelatedTransactions { get; set; } = new List<Transaction>();

        /// <summary>
        /// Data Linking: Budget items related to this debt
        /// </summary>
        public virtual ICollection<Budget> RelatedBudgets { get; set; } = new List<Budget>();

        /// <summary>
        /// Calculated property: Total amount paid toward this debt
        /// </summary>
        [NotMapped]
        public decimal TotalPaid => Payments?.Sum(p => p.Amount) ?? 0;

        /// <summary>
        /// Calculated property: Remaining balance percentage
        /// </summary>
        [NotMapped]
        public decimal BalancePercentage => PrincipalAmount > 0 ? (CurrentBalance / PrincipalAmount) * 100 : 0;

        /// <summary>
        /// Calculated property: Next payment due date
        /// </summary>
        [NotMapped]
        public DateTime? NextPaymentDue
        {
            get
            {
                if (!PaymentDueDay.HasValue || !IsActive)
                    return null;

                var today = DateTime.Today;
                var thisMonth = new DateTime(today.Year, today.Month, Math.Min(PaymentDueDay.Value, DateTime.DaysInMonth(today.Year, today.Month)));
                
                return thisMonth >= today ? thisMonth : thisMonth.AddMonths(1);
            }
        }
    }
}