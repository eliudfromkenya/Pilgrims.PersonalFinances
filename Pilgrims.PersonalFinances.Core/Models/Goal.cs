using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pilgrims.PersonalFinances.Models.Enums;

namespace Pilgrims.PersonalFinances.Models;

public class Goal : BaseEntity
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    public GoalType GoalType { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TargetAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal CurrentAmount { get; set; } = 0;

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime TargetDate { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsCompleted { get; set; } = false;

    public DateTime? CompletedDate { get; set; }

    [Range(1, 5)]
    public int Priority { get; set; } = 3; // 1 = Highest, 5 = Lowest

    [StringLength(50)]
    public string? Category { get; set; }

    [StringLength(20)]
    public string? Icon { get; set; }

    [StringLength(50)]
    public string? Color { get; set; }

    public bool EnableReminders { get; set; } = true;

    public int ReminderFrequencyDays { get; set; } = 30;

    public DateTime? LastReminderDate { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }

    // Navigation Properties
    public virtual ICollection<Budget> RelatedBudgets { get; set; } = new List<Budget>();
    public virtual ICollection<Transaction> RelatedTransactions { get; set; } = new List<Transaction>();

    // Computed Properties
    [NotMapped]
    public decimal RemainingAmount => TargetAmount - CurrentAmount;

    [NotMapped]
    public decimal ProgressPercentage => TargetAmount > 0 ? Math.Round((CurrentAmount / TargetAmount) * 100, 2) : 0;

    [NotMapped]
    public bool IsOnTrack
    {
        get
        {
            if (IsCompleted) return true;
            
            var totalDays = (TargetDate - StartDate).TotalDays;
            var elapsedDays = (DateTime.Now - StartDate).TotalDays;
            
            if (totalDays <= 0) return false;
            
            var expectedProgress = (elapsedDays / totalDays) * 100;
            return ProgressPercentage >= (decimal)(expectedProgress * 0.9); // 90% of expected progress
        }
    }

    [NotMapped]
    public int DaysRemaining => (TargetDate - DateTime.Now).Days;

    [NotMapped]
    public decimal MonthlyTargetAmount
    {
        get
        {
            var monthsRemaining = Math.Max(1, (TargetDate.Year - DateTime.Now.Year) * 12 + TargetDate.Month - DateTime.Now.Month);
            return RemainingAmount / monthsRemaining;
        }
    }

    [NotMapped]
    public string FormattedTargetAmount => TargetAmount.ToString("C");

    [NotMapped]
    public string FormattedCurrentAmount => CurrentAmount.ToString("C");

    [NotMapped]
    public string FormattedRemainingAmount => RemainingAmount.ToString("C");

    // Methods
    public void UpdateProgress(decimal amount)
    {
        CurrentAmount += amount;
        
        if (CurrentAmount >= TargetAmount && !IsCompleted)
        {
            IsCompleted = true;
            CompletedDate = DateTime.Now;
        }
        else if (CurrentAmount < TargetAmount && IsCompleted)
        {
            IsCompleted = false;
            CompletedDate = null;
        }
    }

    public bool IsReminderDue()
    {
        if (!EnableReminders || IsCompleted) return false;
        
        return LastReminderDate == null || 
               (DateTime.Now - LastReminderDate.Value).TotalDays >= ReminderFrequencyDays;
    }

    public void MarkReminderSent()
    {
        LastReminderDate = DateTime.Now;
    }

    public ValidationResult Validate()
    {
        var result = new ValidationResult();

        if (string.IsNullOrWhiteSpace(Name))
            result.AddError("Goal name is required");

        if (TargetAmount <= 0)
            result.AddError("Target amount must be greater than zero");

        if (StartDate >= TargetDate)
            result.AddError("Target date must be after start date");

        if (CurrentAmount < 0)
            result.AddError("Current amount cannot be negative");

        if (Priority < 1 || Priority > 5)
            result.AddError("Priority must be between 1 and 5");

        return result;
    }
}