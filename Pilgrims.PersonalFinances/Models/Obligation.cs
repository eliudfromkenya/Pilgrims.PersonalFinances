using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pilgrims.PersonalFinances.Models.Enums;

namespace Pilgrims.PersonalFinances.Models
{
    public class Obligation : BaseEntity
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public ObligationType Type { get; set; }

        // Alias for Type for backward compatibility
        public ObligationType ObligationType => Type;

        [StringLength(200)]
        public string OrganizationName { get; set; } = string.Empty;

        // Alias for OrganizationName for backward compatibility
        public string Organization => OrganizationName;

        [StringLength(100)]
        public string MembershipNumber { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        // Financial Details
        [Column(TypeName = "decimal(18,2)")]
        public decimal ContributionAmount { get; set; }

        // Alias for ContributionAmount for backward compatibility
        public decimal MonthlyContribution => ContributionAmount;

        [Column(TypeName = "decimal(18,2)")]
        public decimal? CurrentBalance { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? OriginalAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? InterestRate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? MinimumPayment { get; set; }

        public PaymentFrequency ContributionFrequency { get; set; }

        // Alias for ContributionFrequency for backward compatibility
        public PaymentFrequency PaymentFrequency => ContributionFrequency;

        // Dates
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? NextContributionDueDate { get; set; }

        // Alias for NextContributionDueDate for backward compatibility
        public DateTime? NextContributionDue => NextContributionDueDate;

        public DateTime? LastPaymentDate { get; set; }

        // Status and Terms
        public ObligationStatus Status { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? LateFeeAmount { get; set; }

        public int? GracePeriodDays { get; set; }

        public int? TermInMonths { get; set; }

        // Credit and Debt specific fields
        [Column(TypeName = "decimal(18,2)")]
        public decimal? CreditLimit { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? AvailableCredit { get; set; }

        public bool IsCompoundInterest { get; set; } = false;

        [StringLength(15)]
        public string ContactPhone { get; set; } = string.Empty;

        [StringLength(200)]
        public string ContactEmail { get; set; } = string.Empty;

        [StringLength(300)]
        public string ContactAddress { get; set; } = string.Empty;

        [StringLength(100)]
        public string ContactPerson { get; set; } = string.Empty;

        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;

        [StringLength(200)]
        public string Website { get; set; } = string.Empty;

        public int Priority { get; set; } = 1;

        // Benefits and returns
        [Column(TypeName = "decimal(18,2)")]
        public decimal? ExpectedAnnualReturn { get; set; }

        [StringLength(500)]
        public string BenefitsDescription { get; set; } = string.Empty;

        public DateTime? LastDividendDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? LastDividendAmount { get; set; }

        // Navigation properties
        public virtual ICollection<ObligationPayment> Payments { get; set; } = new List<ObligationPayment>();
        public virtual ICollection<ObligationBenefit> Benefits { get; set; } = new List<ObligationBenefit>();
        public virtual ICollection<ObligationDocument> Documents { get; set; } = new List<ObligationDocument>();

        // Calculated properties
        [NotMapped]
        public decimal TotalContributionsPaid => Payments?.Where(p => p.PaymentStatus == PaymentStatus.Completed).Sum(p => p.Amount) ?? 0;

        [NotMapped]
        public decimal OutstandingContributions => Payments?.Where(p => p.PaymentStatus == PaymentStatus.Pending && p.DueDate < DateTime.Now).Sum(p => p.Amount) ?? 0;

        [NotMapped]
        public decimal TotalArrears => Payments?.Where(p => p.PaymentStatus == PaymentStatus.Overdue).Sum(p => p.TotalAmountDue) ?? 0;

        [NotMapped]
        public bool IsActive => Status == ObligationStatus.Active && (EndDate == null || EndDate > DateTime.Now);

        [NotMapped]
        public decimal AnnualContribution => ContributionFrequency switch
        {
            PaymentFrequency.Weekly => ContributionAmount * 52,
            PaymentFrequency.BiWeekly => ContributionAmount * 26,
            PaymentFrequency.Monthly => ContributionAmount * 12,
            PaymentFrequency.Quarterly => ContributionAmount * 4,
            PaymentFrequency.SemiAnnual => ContributionAmount * 2,
            PaymentFrequency.Annual => ContributionAmount,
            _ => ContributionAmount * 12
        };

        [NotMapped]
        public decimal TotalBenefitsReceived => Benefits?.Sum(b => b.Amount) ?? 0;

        [NotMapped]
        public decimal RemainingBalance => CurrentBalance ?? 0;

        [NotMapped]
        public decimal CreditUtilization => CreditLimit > 0 ? (CurrentBalance ?? 0) / CreditLimit.Value * 100 : 0;

        [NotMapped]
        public decimal MonthlyInterestRate => (InterestRate ?? 0) / 100 / 12;

        [NotMapped]
        public bool IsDebt => Type == ObligationType.CreditCard || Type == ObligationType.Mortgage || 
                             Type == ObligationType.PersonalLoan || Type == ObligationType.StudentLoan || 
                             Type == ObligationType.AutoLoan || Type == ObligationType.BusinessLoan || 
                             Type == ObligationType.MedicalDebt;

        [NotMapped]
        public bool IsOverdue => NextContributionDueDate < DateTime.Now && Status == ObligationStatus.Active;

        [NotMapped]
        public int DaysOverdue => IsOverdue ? (DateTime.Now - NextContributionDueDate)?.Days??0 : 0;

        [NotMapped]
        public decimal TotalInterestPaid => Payments?.Where(p => p.PaymentStatus == PaymentStatus.Completed && p.InterestAmount.HasValue)
                                                   .Sum(p => p.InterestAmount.Value) ?? 0;

        [NotMapped]
        public decimal NetContribution => TotalContributionsPaid - TotalBenefitsReceived;
    }

    public class ObligationPayment : BaseEntity
    {
        public string ObligationId { get; set; } = string.Empty;
        public virtual Obligation Obligation { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public DateTime DueDate { get; set; }

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

        [StringLength(100)]
        public string PaymentPeriod { get; set; } = string.Empty; // e.g., "January 2024", "Q1 2024"

        // Interest and Principal breakdown for debt payments
        [Column(TypeName = "decimal(18,2)")]
        public decimal? PrincipalAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? InterestAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? RemainingBalance { get; set; }

        public bool IsExtraPayment { get; set; } = false;

        // Calculated properties
        [NotMapped]
        public bool IsOverdue => PaymentStatus == PaymentStatus.Pending && DueDate < DateTime.Now;

        [NotMapped]
        public int DaysOverdue => IsOverdue ? (DateTime.Now - DueDate).Days : 0;

        [NotMapped]
        public decimal TotalAmountDue => Amount + (LateFee ?? 0);

        [NotMapped]
        public bool IsInGracePeriod => IsOverdue && Obligation?.GracePeriodDays != null && DaysOverdue <= Obligation.GracePeriodDays;
    }

    public class ObligationBenefit : BaseEntity
    {
        public string ObligationId { get; set; } = string.Empty;
        public virtual Obligation Obligation { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string BenefitType { get; set; } = string.Empty; // Dividend, Loan, Emergency Fund, etc.

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public DateTime ReceivedDate { get; set; }

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [StringLength(100)]
        public string ReferenceNumber { get; set; } = string.Empty;

        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;

        public BenefitStatus Status { get; set; }

        public DateTime? ProcessedDate { get; set; }

        [StringLength(100)]
        public string ProcessedBy { get; set; } = string.Empty;
    }

    public class ObligationDocument : BaseEntity
    {
        public string ObligationId { get; set; } = string.Empty;
        public virtual Obligation Obligation { get; set; } = null!;

        [Required]
        [StringLength(200)]
        public string DocumentName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string DocumentType { get; set; } = string.Empty; // Certificate, Receipt, Statement, Agreement, etc.

        [Required]
        [StringLength(500)]
        public string FilePath { get; set; } = string.Empty;

        [StringLength(50)]
        public string FileType { get; set; } = string.Empty;

        public long FileSize { get; set; }

        public DateTime UploadDate { get; set; }

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        public DateTime? ExpiryDate { get; set; }

        public bool IsActive { get; set; } = true;
    }
}