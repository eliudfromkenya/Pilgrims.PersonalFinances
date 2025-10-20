using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pilgrims.PersonalFinances.Core.Models.Enums;

namespace Pilgrims.PersonalFinances.Core.Models
{
    public class ReconciliationSession: BaseEntity
    {
        [Required]
        public string AccountId { get; set; } = string.Empty;

        [ForeignKey("AccountId")]
        public virtual Account Account { get; set; }

        [Required]
        [StringLength(255)]
        public string SessionName { get; set; }

        [Required]
        public DateTime ReconciliationDate { get; set; }

        [Required]
        public DateTime StatementStartDate { get; set; }

        [Required]
        public DateTime StatementEndDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal StatementStartingBalance { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal StatementEndingBalance { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal BookStartingBalance { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal BookEndingBalance { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Difference { get; set; }

        public ReconciliationStatus Status { get; set; } = ReconciliationStatus.InProgress;

        [StringLength(500)]
        public string? BankStatementFilePath { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        public bool IsReconciled { get; set; } = false;

        public DateTime? ReconciledDate { get; set; }

        public string? ReconciledBy { get; set; }

        // Navigation Properties
        public virtual ICollection<ReconciliationItem> ReconciliationItems { get; set; } = new List<ReconciliationItem>();

        // Calculated Properties
        [NotMapped]
        public decimal TotalClearedAmount => ReconciliationItems
            .Where(item => item.IsCleared)
            .Sum(item => item.Amount);

        [NotMapped]
        public decimal TotalUnclearedAmount => ReconciliationItems
            .Where(item => !item.IsCleared)
            .Sum(item => item.Amount);

        [NotMapped]
        public int ClearedItemsCount => ReconciliationItems.Count(item => item.IsCleared);

        [NotMapped]
        public int UnclearedItemsCount => ReconciliationItems.Count(item => !item.IsCleared);

        [NotMapped]
        public decimal CalculatedBookBalance => BookStartingBalance + TotalClearedAmount;

        [NotMapped]
        public bool IsBalanced => Math.Abs(Difference) < 0.01m;

        [NotMapped]
        public string StatusDisplayName => Status switch
        {
            ReconciliationStatus.InProgress => "In Progress",
            ReconciliationStatus.Completed => "Completed",
            ReconciliationStatus.Cancelled => "Cancelled",
            ReconciliationStatus.RequiresReview => "Requires Review",
            _ => "Unknown"
        };

        [NotMapped]
        public string StatusCssClass => Status switch
        {
            ReconciliationStatus.InProgress => "bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-300",
            ReconciliationStatus.Completed => "bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-300",
            ReconciliationStatus.Cancelled => "bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-300",
            ReconciliationStatus.RequiresReview => "bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-300",
            _ => "bg-gray-100 text-gray-800 dark:bg-gray-900/30 dark:text-gray-300"
        };

        // Methods
        public void CalculateDifference()
        {
            BookEndingBalance = CalculatedBookBalance;
            Difference = StatementEndingBalance - BookEndingBalance;
        }

        public void MarkAsCompleted(string? reconciledBy = null)
        {
            Status = ReconciliationStatus.Completed;
            IsReconciled = true;
            ReconciledDate = DateTime.UtcNow;
            ReconciledBy = reconciledBy;
            CalculateDifference();
        }

        public void MarkAsRequiresReview(string? notes = null)
        {
            Status = ReconciliationStatus.RequiresReview;
            if (!string.IsNullOrEmpty(notes))
            {
                Notes = string.IsNullOrEmpty(Notes) ? notes : $"{Notes}\n{notes}";
            }
        }

        public bool CanBeCompleted()
        {
            return Status == ReconciliationStatus.InProgress && 
                   (IsBalanced || Math.Abs(Difference) < 100m); // Allow small differences
        }
    }
}
