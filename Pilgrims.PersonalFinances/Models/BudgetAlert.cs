using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pilgrims.PersonalFinances.Models.Enums;

namespace Pilgrims.PersonalFinances.Models
{
    /// <summary>
    /// Represents a budget alert notification
    /// </summary>
    public class BudgetAlert : BaseEntity
    {
        /// <summary>
        /// Budget ID this alert belongs to
        /// </summary>
        [Required]
        public string BudgetId { get; set; } = string.Empty;

        /// <summary>
        /// Alert level percentage (50, 75, 90, 100)
        /// </summary>
        [Required]
        public int AlertLevel { get; set; }

        /// <summary>
        /// Budget amount at the time of alert
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal BudgetAmount { get; set; }

        /// <summary>
        /// Spent amount at the time of alert
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal SpentAmount { get; set; }

        /// <summary>
        /// Percentage used at the time of alert
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal UsedPercentage { get; set; }

        /// <summary>
        /// Alert message
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Whether the alert has been read/acknowledged
        /// </summary>
        public bool IsRead { get; set; } = false;

        /// <summary>
        /// Date and time when the alert was read
        /// </summary>
        public DateTime? ReadDate { get; set; }

        // Navigation properties
        /// <summary>
        /// Budget this alert belongs to
        /// </summary>
        public virtual Budget Budget { get; set; } = null!;

        // Computed properties
        /// <summary>
        /// Formatted budget amount
        /// </summary>
        [NotMapped]
        public string FormattedBudgetAmount => BudgetAmount.ToString("C2");

        /// <summary>
        /// Formatted spent amount
        /// </summary>
        [NotMapped]
        public string FormattedSpentAmount => SpentAmount.ToString("C2");

        /// <summary>
        /// Alert level display text
        /// </summary>
        [NotMapped]
        public string AlertLevelDisplay => AlertLevel switch
        {
            50 => "50% Budget Used",
            75 => "75% Budget Used", 
            90 => "90% Budget Used",
            100 => "Budget Exceeded",
            _ => $"{AlertLevel}% Budget Used"
        };

        /// <summary>
        /// Alert severity for UI styling
        /// </summary>
        [NotMapped]
        public string AlertSeverity => AlertLevel switch
        {
            50 => "Info",
            75 => "Warning",
            90 => "Warning",
            100 => "Error",
            _ => "Info"
        };

        /// <summary>
        /// Time since alert was created
        /// </summary>
        [NotMapped]
        public string TimeAgo
        {
            get
            {
                var timeSpan = DateTime.UtcNow - CreatedAt;
                
                if (timeSpan.TotalMinutes < 1)
                    return "Just now";
                if (timeSpan.TotalMinutes < 60)
                    return $"{(int)timeSpan.TotalMinutes} minutes ago";
                if (timeSpan.TotalHours < 24)
                    return $"{(int)timeSpan.TotalHours} hours ago";
                if (timeSpan.TotalDays < 7)
                    return $"{(int)timeSpan.TotalDays} days ago";
                
                return CreatedAt.ToString("MMM dd, yyyy");
            }
        }

        // Methods
        /// <summary>
        /// Marks the alert as read
        /// </summary>
        public void MarkAsRead()
        {
            IsRead = true;
            ReadDate = DateTime.UtcNow;
        }

        /// <summary>
        /// Creates a budget alert message
        /// </summary>
        /// <param name="budget">Budget that triggered the alert</param>
        /// <param name="alertLevel">Alert level percentage</param>
        /// <returns>Budget alert instance</returns>
        public static BudgetAlert Create(Budget budget, int alertLevel)
        {
            var message = alertLevel switch
            {
                50 => $"You've used 50% of your '{budget.Name}' budget ({budget.FormattedSpentAmount} of {budget.FormattedLimitAmount})",
                75 => $"You've used 75% of your '{budget.Name}' budget ({budget.FormattedSpentAmount} of {budget.FormattedLimitAmount})",
                90 => $"Warning: You've used 90% of your '{budget.Name}' budget ({budget.FormattedSpentAmount} of {budget.FormattedLimitAmount})",
                100 => $"Alert: You've exceeded your '{budget.Name}' budget! ({budget.FormattedSpentAmount} of {budget.FormattedLimitAmount})",
                _ => $"You've used {alertLevel}% of your '{budget.Name}' budget ({budget.FormattedSpentAmount} of {budget.FormattedLimitAmount})"
            };

            return new BudgetAlert
            {
                BudgetId = budget.Id,
                AlertLevel = alertLevel,
                BudgetAmount = budget.LimitAmount + budget.RolloverAmount,
                SpentAmount = budget.SpentAmount,
                UsedPercentage = budget.UsedPercentage,
                Message = message
            };
        }
    }
}