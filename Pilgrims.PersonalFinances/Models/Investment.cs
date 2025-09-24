using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pilgrims.PersonalFinances.Models.Enums;

namespace Pilgrims.PersonalFinances.Models
{
    public class Investment: BaseEntity
    {
        [Required]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Symbol { get; set; }

        [Required]
        public InvestmentType InvestmentType { get; set; }

        [Required]
        public string AccountId { get; set; } = string.Empty;

        [ForeignKey("AccountId")]
        public virtual Account Account { get; set; } = null!;

        [Column(TypeName = "decimal(18,6)")]
        public decimal Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PurchasePrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? CurrentPrice { get; set; }

        public DateTime PurchaseDate { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        // Calculated Properties
        [NotMapped]
        public decimal TotalValue => Quantity * (CurrentPrice ?? PurchasePrice);

        [NotMapped]
        public decimal TotalCost => Quantity * PurchasePrice;

        [NotMapped]
        public decimal GainLoss => TotalValue - TotalCost;

        [NotMapped]
        public decimal GainLossPercentage => TotalCost != 0 ? (GainLoss / TotalCost) * 100 : 0;

        // Audit Fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        // Audit fields - CreatedAt and UpdatedAt are inherited from BaseEntity
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        // Navigation Properties
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}