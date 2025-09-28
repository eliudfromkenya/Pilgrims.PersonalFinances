using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pilgrims.PersonalFinances.Models.Enums;

namespace Pilgrims.PersonalFinances.Models
{
    public class ReconciliationItem: BaseEntity
    {
        [Required]
        public string? ReconciliationSessionId { get; set; }

        [ForeignKey("ReconciliationSessionId")]
        public virtual ReconciliationSession ReconciliationSession { get; set; } = null!;

        public string? TransactionId { get; set; }

        [ForeignKey("TransactionId")]
        public virtual Transaction? Transaction { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [StringLength(100)]
        public string? Reference { get; set; }

        public ReconciliationItemType ItemType { get; set; }

        public ReconciliationItemStatus Status { get; set; } = ReconciliationItemStatus.Unmatched;

        [StringLength(1000)]
        public string? Notes { get; set; }

        public bool IsCleared { get; set; } = false;

        public DateTime? ClearedDate { get; set; }

        // For bank statement items that don't match transactions
        public bool IsStatementOnly { get; set; } = false;

        // For book transactions that don't appear on statement
        public bool IsBookOnly { get; set; } = false;

        // Audit fields - CreatedAt and UpdatedAt are inherited from BaseEntity
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        // Calculated Properties
        [NotMapped]
        public string StatusDisplayName => Status switch
        {
            ReconciliationItemStatus.Unmatched => "Unmatched",
            ReconciliationItemStatus.Matched => "Matched",
            ReconciliationItemStatus.Disputed => "Disputed",
            ReconciliationItemStatus.Resolved => "Resolved",
            ReconciliationItemStatus.Ignored => "Ignored",
            _ => "Unknown"
        };

        [NotMapped]
        public string StatusCssClass => Status switch
        {
            ReconciliationItemStatus.Unmatched => "bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-300",
            ReconciliationItemStatus.Matched => "bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-300",
            ReconciliationItemStatus.Disputed => "bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-300",
            ReconciliationItemStatus.Resolved => "bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-300",
            ReconciliationItemStatus.Ignored => "bg-gray-100 text-gray-800 dark:bg-gray-900/30 dark:text-gray-300",
            _ => "bg-gray-100 text-gray-800 dark:bg-gray-900/30 dark:text-gray-300"
        };

        [NotMapped]
        public string ItemTypeDisplayName => ItemType switch
        {
            ReconciliationItemType.Deposit => "Deposit",
            ReconciliationItemType.Withdrawal => "Withdrawal",
            ReconciliationItemType.Fee => "Fee",
            ReconciliationItemType.Interest => "Interest",
            ReconciliationItemType.Transfer => "Transfer",
            ReconciliationItemType.Adjustment => "Adjustment",
            ReconciliationItemType.Other => "Other",
            _ => "Unknown"
        };

        [NotMapped]
        public string ItemTypeCssClass => ItemType switch
        {
            ReconciliationItemType.Deposit => "bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-300",
            ReconciliationItemType.Withdrawal => "bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-300",
            ReconciliationItemType.Fee => "bg-orange-100 text-orange-800 dark:bg-orange-900/30 dark:text-orange-300",
            ReconciliationItemType.Interest => "bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-300",
            ReconciliationItemType.Transfer => "bg-purple-100 text-purple-800 dark:bg-purple-900/30 dark:text-purple-300",
            ReconciliationItemType.Adjustment => "bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-300",
            ReconciliationItemType.Other => "bg-gray-100 text-gray-800 dark:bg-gray-900/30 dark:text-gray-300",
            _ => "bg-gray-100 text-gray-800 dark:bg-gray-900/30 dark:text-gray-300"
        };

        [NotMapped]
        public bool IsDebit => Amount < 0 || ItemType == ReconciliationItemType.Withdrawal || ItemType == ReconciliationItemType.Fee;

        [NotMapped]
        public bool IsCredit => Amount > 0 || ItemType == ReconciliationItemType.Deposit || ItemType == ReconciliationItemType.Interest;

        [NotMapped]
        public string FormattedAmount => Amount.ToString("C");

        [NotMapped]
        public string AmountCssClass => IsDebit ? "text-red-600 dark:text-red-400" : "text-green-600 dark:text-green-400";

        // Methods
        public void MarkAsCleared(string? clearedBy = null)
        {
            IsCleared = true;
            ClearedDate = DateTime.UtcNow;
            Status = ReconciliationItemStatus.Matched;
            UpdatedBy = clearedBy;
        }

        public void MarkAsDisputed(string? notes = null)
        {
            Status = ReconciliationItemStatus.Disputed;
            if (!string.IsNullOrEmpty(notes))
            {
                Notes = string.IsNullOrEmpty(Notes) ? notes : $"{Notes}\n{notes}";
            }
        }

        public void MarkAsResolved(string? notes = null)
        {
            Status = ReconciliationItemStatus.Resolved;
            if (!string.IsNullOrEmpty(notes))
            {
                Notes = string.IsNullOrEmpty(Notes) ? notes : $"{Notes}\n{notes}";
            }
            TouchUpdatedAt();
        }

        public void MarkAsIgnored(string? reason = null)
        {
            Status = ReconciliationItemStatus.Ignored;
            if (!string.IsNullOrEmpty(reason))
            {
                Notes = string.IsNullOrEmpty(Notes) ? $"Ignored: {reason}" : $"{Notes}\nIgnored: {reason}";
            }
            TouchUpdatedAt();
        }

        public bool CanBeMatched()
        {
            return Status == ReconciliationItemStatus.Unmatched && !IsCleared;
        }

        public bool CanBeDisputed()
        {
            return Status != ReconciliationItemStatus.Ignored && Status != ReconciliationItemStatus.Resolved;
        }
    }
}