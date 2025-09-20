using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pilgrims.PersonalFinances.Models.Enums;

namespace Pilgrims.PersonalFinances.Models
{
    /// <summary>
    /// Tracks the history of all notifications sent to users
    /// </summary>
    public class NotificationHistory
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Reference to the notification rule that triggered this notification
        /// </summary>
        public int? NotificationRuleId { get; set; }
        public virtual NotificationRule? NotificationRule { get; set; }

        [Required]
        public NotificationType NotificationType { get; set; }

        [Required]
        public NotificationChannel Channel { get; set; }

        [Required]
        public NotificationPriority Priority { get; set; }

        [Required]
        public NotificationStatus Status { get; set; } = NotificationStatus.Pending;

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Additional data payload (JSON format)
        /// </summary>
        [StringLength(2000)]
        public string? Data { get; set; }

        /// <summary>
        /// When the notification was scheduled to be sent
        /// </summary>
        [Required]
        public DateTime ScheduledAt { get; set; }

        /// <summary>
        /// When the notification was actually sent
        /// </summary>
        public DateTime? SentAt { get; set; }

        /// <summary>
        /// When the notification was read by the user
        /// </summary>
        public DateTime? ReadAt { get; set; }

        /// <summary>
        /// When the notification was dismissed by the user
        /// </summary>
        public DateTime? DismissedAt { get; set; }

        /// <summary>
        /// When the notification expires (for temporary notifications)
        /// </summary>
        public DateTime? ExpiresAt { get; set; }

        // Snooze functionality
        
        /// <summary>
        /// Number of times this notification has been snoozed
        /// </summary>
        public int SnoozeCount { get; set; } = 0;

        /// <summary>
        /// When the notification was last snoozed
        /// </summary>
        public DateTime? LastSnoozedAt { get; set; }

        /// <summary>
        /// When the snoozed notification should be shown again
        /// </summary>
        public DateTime? SnoozeUntil { get; set; }

        // Action tracking

        /// <summary>
        /// Action taken from the notification (if any)
        /// </summary>
        [StringLength(100)]
        public string? ActionTaken { get; set; }

        /// <summary>
        /// When the action was taken
        /// </summary>
        public DateTime? ActionTakenAt { get; set; }

        /// <summary>
        /// Result of the action (success, failure, etc.)
        /// </summary>
        [StringLength(200)]
        public string? ActionResult { get; set; }

        // Error handling

        /// <summary>
        /// Error message if notification failed to send
        /// </summary>
        [StringLength(500)]
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Number of retry attempts
        /// </summary>
        public int RetryCount { get; set; } = 0;

        /// <summary>
        /// When to retry sending (for failed notifications)
        /// </summary>
        public DateTime? RetryAt { get; set; }

        // Entity references (what the notification is about)

        /// <summary>
        /// Related transaction ID
        /// </summary>
        public int? TransactionId { get; set; }
        public virtual Transaction? Transaction { get; set; }

        /// <summary>
        /// Related scheduled transaction ID
        /// </summary>
        public int? ScheduledTransactionId { get; set; }
        public virtual ScheduledTransaction? ScheduledTransaction { get; set; }

        /// <summary>
        /// Related budget ID
        /// </summary>
        public int? BudgetId { get; set; }
        public virtual Budget? Budget { get; set; }

        /// <summary>
        /// Related debt ID
        /// </summary>
        public int? DebtId { get; set; }
        public virtual Debt? Debt { get; set; }

        /// <summary>
        /// Related account ID
        /// </summary>
        public int? AccountId { get; set; }
        public virtual Account? Account { get; set; }

        // Audit fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Computed properties
        [NotMapped]
        public bool IsExpired => ExpiresAt.HasValue && ExpiresAt.Value < DateTime.UtcNow;

        [NotMapped]
        public bool IsSnoozed => Status == NotificationStatus.Snoozed && 
                                SnoozeUntil.HasValue && 
                                SnoozeUntil.Value > DateTime.UtcNow;

        [NotMapped]
        public bool IsOverdue => Status == NotificationStatus.Pending && 
                                ScheduledAt < DateTime.UtcNow;

        [NotMapped]
        public bool CanSnooze => Status == NotificationStatus.Sent && 
                                NotificationRule?.NotificationSettings.AllowSnooze == true &&
                                SnoozeCount < (NotificationRule?.NotificationSettings.MaxSnoozeCount ?? 0);

        [NotMapped]
        public bool CanRetry => Status == NotificationStatus.Failed && 
                               RetryCount < 3 && 
                               (RetryAt == null || RetryAt <= DateTime.UtcNow);

        [NotMapped]
        public TimeSpan? TimeSinceSent => SentAt.HasValue ? DateTime.UtcNow - SentAt.Value : null;

        [NotMapped]
        public TimeSpan? TimeUntilExpiry => ExpiresAt.HasValue ? ExpiresAt.Value - DateTime.UtcNow : null;

        [NotMapped]
        public string EntityReference
        {
            get
            {
                if (TransactionId.HasValue) return $"Transaction:{TransactionId}";
                if (ScheduledTransactionId.HasValue) return $"ScheduledTransaction:{ScheduledTransactionId}";
                if (BudgetId.HasValue) return $"Budget:{BudgetId}";
                if (DebtId.HasValue) return $"Debt:{DebtId}";
                if (AccountId.HasValue) return $"Account:{AccountId}";
                return "System";
            }
        }

        /// <summary>
        /// Marks the notification as read
        /// </summary>
        public void MarkAsRead()
        {
            if (Status == NotificationStatus.Sent)
            {
                Status = NotificationStatus.Read;
                ReadAt = DateTime.UtcNow;
                UpdatedAt = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Dismisses the notification
        /// </summary>
        public void Dismiss()
        {
            Status = NotificationStatus.Dismissed;
            DismissedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Snoozes the notification for the specified duration
        /// </summary>
        public bool Snooze(TimeSpan duration)
        {
            if (!CanSnooze) return false;

            Status = NotificationStatus.Snoozed;
            SnoozeCount++;
            LastSnoozedAt = DateTime.UtcNow;
            SnoozeUntil = DateTime.UtcNow.Add(duration);
            UpdatedAt = DateTime.UtcNow;

            return true;
        }

        /// <summary>
        /// Records an action taken from this notification
        /// </summary>
        public void RecordAction(string action, string? result = null)
        {
            ActionTaken = action;
            ActionTakenAt = DateTime.UtcNow;
            ActionResult = result;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}