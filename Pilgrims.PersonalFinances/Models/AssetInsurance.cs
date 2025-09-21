using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pilgrims.PersonalFinances.Models
{
    public class AssetInsurance: BaseEntity
    {
        // Asset relationship
        [Required]
        public string AssetId { get; set; } = string.Empty;
        public virtual Asset Asset { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string PolicyNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string InsuranceProvider { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? AgentName { get; set; }

        [MaxLength(100)]
        public string? AgentContact { get; set; }

        // Coverage details
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CoverageAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Deductible { get; set; }

        [MaxLength(100)]
        public string CoverageType { get; set; } = "Comprehensive"; // Comprehensive, Liability, Collision, etc.

        [MaxLength(1000)]
        public string? CoverageDetails { get; set; }

        // Policy dates
        [Required]
        public DateTime PolicyStartDate { get; set; }

        [Required]
        public DateTime PolicyEndDate { get; set; }

        public DateTime? RenewalDate { get; set; }

        // Premium information
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal AnnualPremium { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? MonthlyPremium { get; set; }

        [MaxLength(20)]
        public string PaymentFrequency { get; set; } = "Annual"; // Annual, Semi-annual, Quarterly, Monthly

        // Status
        public bool IsActive { get; set; } = true;
        public bool AutoRenewal { get; set; } = false;

        [MaxLength(20)]
        public string Status { get; set; } = "Active"; // Active, Expired, Cancelled, Pending

        // Beneficiary information
        [MaxLength(200)]
        public string? PrimaryBeneficiary { get; set; }

        [MaxLength(200)]
        public string? SecondaryBeneficiary { get; set; }

        // Claims history
        public int ClaimsCount { get; set; } = 0;
        [Column(TypeName = "decimal(18,2)")]
        public decimal? TotalClaimsAmount { get; set; }

        public DateTime? LastClaimDate { get; set; }

        // Document attachments
        [MaxLength(1000)]
        public string? PolicyDocumentPath { get; set; }

        [MaxLength(1000)]
        public string? CertificatePath { get; set; }

        // Notes and additional information
        [MaxLength(1000)]
        public string? Notes { get; set; }

        // Link to scheduled transactions for premium payments
        public string? ScheduledTransactionId { get; set; }
        public virtual ScheduledTransaction? ScheduledTransaction { get; set; }

        // Audit fields
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        // Calculated properties
        [NotMapped]
        public bool IsExpiringSoon => PolicyEndDate <= DateTime.Now.AddDays(30);

        [NotMapped]
        public bool IsExpired => PolicyEndDate < DateTime.Now;

        [NotMapped]
        public int DaysUntilExpiry => (PolicyEndDate - DateTime.Now).Days;

        [NotMapped]
        public decimal MonthlyPremiumCalculated => PaymentFrequency switch
        {
            "Monthly" => MonthlyPremium ?? (AnnualPremium / 12),
            "Quarterly" => AnnualPremium / 4 / 3,
            "Semi-annual" => AnnualPremium / 2 / 6,
            _ => AnnualPremium / 12
        };

        [NotMapped]
        public string PolicySummary => $"{InsuranceProvider} - {PolicyNumber} - ${CoverageAmount:N0} coverage";

        [NotMapped]
        public string ExpiryStatus
        {
            get
            {
                if (IsExpired) return "Expired";
                if (IsExpiringSoon) return "Expiring Soon";
                return "Active";
            }
        }

        // Methods
        public void AddClaim(decimal claimAmount)
        {
            ClaimsCount++;
            TotalClaimsAmount = (TotalClaimsAmount ?? 0) + claimAmount;
            LastClaimDate = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }

        public void Renew(DateTime newEndDate, decimal? newPremium = null)
        {
            PolicyStartDate = PolicyEndDate;
            PolicyEndDate = newEndDate;
            RenewalDate = DateTime.Now;
            
            if (newPremium.HasValue)
                AnnualPremium = newPremium.Value;
            
            Status = "Active";
            IsActive = true;
            UpdatedAt = DateTime.Now;
        }
    }
}