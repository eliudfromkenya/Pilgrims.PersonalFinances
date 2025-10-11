using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Pilgrims.PersonalFinances.Core.Models.Enums;

namespace Pilgrims.PersonalFinances.Core.Models
{
    /// <summary>
    /// Defines specific rules for notification triggers
    /// </summary>
    public class NotificationRule : BaseEntity
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public NotificationType NotificationType { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public NotificationPriority Priority { get; set; } = NotificationPriority.Medium;

        /// <summary>
        /// Frequency of the notification rule
        /// </summary>
        public NotificationFrequency Frequency { get; set; } = NotificationFrequency.Once;

        /// <summary>
        /// Reference to the notification settings
        /// </summary>
        [Required]
        public string? NotificationSettingsId { get; set; }
        public virtual NotificationSettings NotificationSettings { get; set; } = null!;

        // Entity-specific foreign keys (nullable - only one should be set per rule)
        
        /// <summary>
        /// For scheduled transaction reminders
        /// </summary>
        public string? ScheduledTransactionId { get; set; }
        public virtual ScheduledTransaction? ScheduledTransaction { get; set; }

        /// <summary>
        /// For budget alerts
        /// </summary>
        public string? BudgetId { get; set; }
        public virtual Budget? Budget { get; set; }

        /// <summary>
        /// For debt payment reminders
        /// </summary>
        public string? DebtId { get; set; }
        public virtual Debt? Debt { get; set; }

        /// <summary>
        /// For account-specific notifications
        /// </summary>
        public string? AccountId { get; set; }
        public virtual Account? Account { get; set; }

        /// <summary>
        /// For category-specific notifications
        /// </summary>
        public string? CategoryId { get; set; }
        public virtual Category? Category { get; set; }

        // Rule conditions

        /// <summary>
        /// Minimum amount threshold for triggering notification
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? MinAmount { get; set; }

        /// <summary>
        /// Maximum amount threshold for triggering notification
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? MaxAmount { get; set; }

        /// <summary>
        /// Specific day of month for monthly notifications (1-31)
        /// </summary>
        public int? DayOfMonth { get; set; }

        /// <summary>
        /// Specific day of week for weekly notifications
        /// </summary>
        public DayOfWeek? DayOfWeek { get; set; }

        /// <summary>
        /// Custom condition expression (for advanced rules)
        /// </summary>
        [StringLength(1000)]
        public string? CustomCondition { get; set; }

        /// <summary>
        /// Last time this rule was evaluated
        /// </summary>
        public DateTime? LastEvaluated { get; set; }

        /// <summary>
        /// Last time this rule triggered a notification
        /// </summary>
        public DateTime? LastTriggered { get; set; }

        /// <summary>
        /// Number of times this rule has been triggered
        /// </summary>
        public int TriggerCount { get; set; } = 0;

        /// <summary>
        /// Maximum number of triggers allowed (null = unlimited)
        /// </summary>
        public int? MaxTriggers { get; set; }

        /// <summary>
        /// Rule expiration date (null = never expires)
        /// </summary>
        public DateTime? ExpiresAt { get; set; }

        // Navigation properties
        public virtual ICollection<NotificationHistory> NotificationHistory { get; set; } = new List<NotificationHistory>();

        // Computed properties
        [NotMapped]
        public bool IsExpired => ExpiresAt.HasValue && ExpiresAt.Value < DateTime.UtcNow;

        [NotMapped]
        public bool HasReachedMaxTriggers => MaxTriggers.HasValue && TriggerCount >= MaxTriggers.Value;

        [NotMapped]
        public bool CanTrigger => IsActive && !IsExpired && !HasReachedMaxTriggers;

        [NotMapped]
        public string EntityReference
        {
            get
            {
                if (!string.IsNullOrEmpty(ScheduledTransactionId)) return $"ScheduledTransaction:{ScheduledTransactionId}";
                if (!string.IsNullOrEmpty(BudgetId)) return $"Budget:{BudgetId}";
                if (!string.IsNullOrEmpty(DebtId)) return $"Debt:{DebtId}";
                if (!string.IsNullOrEmpty(AccountId)) return $"Account:{AccountId}";
                if (!string.IsNullOrEmpty(CategoryId)) return $"Category:{CategoryId}";
                return "System";
            }
        }

        /// <summary>
        /// Validates the notification rule
        /// </summary>
        public ValidationResult Validate()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(Name))
                errors.Add("Rule name is required");

            if (string.IsNullOrEmpty(NotificationSettingsId))
                errors.Add("Notification settings reference is required");

            // Ensure only one entity reference is set
            var entityReferences = new object?[] { ScheduledTransactionId, BudgetId, DebtId, AccountId, CategoryId }
                .Count(id => 
                {
                    if (id is int intId) return intId > 0;
                    if (id is string strId) return !string.IsNullOrEmpty(strId);
                    return false;
                });

            if (entityReferences > 1)
                errors.Add("Only one entity reference can be set per rule");

            // Validate amount thresholds
            if (MinAmount.HasValue && MaxAmount.HasValue && MinAmount > MaxAmount)
                errors.Add("Minimum amount cannot be greater than maximum amount");

            // Validate day of month
            if (DayOfMonth.HasValue && (DayOfMonth < 1 || DayOfMonth > 31))
                errors.Add("Day of month must be between 1 and 31");

            // Validate max triggers
            if (MaxTriggers.HasValue && MaxTriggers <= 0)
                errors.Add("Maximum triggers must be greater than zero");

            return new ValidationResult
            {
                IsValid = !errors.Any(),
                Errors = errors
            };
        }
    }
}
