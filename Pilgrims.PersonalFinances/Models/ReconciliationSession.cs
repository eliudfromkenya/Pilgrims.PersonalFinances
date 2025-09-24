using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pilgrims.PersonalFinances.Models.Enums;

namespace Pilgrims.PersonalFinances.Models
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

        // Audit Fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        // Audit fields - CreatedAt and UpdatedAt are inherited from BaseEntity
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        // Navigation Properties
        public virtual ICollection<ReconciliationItem> ReconciliationItems { get; set; } = new List<ReconciliationItem>();
    }
}