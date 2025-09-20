using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pilgrims.PersonalFinances.Models
{
    /// <summary>
    /// Represents a payment made toward a debt
    /// </summary>
    public class DebtPayment : BaseEntity
    {
        /// <summary>
        /// Foreign key to the debt
        /// </summary>
        [Required]
        public int DebtId { get; set; }

        /// <summary>
        /// Navigation property to the debt
        /// </summary>
        public virtual Debt Debt { get; set; } = null!;

        /// <summary>
        /// Payment amount
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Date when the payment was made
        /// </summary>
        [Required]
        public DateTime PaymentDate { get; set; }

        /// <summary>
        /// Amount applied to principal
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? PrincipalAmount { get; set; }

        /// <summary>
        /// Amount applied to interest
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? InterestAmount { get; set; }

        /// <summary>
        /// Amount applied to fees
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? FeesAmount { get; set; }

        /// <summary>
        /// Payment method or description
        /// </summary>
        [StringLength(100)]
        public string? PaymentMethod { get; set; }

        /// <summary>
        /// Reference number or confirmation number
        /// </summary>
        [StringLength(50)]
        public string? ReferenceNumber { get; set; }

        /// <summary>
        /// Additional notes about the payment
        /// </summary>
        [StringLength(500)]
        public string? Notes { get; set; }

        /// <summary>
        /// Whether this payment was scheduled or manual
        /// </summary>
        public bool IsScheduled { get; set; } = false;

        /// <summary>
        /// Foreign key to scheduled transaction (if applicable)
        /// </summary>
        public int? ScheduledTransactionId { get; set; }

        /// <summary>
        /// Navigation property to the scheduled transaction
        /// </summary>
        public virtual ScheduledTransaction? ScheduledTransaction { get; set; }
    }
}