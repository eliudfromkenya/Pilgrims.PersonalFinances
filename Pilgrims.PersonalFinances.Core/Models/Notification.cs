using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pilgrims.PersonalFinances.Models.Enums;

namespace Pilgrims.PersonalFinances.Models;

public class Notification : BaseEntity
{
    [Required]
    public AppNotificationType Type { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(1000)]
    public string Message { get; set; } = string.Empty;

    [Required]
    public DateTime ScheduledDate { get; set; }

    public DateTime? SentDate { get; set; }

    public DateTime? ReadDate { get; set; }

    public DateTime? DismissedDate { get; set; }

    public DateTime? SnoozedUntil { get; set; }

    public NotificationStatus Status { get; set; } = NotificationStatus.Pending;

    public NotificationPriority Priority { get; set; } = NotificationPriority.Medium;

    public bool IsRecurring { get; set; } = false;

    public RecurrenceType? RecurrenceType { get; set; }

    public int? RecurrenceInterval { get; set; }

    public DateTime? RecurrenceEndDate { get; set; }

    // Related Entity IDs (nullable - depends on notification type)
    public int? TransactionId { get; set; }
    public int? BudgetId { get; set; }
    public int? DebtId { get; set; }
    public int? GoalId { get; set; }
    public int? ScheduledTransactionId { get; set; }

    [StringLength(500)]
    public string? ActionData { get; set; } // JSON data for quick actions

    [StringLength(100)]
    public string? ActionUrl { get; set; } // URL for navigation

    [StringLength(50)]
    public string? Icon { get; set; }

    [StringLength(50)]
    public string? Color { get; set; }

    public bool EnableSound { get; set; } = true;

    public bool EnableVibration { get; set; } = true;

    public int AdvanceNoticeDays { get; set; } = 1;

    public int MaxSnoozeCount { get; set; } = 3;

    public int CurrentSnoozeCount { get; set; } = 0;

    [StringLength(500)]
    public string? Notes { get; set; }

    // Navigation Properties
    public virtual Transaction? Transaction { get; set; }
    public virtual Budget? Budget { get; set; }
    public virtual Debt? Debt { get; set; }
    public virtual Goal? Goal { get; set; }
    public virtual ScheduledTransaction? ScheduledTransaction { get; set; }

    // Computed Properties
    [NotMapped]
    public bool IsOverdue => Status == NotificationStatus.Pending && ScheduledDate < DateTime.Now;

    [NotMapped]
    public bool IsSnoozed => SnoozedUntil.HasValue && SnoozedUntil > DateTime.Now;

    [NotMapped]
    public bool CanSnooze => CurrentSnoozeCount < MaxSnoozeCount && Status == NotificationStatus.Pending;

    [NotMapped]
    public string FormattedScheduledDate => ScheduledDate.ToString("MMM dd, yyyy h:mm tt");

    [NotMapped]
    public string RelativeScheduledDate
    {
        get
        {
            var diff = ScheduledDate - DateTime.Now;
            if (diff.TotalDays > 1)
                return $"in {diff.Days} days";
            else if (diff.TotalHours > 1)
                return $"in {(int)diff.TotalHours} hours";
            else if (diff.TotalMinutes > 1)
                return $"in {(int)diff.TotalMinutes} minutes";
            else if (diff.TotalSeconds > 0)
                return "in a few seconds";
            else
                return "overdue";
        }
    }

    // Methods
    public void MarkAsSent()
    {
        Status = NotificationStatus.Sent;
        SentDate = DateTime.Now;
    }

    public void MarkAsRead()
    {
        if (Status == NotificationStatus.Sent)
        {
            Status = NotificationStatus.Read;
            ReadDate = DateTime.Now;
        }
    }

    public void Dismiss()
    {
        Status = NotificationStatus.Dismissed;
        DismissedDate = DateTime.Now;
    }

    public bool Snooze(int minutes)
    {
        if (!CanSnooze) return false;

        SnoozedUntil = DateTime.Now.AddMinutes(minutes);
        CurrentSnoozeCount++;
        Status = NotificationStatus.Snoozed;
        return true;
    }

    public void ClearSnooze()
    {
        SnoozedUntil = null;
        if (Status == NotificationStatus.Snoozed)
        {
            Status = NotificationStatus.Pending;
        }
    }

    public Notification? CreateNextRecurrence()
    {
        if (!IsRecurring || RecurrenceType == null) return null;

        var nextDate = RecurrenceType switch
        {
            Enums.RecurrenceType.Daily => ScheduledDate.AddDays(RecurrenceInterval ?? 1),
            Enums.RecurrenceType.Weekly => ScheduledDate.AddDays((RecurrenceInterval ?? 1) * 7),
            Enums.RecurrenceType.Monthly => ScheduledDate.AddMonths(RecurrenceInterval ?? 1),
            Enums.RecurrenceType.Annually => ScheduledDate.AddYears(RecurrenceInterval ?? 1),
            _ => ScheduledDate.AddDays(1)
        };

        if (RecurrenceEndDate.HasValue && nextDate > RecurrenceEndDate.Value)
            return null;

        return new Notification
        {
            Type = Type,
            Title = Title,
            Message = Message,
            ScheduledDate = nextDate,
            Priority = Priority,
            IsRecurring = IsRecurring,
            RecurrenceType = RecurrenceType,
            RecurrenceInterval = RecurrenceInterval,
            RecurrenceEndDate = RecurrenceEndDate,
            TransactionId = TransactionId,
            BudgetId = BudgetId,
            DebtId = DebtId,
            GoalId = GoalId,
            ScheduledTransactionId = ScheduledTransactionId,
            ActionData = ActionData,
            ActionUrl = ActionUrl,
            Icon = Icon,
            Color = Color,
            EnableSound = EnableSound,
            EnableVibration = EnableVibration,
            AdvanceNoticeDays = AdvanceNoticeDays,
            MaxSnoozeCount = MaxSnoozeCount,
            Notes = Notes
        };
    }

    public ValidationResult Validate()
    {
        var result = new ValidationResult();

        if (string.IsNullOrWhiteSpace(Title))
            result.AddError("Notification title is required");

        if (string.IsNullOrWhiteSpace(Message))
            result.AddError("Notification message is required");

        if (ScheduledDate < DateTime.Now.AddMinutes(-5)) // Allow 5 minutes tolerance
            result.AddError("Scheduled date cannot be in the past");

        if (IsRecurring && RecurrenceType == null)
            result.AddError("Recurrence type is required for recurring notifications");

        if (IsRecurring && RecurrenceInterval.HasValue && RecurrenceInterval <= 0)
            result.AddError("Recurrence interval must be greater than zero");

        if (MaxSnoozeCount < 0)
            result.AddError("Max snooze count cannot be negative");

        if (AdvanceNoticeDays < 0)
            result.AddError("Advance notice days cannot be negative");

        return result;
    }
}