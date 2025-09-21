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

        // Audit Fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}