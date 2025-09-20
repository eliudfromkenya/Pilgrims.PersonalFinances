using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pilgrims.PersonalFinances.Models
{
    public class Asset : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        // Category relationship
        public int? AssetCategoryId { get; set; }
        public virtual AssetCategory? AssetCategory { get; set; }

        [Required]
        public DateTime PurchaseDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PurchasePrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CurrentValue { get; set; }

        [MaxLength(50)]
        public string DepreciationMethod { get; set; } = "Straight-line"; // Straight-line, Declining balance, Custom

        [Column(TypeName = "decimal(5,2)")]
        public decimal? DepreciationRate { get; set; } // Annual depreciation rate as percentage

        public int? UsefulLifeYears { get; set; } // For straight-line depreciation

        [Column(TypeName = "decimal(18,2)")]
        public decimal? SalvageValue { get; set; } // Estimated value at end of useful life

        // Linking to purchase transaction
        public int? PurchaseTransactionId { get; set; }
        public virtual Transaction? PurchaseTransaction { get; set; }

        // Asset status
        public bool IsActive { get; set; } = true;
        public DateTime? DisposalDate { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DisposalValue { get; set; }

        // Document attachments (stored as file paths or references)
        [MaxLength(1000)]
        public string? ReceiptPath { get; set; }
        [MaxLength(1000)]
        public string? WarrantyPath { get; set; }
        [MaxLength(1000)]
        public string? InsuranceDocumentPath { get; set; }

        // Location and condition
        [MaxLength(200)]
        public string? Location { get; set; }
        [MaxLength(100)]
        public string? Condition { get; set; } // Excellent, Good, Fair, Poor

        // Serial number or identifier
        [MaxLength(100)]
        public string? SerialNumber { get; set; }

        // Audit fields
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        // Data Linking Relationships
        public int? AssetRegisterId { get; set; }
        public virtual AssetRegister? AssetRegister { get; set; }

        // Navigation properties
        public virtual ICollection<AssetMaintenance> MaintenanceRecords { get; set; } = new List<AssetMaintenance>();
        public virtual ICollection<AssetInsurance> InsurancePolicies { get; set; } = new List<AssetInsurance>();
        public virtual ICollection<Transaction> RelatedTransactions { get; set; } = new List<Transaction>();

        // Calculated properties
        [NotMapped]
        public decimal CalculatedCurrentValue
        {
            get
            {
                if (CurrentValue > 0)
                    return CurrentValue;

                return CalculateDepreciatedValue();
            }
        }

        [NotMapped]
        public decimal TotalDepreciation => PurchasePrice - CalculatedCurrentValue;

        [NotMapped]
        public decimal DepreciationPercentage => PurchasePrice > 0 ? (TotalDepreciation / PurchasePrice) * 100 : 0;

        [NotMapped]
        public int AgeInYears => (int)((DateTime.Now - PurchaseDate).TotalDays / 365.25);

        [NotMapped]
        public decimal AnnualDepreciationAmount
        {
            get
            {
                return DepreciationMethod switch
                {
                    "Straight-line" => UsefulLifeYears > 0 ? (PurchasePrice - (SalvageValue ?? 0)) / UsefulLifeYears.Value : 0,
                    "Declining balance" => DepreciationRate.HasValue ? PurchasePrice * (DepreciationRate.Value / 100) : 0,
                    _ => 0
                };
            }
        }

        // Methods
        public decimal CalculateDepreciatedValue()
        {
            if (!IsActive || DisposalDate.HasValue)
                return DisposalValue ?? 0;

            var yearsOwned = (decimal)AgeInYears;
            
            return DepreciationMethod switch
            {
                "Straight-line" => CalculateStraightLineDepreciation(yearsOwned),
                "Declining balance" => CalculateDecliningBalanceDepreciation(yearsOwned),
                "Custom" => CurrentValue, // Use manually set current value
                _ => PurchasePrice // No depreciation
            };
        }

        private decimal CalculateStraightLineDepreciation(decimal yearsOwned)
        {
            if (!UsefulLifeYears.HasValue || UsefulLifeYears <= 0)
                return PurchasePrice;

            var salvage = SalvageValue ?? 0;
            var annualDepreciation = (PurchasePrice - salvage) / UsefulLifeYears.Value;
            var totalDepreciation = Math.Min(annualDepreciation * yearsOwned, PurchasePrice - salvage);
            
            return Math.Max(PurchasePrice - totalDepreciation, salvage);
        }

        private decimal CalculateDecliningBalanceDepreciation(decimal yearsOwned)
        {
            if (!DepreciationRate.HasValue || DepreciationRate <= 0)
                return PurchasePrice;

            var rate = DepreciationRate.Value / 100;
            var remainingValue = PurchasePrice * (decimal)Math.Pow((double)(1 - rate), (double)yearsOwned);
            var salvage = SalvageValue ?? 0;
            
            return Math.Max(remainingValue, salvage);
        }

        public void UpdateCurrentValue()
        {
            if (DepreciationMethod != "Custom")
            {
                CurrentValue = CalculateDepreciatedValue();
                UpdatedAt = DateTime.Now;
            }
        }
    }
}