using System.ComponentModel.DataAnnotations;

namespace Pilgrims.PersonalFinances.Models
{
    /// <summary>
    /// Represents a creditor or lender
    /// </summary>
    public class Creditor: BaseEntity
    {
        /// <summary>
        /// Name of the creditor
        /// </summary>
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Contact phone number
        /// </summary>
        [StringLength(20)]
        public string? Phone { get; set; }

        /// <summary>
        /// Contact email address
        /// </summary>
        [StringLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        /// <summary>
        /// Mailing address
        /// </summary>
        [StringLength(500)]
        public string? Address { get; set; }

        /// <summary>
        /// Website URL
        /// </summary>
        [StringLength(200)]
        [Url]
        public string? Website { get; set; }

        /// <summary>
        /// Customer service phone number
        /// </summary>
        [StringLength(20)]
        public string? CustomerServicePhone { get; set; }

        /// <summary>
        /// Account number with this creditor
        /// </summary>
        [StringLength(50)]
        public string? AccountNumber { get; set; }

        /// <summary>
        /// Additional notes about the creditor
        /// </summary>
        [StringLength(1000)]
        public string? Notes { get; set; }

        /// <summary>
        /// Date when the creditor record was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Date when the creditor record was last updated
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Collection of debts associated with this creditor
        /// </summary>
        public virtual ICollection<Debt> Debts { get; set; } = new List<Debt>();
    }
}