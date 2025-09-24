using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pilgrims.PersonalFinances.Models;

public class Income: BaseEntity
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [Required]
    [StringLength(50)]
    public string IncomeType { get; set; } = "Salary"; // Salary, Freelance, Investment, Rental, Other

    [Required]
    [StringLength(50)]
    public string Frequency { get; set; } = "Monthly"; // Weekly, Bi-weekly, Monthly, Quarterly, Annually, One-time

    public bool IsRegular { get; set; } = true; // Regular vs Variable income

    public bool IsPreTax { get; set; } = true; // Pre-tax vs Post-tax

    [Column(TypeName = "decimal(18,2)")]
    public decimal? TaxRate { get; set; } // Tax rate percentage for calculations

    [Column(TypeName = "decimal(18,2)")]
    public decimal? NetAmount { get; set; } // Calculated net amount after taxes

    public DateTime ReceivedDate { get; set; } = DateTime.Today;

    public DateTime? NextExpectedDate { get; set; } // For recurring income

    public DateTime? StartDate { get; set; } // For recurring income series

    public DateTime? EndDate { get; set; } // For recurring income series

    [StringLength(100)]
    public string? Source { get; set; } // Company name, client name, etc.

    [StringLength(100)]
    public string? PaymentMethod { get; set; } // Direct deposit, check, cash, etc.

    [StringLength(100)]
    public string? ReferenceNumber { get; set; } // Invoice number, payroll reference, etc.

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    // Audit fields - CreatedAt and UpdatedAt are inherited from BaseEntity

    // Foreign Keys
    public string? IncomeCategoryId { get; set; }

    // Navigation Properties
    public virtual IncomeCategory? IncomeCategory { get; set; }

    // Calculated Properties
    [NotMapped]
    public decimal CalculatedNetAmount => IsPreTax && TaxRate.HasValue 
        ? Amount * (1 - TaxRate.Value / 100) 
        : Amount;

    [NotMapped]
    public string FrequencyDisplay => Frequency switch
    {
        "Weekly" => "Weekly",
        "Bi-weekly" => "Bi-weekly",
        "Monthly" => "Monthly", 
        "Quarterly" => "Quarterly",
        "Annually" => "Annually",
        "One-time" => "One-time",
        _ => Frequency
    };

    [NotMapped]
    public string IncomeTypeDisplay => IncomeType switch
    {
        "Salary" => "Salary/Wages",
        "Freelance" => "Freelance/Contract",
        "Investment" => "Investment Income",
        "Rental" => "Rental Income",
        "Other" => "Other Income",
        _ => IncomeType
    };
}