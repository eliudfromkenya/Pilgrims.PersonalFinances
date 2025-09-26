using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pilgrims.PersonalFinances.Models
{
    public class Asset : BaseEntity
    {
        [Required(ErrorMessage = "Asset name is required")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Asset name must be between 2 and 200 characters")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        // Category relationship
        [Required(ErrorMessage = "Asset category is required")]
        public string AssetCategoryId { get; set; } = string.Empty;
        public virtual AssetCategory? AssetCategory { get; set; }

        // Alias properties for backward compatibility
        public string CategoryId => AssetCategoryId;
        public AssetCategory? Category => AssetCategory;

        [Required(ErrorMessage = "Purchase date is required")]
        [DataType(DataType.Date)]
        public DateTime? PurchaseDate { get; set; }

        [Required(ErrorMessage = "Purchase price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Purchase price must be greater than 0")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? PurchasePrice { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Current value cannot be negative")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? CurrentValue { get; set; }

        [Required(ErrorMessage = "Depreciation method is required")]
        [MaxLength(50)]
        public string? DepreciationMethod { get; set; } = "Straight-line"; // Straight-line, Declining balance, Custom

        [Range(0, 100, ErrorMessage = "Depreciation rate must be between 0 and 100")]
        [Column(TypeName = "decimal(5,2)")]
        public decimal? DepreciationRate { get; set; } // Annual depreciation rate as percentage

        [Range(1, 100, ErrorMessage = "Useful life must be between 1 and 100 years")]
        public int? UsefulLifeYears { get; set; } // For straight-line depreciation

        [Range(0, double.MaxValue, ErrorMessage = "Salvage value cannot be negative")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? SalvageValue { get; set; } // Estimated value at end of useful life

        // Additional asset properties
        [MaxLength(100, ErrorMessage = "Brand cannot exceed 100 characters")]
        public string? Brand { get; set; }

        [MaxLength(100, ErrorMessage = "Model cannot exceed 100 characters")]
        public string? Model { get; set; }

        // Linking to purchase transaction
        public string? PurchaseTransactionId { get; set; }
        public virtual Transaction? PurchaseTransaction { get; set; }

        // Asset status
        public bool IsActive { get; set; } = true;
        
        [DataType(DataType.Date)]
        public DateTime? DisposalDate { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Disposal value cannot be negative")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DisposalValue { get; set; }

        // Document attachments (stored as file paths or references)
        [MaxLength(1000, ErrorMessage = "Receipt path cannot exceed 1000 characters")]
        public string? ReceiptPath { get; set; }
        
        [MaxLength(1000, ErrorMessage = "Warranty path cannot exceed 1000 characters")]
        public string? WarrantyPath { get; set; }
        
        [MaxLength(1000, ErrorMessage = "Insurance document path cannot exceed 1000 characters")]
        public string? InsuranceDocumentPath { get; set; }

        // Location and condition
        [MaxLength(200, ErrorMessage = "Location cannot exceed 200 characters")]
        public string? Location { get; set; }
        
        [MaxLength(100, ErrorMessage = "Condition cannot exceed 100 characters")]
        public string? Condition { get; set; } // Excellent, Good, Fair, Poor

        // Serial number or identifier
        [MaxLength(100, ErrorMessage = "Serial number cannot exceed 100 characters")]
        public string? SerialNumber { get; set; }

        // Audit fields - CreatedAt and UpdatedAt are inherited from BaseEntity

        // Data Linking Relationships
        public string? AssetRegisterId { get; set; }
        public virtual AssetRegister? AssetRegister { get; set; }

        // Navigation properties
        public virtual ICollection<AssetMaintenance> MaintenanceRecords { get; set; } = new List<AssetMaintenance>();
        public virtual ICollection<AssetInsurance> InsurancePolicies { get; set; } = new List<AssetInsurance>();
        public virtual ICollection<Transaction> RelatedTransactions { get; set; } = new List<Transaction>();
        public virtual ICollection<AssetDocument> Documents { get; set; } = new List<AssetDocument>();

        // Calculated properties
        [NotMapped]
        public decimal CalculatedCurrentValue
        {
            get
            {
                if (CurrentValue.HasValue && CurrentValue > 0)
                    return CurrentValue.Value;

                return CalculateDepreciatedValue();
            }
        }

        [NotMapped]
        public decimal TotalDepreciation => (PurchasePrice ?? 0) - CalculatedCurrentValue;

        [NotMapped]
        public decimal DepreciationPercentage => (PurchasePrice ?? 0) > 0 ? (TotalDepreciation / (PurchasePrice ?? 1)) * 100 : 0;

        [NotMapped]
        public int AgeInYears => PurchaseDate.HasValue ? (int)((DateTime.Now - PurchaseDate.Value).TotalDays / 365.25) : 0;

        [NotMapped]
        public int AgeInMonths => PurchaseDate.HasValue ? (int)((DateTime.Now - PurchaseDate.Value).TotalDays / 30.44) : 0;

        [NotMapped]
        public decimal AnnualDepreciationAmount
        {
            get
            {
                return DepreciationMethod switch
                {
                    "Straight-line" => UsefulLifeYears > 0 ? ((PurchasePrice ?? 0) - (SalvageValue ?? 0)) / UsefulLifeYears.Value : 0,
                    "Declining balance" => DepreciationRate.HasValue ? (PurchasePrice ?? 0) * (DepreciationRate.Value / 100) : 0,
                    _ => 0
                };
            }
        }

        [NotMapped]
        public decimal MonthlyDepreciationAmount => AnnualDepreciationAmount / 12;

        [NotMapped]
        public string FormattedPurchasePrice => (PurchasePrice ?? 0).ToString("C2");

        [NotMapped]
        public string FormattedCurrentValue => CalculatedCurrentValue.ToString("C2");

        [NotMapped]
        public string FormattedTotalDepreciation => TotalDepreciation.ToString("C2");

        [NotMapped]
        public bool IsFullyDepreciated => CalculatedCurrentValue <= (SalvageValue ?? 0);

        [NotMapped]
        public bool HasInsurance => InsurancePolicies?.Any(i => i.IsActive && i.PolicyEndDate > DateTime.Now) ?? false;

        [NotMapped]
        public bool HasWarranty => !string.IsNullOrEmpty(WarrantyPath);

        [NotMapped]
        public bool RequiresMaintenance => MaintenanceRecords?.Any() == true && 
            MaintenanceRecords.OrderByDescending(m => m.ServiceDate).FirstOrDefault()?.ServiceDate < DateTime.Now.AddMonths(-6);

        [NotMapped]
        public decimal MaintenanceCostTotal => MaintenanceRecords?.Sum(m => m.Cost) ?? 0;

        [NotMapped]
        public decimal TotalCostOfOwnership => (PurchasePrice ?? 0m) + MaintenanceCostTotal;

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
                "Custom" => CurrentValue ?? 0, // Use manually set current value
                _ => PurchasePrice ?? 0 // No depreciation
            };
        }

        private decimal CalculateStraightLineDepreciation(decimal yearsOwned)
        {
            if (!UsefulLifeYears.HasValue || UsefulLifeYears <= 0)
                return PurchasePrice ?? 0;

            var salvage = SalvageValue ?? 0;
            var annualDepreciation = ((PurchasePrice ?? 0) - salvage) / UsefulLifeYears.Value;
            var totalDepreciation = Math.Min(annualDepreciation * yearsOwned, (PurchasePrice ?? 0) - salvage);
            return Math.Max((PurchasePrice ?? 0) - totalDepreciation, salvage);
        }

        private decimal CalculateDecliningBalanceDepreciation(decimal yearsOwned)
        {
            if (!DepreciationRate.HasValue || DepreciationRate <= 0)
                return PurchasePrice ?? 0;

            var rate = DepreciationRate.Value / 100;
            var remainingValue = (PurchasePrice ?? 0) * (decimal)Math.Pow((double)(1 - rate), (double)yearsOwned);
            var salvage = SalvageValue ?? 0;
            
            return Math.Max(remainingValue, salvage);
        }

        public void UpdateCurrentValue()
        {
            if (DepreciationMethod != "Custom")
            {
                CurrentValue = CalculateDepreciatedValue();
                TouchUpdatedAt();
            }
        }

        /// <summary>
        /// Validates the asset data
        /// </summary>
        /// <returns>List of validation errors</returns>
        public List<string> Validate()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(Name))
                errors.Add("Asset name is required");

            if (PurchasePrice <= 0)
                errors.Add("Purchase price must be greater than 0");

            if (PurchaseDate > DateTime.Now)
                errors.Add("Purchase date cannot be in the future");

            if (DisposalDate.HasValue && DisposalDate.Value < PurchaseDate)
                errors.Add("Disposal date cannot be before purchase date");

            if (SalvageValue.HasValue && SalvageValue.Value > PurchasePrice)
                errors.Add("Salvage value cannot be greater than purchase price");

            if (DepreciationMethod == "Straight-line" && (!UsefulLifeYears.HasValue || UsefulLifeYears <= 0))
                errors.Add("Useful life years is required for straight-line depreciation");

            if (DepreciationMethod == "Declining balance" && (!DepreciationRate.HasValue || DepreciationRate <= 0))
                errors.Add("Depreciation rate is required for declining balance depreciation");

            if (DepreciationRate.HasValue && (DepreciationRate < 0 || DepreciationRate > 100))
                errors.Add("Depreciation rate must be between 0 and 100");

            return errors;
        }

        /// <summary>
        /// Determines if the asset can be disposed
        /// </summary>
        public bool CanDispose()
        {
            return IsActive && !DisposalDate.HasValue;
        }

        /// <summary>
        /// Disposes the asset
        /// </summary>
        public void Dispose(DateTime disposalDate, decimal? disposalValue = null)
        {
            if (!CanDispose())
                throw new InvalidOperationException("Asset cannot be disposed");

            IsActive = false;
            DisposalDate = disposalDate;
            DisposalValue = disposalValue;
            TouchUpdatedAt();
        }

        /// <summary>
        /// Gets the depreciation schedule for the asset
        /// </summary>
        public List<DepreciationScheduleItem> GetDepreciationSchedule()
        {
            var schedule = new List<DepreciationScheduleItem>();
            
            if (!IsActive || DepreciationMethod == "Custom")
                return schedule;

            var years = UsefulLifeYears ?? 10;
            var currentValue = PurchasePrice ?? 0;
            var salvage = SalvageValue ?? 0;

            for (int year = 1; year <= years; year++)
            {
                var yearDate = (PurchaseDate ?? DateTime.Now).AddYears(year);
                decimal depreciationAmount = 0;
                decimal endingValue = 0;

                switch (DepreciationMethod)
                {
                    case "Straight-line":
                        depreciationAmount = ((PurchasePrice ?? 0) - salvage) / years;
                        endingValue = Math.Max(currentValue - depreciationAmount, salvage);
                        break;
                    case "Declining balance":
                        if (DepreciationRate.HasValue)
                        {
                            depreciationAmount = currentValue * (DepreciationRate.Value / 100);
                            endingValue = Math.Max(currentValue - depreciationAmount, salvage);
                        }
                        break;
                }

                schedule.Add(new DepreciationScheduleItem
                {
                    Year = year,
                    Date = yearDate,
                    BeginningValue = currentValue,
                    DepreciationAmount = depreciationAmount,
                    EndingValue = endingValue
                });

                currentValue = endingValue;
                
                if (currentValue <= salvage)
                    break;
            }

            return schedule;
        }
    }

    public class DepreciationScheduleItem
    {
        public int Year { get; set; }
        public DateTime Date { get; set; }
        public decimal BeginningValue { get; set; }
        public decimal DepreciationAmount { get; set; }
        public decimal EndingValue { get; set; }
    }
}