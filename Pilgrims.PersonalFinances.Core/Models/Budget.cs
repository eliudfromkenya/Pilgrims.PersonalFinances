using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pilgrims.PersonalFinances.Core.Models.Enums;

namespace Pilgrims.PersonalFinances.Core.Models
{
    /// <summary>
    /// Represents a budget with tracking and alert capabilities
    /// </summary>
    public class Budget : BaseEntity
    {
        /// <summary>
        /// User ID for the budget owner
        /// </summary>
        [Required]
        public string UserId { get; set; } = string.Empty;
        /// <summary>
        /// Name of the budget
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description of the budget
        /// </summary>
        [MaxLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// Type of budget (Category, Account, Tag, TimePeriod)
        /// </summary>
        [Required]
        public BudgetType BudgetType { get; set; }

        /// <summary>
        /// Budget period (Weekly, Monthly, Quarterly, Annual)
        /// </summary>
        [Required]
        public BudgetPeriod Period { get; set; }

        /// <summary>
        /// Budget limit amount
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal LimitAmount { get; set; }

        /// <summary>
        /// Current spent amount
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal SpentAmount { get; set; } = 0;

        /// <summary>
        /// Start date of the budget period
        /// </summary>
        [Required]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// End date of the budget period
        /// </summary>
        [Required]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Whether the budget is active
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Whether to allow overspending
        /// </summary>
        public bool AllowOverspend { get; set; } = true;

        /// <summary>
        /// Whether to rollover unused budget to next period
        /// </summary>
        public bool EnableRollover { get; set; } = false;

        /// <summary>
        /// Rollover amount from previous period
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal RolloverAmount { get; set; } = 0;

        /// <summary>
        /// Category ID for category-based budgets
        /// </summary>
        public string? CategoryId { get; set; }

        /// <summary>
        /// Account ID for account-based budgets
        /// </summary>
        public string? AccountId { get; set; }

        /// <summary>
        /// Tag for tag-based budgets
        /// </summary>
        [MaxLength(50)]
        public string? Tag { get; set; }

        /// <summary>
        /// Goal ID for linking budgets to goals
        /// </summary>
        public int? GoalId { get; set; }

        /// <summary>
        /// Alert levels enabled for this budget (comma-separated)
        /// </summary>
        [MaxLength(20)]
        public string AlertLevels { get; set; } = "50,75,90,100";

        /// <summary>
        /// Whether alerts are enabled
        /// </summary>
        public bool AlertsEnabled { get; set; } = true;

        /// <summary>
        /// Whether this budget is a template
        /// </summary>
        public bool IsTemplate { get; set; } = false;

        /// <summary>
        /// Last alert level that was triggered
        /// </summary>
        public int? LastAlertLevel { get; set; }

        // Navigation properties
        /// <summary>
        /// Category for category-based budgets
        /// </summary>
        public virtual Category? Category { get; set; }

        /// <summary>
        /// Account for account-based budgets
        /// </summary>
        public virtual Account? Account { get; set; }

        /// <summary>
        /// Goal for budget-goal linking
        /// </summary>
        public virtual Goal? Goal { get; set; }

        /// <summary>
        /// Budget alerts for this budget
        /// </summary>
        public virtual ICollection<BudgetAlert> BudgetAlerts { get; set; } = new List<BudgetAlert>();

        /// <summary>
        /// Budget categories for this budget
        /// </summary>
        public virtual ICollection<BudgetCategory> Categories { get; set; } = new List<BudgetCategory>();

        // Navigation properties for data linking
        public virtual ICollection<Transaction> RelatedTransactions { get; set; } = new List<Transaction>();
        public virtual ICollection<Debt> RelatedDebts { get; set; } = new List<Debt>();

        // Computed properties
        /// <summary>
        /// Remaining budget amount
        /// </summary>
        [NotMapped]
        public decimal RemainingAmount => (LimitAmount + RolloverAmount) - SpentAmount;

        /// <summary>
        /// Percentage of budget used
        /// </summary>
        [NotMapped]
        public decimal UsedPercentage => LimitAmount + RolloverAmount > 0 
            ? Math.Round((SpentAmount / (LimitAmount + RolloverAmount)) * 100, 2) 
            : 0;

        /// <summary>
        /// Utilization percentage (alias for UsedPercentage for compatibility)
        /// </summary>
        [NotMapped]
        public decimal UtilizationPercentage => UsedPercentage;

        /// <summary>
        /// Whether the budget is over the limit
        /// </summary>
        [NotMapped]
        public bool IsOverBudget => SpentAmount > (LimitAmount + RolloverAmount);

        /// <summary>
        /// Whether the budget is currently active (within date range)
        /// </summary>
        [NotMapped]
        public bool IsCurrentlyActive => IsActive && DateTime.Today >= StartDate.Date && DateTime.Today <= EndDate.Date;

        /// <summary>
        /// Formatted limit amount
        /// </summary>
        [NotMapped]
        public string FormattedLimitAmount => LimitAmount.ToString("C2");

        /// <summary>
        /// Formatted spent amount
        /// </summary>
        [NotMapped]
        public string FormattedSpentAmount => SpentAmount.ToString("C2");

        /// <summary>
        /// Formatted remaining amount
        /// </summary>
        [NotMapped]
        public string FormattedRemainingAmount => RemainingAmount.ToString("C2");

        /// <summary>
        /// Gets the alert levels as a list
        /// </summary>
        [NotMapped]
        public List<int> AlertLevelsList => string.IsNullOrEmpty(AlertLevels) 
            ? new List<int>() 
            : AlertLevels.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => int.TryParse(x.Trim(), out var level) ? level : 0)
                        .Where(x => x > 0)
                        .ToList();

        /// <summary>
        /// Gets the budget type display name
        /// </summary>
        [NotMapped]
        public string BudgetTypeDisplay => BudgetType switch
        {
            BudgetType.Category => "Category",
            BudgetType.Account => "Account", 
            BudgetType.Tag => "Tag",
            BudgetType.TimePeriod => "Time Period",
            _ => "Unknown"
        };

        /// <summary>
        /// Gets the period display name
        /// </summary>
        [NotMapped]
        public string PeriodDisplay => Period switch
        {
            BudgetPeriod.Weekly => "Weekly",
            BudgetPeriod.Monthly => "Monthly",
            BudgetPeriod.Quarterly => "Quarterly",
            BudgetPeriod.Yearly => "Annual",
            _ => "Custom"
        };

        // Methods
        /// <summary>
        /// Validates the budget
        /// </summary>
        /// <returns>List of validation errors</returns>
        public List<string> Validate()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(Name))
                errors.Add("Budget name is required");

            if (LimitAmount <= 0)
                errors.Add("Budget limit must be greater than 0");

            if (StartDate >= EndDate)
                errors.Add("End date must be after start date");

            if (BudgetType == BudgetType.Category && string.IsNullOrEmpty(CategoryId))
                errors.Add("Category is required for category-based budgets");

            if (BudgetType == BudgetType.Account && AccountId == null)
                errors.Add("Account is required for account-based budgets");

            if (BudgetType == BudgetType.Tag && string.IsNullOrWhiteSpace(Tag))
                errors.Add("Tag is required for tag-based budgets");

            return errors;
        }

        /// <summary>
        /// Checks if an alert should be triggered for the current usage
        /// </summary>
        /// <returns>Alert level to trigger, or null if no alert needed</returns>
        public int? CheckForAlert()
        {
            if (!AlertsEnabled || !IsCurrentlyActive) return null;

            var alertLevels = AlertLevelsList.OrderBy(x => x).ToList();
            var currentPercentage = (int)Math.Floor(UsedPercentage);

            foreach (var level in alertLevels)
            {
                if (currentPercentage >= level && (LastAlertLevel == null || level > LastAlertLevel))
                {
                    return level;
                }
            }

            return null;
        }

        /// <summary>
        /// Updates the spent amount and triggers alerts if necessary
        /// </summary>
        /// <param name="newSpentAmount">New spent amount</param>
        /// <returns>Alert level triggered, or null</returns>
        public int? UpdateSpentAmount(decimal newSpentAmount)
        {
            SpentAmount = newSpentAmount;
            MarkAsDirty();
            
            var alertLevel = CheckForAlert();
            if (alertLevel.HasValue)
            {
                LastAlertLevel = alertLevel.Value;
            }

            return alertLevel;
        }
    }
}
