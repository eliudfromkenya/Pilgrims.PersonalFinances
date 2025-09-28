using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pilgrims.PersonalFinances.Models.Enums;

namespace Pilgrims.PersonalFinances.Models
{
    public class Insurance : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string PolicyNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string InsuranceCompany { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string PolicyType { get; set; } = string.Empty; // Life, Health, Auto, Property, etc.

        [Required]
        [StringLength(200)]
        public string PolicyName { get; set; } = string.Empty;

        [StringLength(200)]
        public string? AgentName { get; set; } = string.Empty;
        [StringLength(200)]
        public string? AgentEmail { get; set; } = string.Empty;
        [StringLength(200)]
        public string? AgentPhone { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal? CoverageAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PremiumAmount { get; set; }

        public PaymentFrequency PremiumFrequency { get; set; }

        public DateTime? PolicyStartDate { get; set; }

        public DateTime? PolicyEndDate { get; set; }

        public DateTime? NextPremiumDueDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DeductibleAmount { get; set; }

        public InsuranceStatus Status { get; set; }

        [StringLength(100)]
        public string BeneficiaryName { get; set; } = string.Empty;

        [StringLength(200)]
        public string BeneficiaryRelationship { get; set; } = string.Empty;

        [StringLength(15)]
        public string BeneficiaryPhone { get; set; } = string.Empty;

        [StringLength(200)]
        public string BeneficiaryEmail { get; set; } = string.Empty;

        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;

        // Alias properties for backward compatibility
        [NotMapped]
        public DateTime CreatedDate => CreatedAt;

        // Navigation properties
        public virtual ICollection<InsurancePremiumPayment> PremiumPayments { get; set; } = new List<InsurancePremiumPayment>();
        public virtual ICollection<InsuranceClaim> Claims { get; set; } = new List<InsuranceClaim>();
        public virtual ICollection<InsuranceDocument> Documents { get; set; } = new List<InsuranceDocument>();
        public virtual ICollection<InsuranceBeneficiary> Beneficiaries { get; set; } = new List<InsuranceBeneficiary>();

       // Calculated properties
       [NotMapped]
        public decimal TotalPremiumsPaid => PremiumPayments?.Where(p => p.PaymentStatus == PaymentStatus.Completed).Sum(p => p.Amount) ?? 0;

        [NotMapped]
        public decimal OutstandingPremiums => PremiumPayments?.Where(p => p.PaymentStatus == PaymentStatus.Pending && p.DueDate < DateTime.Now).Sum(p => p.Amount) ?? 0;

        [NotMapped]
        public bool IsActive => Status == InsuranceStatus.Active && PolicyEndDate > DateTime.Now;

        [NotMapped]
        public int DaysUntilExpiry => (PolicyEndDate - DateTime.Now)?.Days ?? 0;

        [NotMapped]
        public decimal AnnualPremium => PremiumFrequency switch
        {
            PaymentFrequency.Monthly => PremiumAmount * 12,
            PaymentFrequency.Quarterly => PremiumAmount * 4,
            PaymentFrequency.SemiAnnual => PremiumAmount * 2,
            PaymentFrequency.Annual => PremiumAmount,
            _ => PremiumAmount * 12
        };
    }

    public class InsurancePremiumPayment : BaseEntity
    {
        public string InsuranceId { get; set; } = string.Empty;
        public virtual Insurance Insurance { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public DateTime? DueDate { get; set; }

        public DateTime? PaymentDate { get; set; }

        public PaymentStatus PaymentStatus { get; set; }

        [StringLength(50)]
        public string PaymentMethod { get; set; } = string.Empty;

        [StringLength(100)]
        public string TransactionReference { get; set; } = string.Empty;

        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal? LateFee { get; set; }

        public DateTime? LateFeeAppliedDate { get; set; }

        // Calculated properties
        [NotMapped]
        public bool IsOverdue => PaymentStatus == PaymentStatus.Pending && DueDate < DateTime.Now;

        [NotMapped]
        public int DaysOverdue => IsOverdue ? (DateTime.Now - DueDate)?.Days ?? 0 : 0;

        [NotMapped]
        public decimal TotalAmountDue => Amount + (LateFee ?? 0);

        public DateTime NextDueDate { get; internal set; }
    }

    public class InsuranceClaim : BaseEntity
    {
        public string InsuranceId { get; set; } = string.Empty;
        public virtual Insurance Insurance { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string ClaimNumber { get; set; } = string.Empty;

        public DateTime ClaimDate { get; set; }

        public DateTime IncidentDate { get; set; }

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [StringLength(200)]
        public string? IncidentLocation { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ClaimAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? ApprovedAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? PaidAmount { get; set; }

        public ClaimStatus Status { get; set; }

        public DateTime? SettlementDate { get; set; }

        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;

        // Navigation properties
        public virtual ICollection<InsuranceDocument> Documents { get; set; } = new List<InsuranceDocument>();
    }

    public class InsuranceDocument : BaseEntity
    {
        public string InsuranceId { get; set; } = string.Empty;
        public virtual Insurance Insurance { get; set; } = null!;

        public string? ClaimId { get; set; }
        public virtual InsuranceClaim? Claim { get; set; }

        [Required]
        [StringLength(200)]
        public string DocumentName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string DocumentType { get; set; } = string.Empty; // Policy, Receipt, Claim, Medical Report, etc.

        [Required]
        [StringLength(500)]
        public string FilePath { get; set; } = string.Empty;

        [StringLength(50)]
        public string FileType { get; set; } = string.Empty;

        public long FileSize { get; set; }

        public DateTime? UploadDate { get; set; }

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        public DateTime? ExpiryDate { get; internal set; }
    }

    public class InsuranceBeneficiary : BaseEntity
    {
        public string InsuranceId { get; set; } = string.Empty;
        public virtual Insurance Insurance { get; set; } = null!;

        [Required]
        [StringLength(200)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Relationship { get; set; } = string.Empty;

        [StringLength(15)]
        public string Phone { get; set; } = string.Empty;

        [StringLength(200)]
        public string Email { get; set; } = string.Empty;

        [StringLength(500)]
        public string Address { get; set; } = string.Empty;

        [Column(TypeName = "decimal(5,2)")]
        [Range(0, 100)]
        public decimal Percentage { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [StringLength(50)]
        public string IdentificationNumber { get; set; } = string.Empty;

        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        // Alias properties for backward compatibility
        [NotMapped]
        public DateTime CreatedDate => CreatedAt;
    }
}