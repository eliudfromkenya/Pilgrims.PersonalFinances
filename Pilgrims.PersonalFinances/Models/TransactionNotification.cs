using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pilgrims.PersonalFinances.Models.Enums;

namespace Pilgrims.PersonalFinances.Models
{
    /// <summary>
    /// Represents a notification for scheduled transactions
    /// </summary>
    public class TransactionNotification : BaseEntity
    {
        /// <summary>
        /// Unique identifier for the notification
        /// </summary>
        // Removed duplicate Id property - inherited from BaseEntity

        /// <summary>
        /// Reference to the scheduled transaction
        /// </summary>
        [Required]
        public string ScheduledTransactionId { get; set; } = string.Empty;

        /// <summary>
        /// Notification title
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Notification message
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Type of notification
        /// </summary>
        [Required]
        public AppNotificationType NotificationType { get; set; }

        /// <summary>
        /// When the notification should be sent
        /// </summary>
        [Required]
        public DateTime ScheduledDate { get; set; }

        /// <summary>
        /// When the notification was actually sent
        /// </summary>
        public DateTime? SentDate { get; set; }

        /// <summary>
        /// Whether the notification has been sent
        /// </summary>
        public bool IsSent { get; set; } = false;

        /// <summary>
        /// Whether the notification has been read by the user
        /// </summary>
        public bool IsRead { get; set; } = false;

        /// <summary>
        /// Whether the notification has been dismissed
        /// </summary>
        public bool IsDismissed { get; set; } = false;

        /// <summary>
    /// Priority level of the notification
    /// </summary>
    [Required]
    public Enums.NotificationPriority Priority { get; set; } = Enums.NotificationPriority.Normal;

        /// <summary>
        /// Additional data for the notification (JSON)
        /// </summary>
        [MaxLength(1000)]
        public string? AdditionalData { get; set; }

        /// <summary>
        /// Number of retry attempts for sending
        /// </summary>
        public int RetryCount { get; set; } = 0;

        /// <summary>
        /// Maximum number of retry attempts
        /// </summary>
        public int MaxRetries { get; set; } = 3;

        /// <summary>
        /// Error message if notification failed to send
        /// </summary>
        [MaxLength(500)]
        public string? ErrorMessage { get; set; }

        // Navigation Properties
        /// <summary>
        /// Navigation property to the scheduled transaction
        /// </summary>
        public virtual ScheduledTransaction? ScheduledTransaction { get; set; }

        // Computed Properties
        /// <summary>
        /// Gets whether this notification is overdue
        /// </summary>
        [NotMapped]
        public bool IsOverdue => !IsSent && ScheduledDate < DateTime.Now;

        /// <summary>
        /// Gets whether this notification is due soon (within 1 hour)
        /// </summary>
        [NotMapped]
        public bool IsDueSoon => !IsSent && ScheduledDate <= DateTime.Now.AddHours(1) && ScheduledDate > DateTime.Now;

        /// <summary>
        /// Gets whether this notification can be retried
        /// </summary>
        [NotMapped]
        public bool CanRetry => !IsSent && RetryCount < MaxRetries && !string.IsNullOrEmpty(ErrorMessage);

        /// <summary>
        /// Gets the formatted scheduled date
        /// </summary>
        [NotMapped]
        public string FormattedScheduledDate => ScheduledDate.ToString("MMM dd, yyyy h:mm tt");

        /// <summary>
        /// Gets the formatted sent date
        /// </summary>
        [NotMapped]
        public string FormattedSentDate => SentDate?.ToString("MMM dd, yyyy h:mm tt") ?? "Not sent";

        /// <summary>
        /// Gets the notification status description
        /// </summary>
        [NotMapped]
        public string StatusDescription
        {
            get
            {
                if (IsDismissed) return "Dismissed";
                if (IsSent && IsRead) return "Read";
                if (IsSent) return "Sent";
                if (IsOverdue) return "Overdue";
                if (IsDueSoon) return "Due Soon";
                return "Pending";
            }
        }

        // Methods
        /// <summary>
        /// Validates the notification
        /// </summary>
        /// <returns>List of validation errors</returns>
        public List<string> Validate()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(Title))
                errors.Add("Title is required");

            if (string.IsNullOrWhiteSpace(Message))
                errors.Add("Message is required");

            if (string.IsNullOrWhiteSpace(ScheduledTransactionId))
                errors.Add("Scheduled transaction is required");

            if (ScheduledDate < DateTime.Now.AddMinutes(-5)) // Allow 5 minutes tolerance
                errors.Add("Scheduled date cannot be in the past");

            if (!string.IsNullOrEmpty(Title) && Title.Length > 200)
                errors.Add("Title cannot exceed 200 characters");

            if (!string.IsNullOrEmpty(Message) && Message.Length > 500)
                errors.Add("Message cannot exceed 500 characters");

            return errors;
        }

        /// <summary>
        /// Marks the notification as sent
        /// </summary>
        public void MarkAsSent()
        {
            IsSent = true;
            SentDate = DateTime.Now;
            ErrorMessage = null;
            MarkAsDirty();
        }

        /// <summary>
        /// Marks the notification as failed with an error message
        /// </summary>
        /// <param name="errorMessage">Error message</param>
        public void MarkAsFailed(string errorMessage)
        {
            IsSent = false;
            ErrorMessage = errorMessage;
            RetryCount++;
            MarkAsDirty();
        }

        /// <summary>
        /// Marks the notification as read
        /// </summary>
        public void MarkAsRead()
        {
            IsRead = true;
            MarkAsDirty();
        }

        /// <summary>
        /// Marks the notification as dismissed
        /// </summary>
        public void Dismiss()
        {
            IsDismissed = true;
            IsRead = true;
            MarkAsDirty();
        }

        /// <summary>
        /// Resets the notification for retry
        /// </summary>
        public void ResetForRetry()
        {
            if (CanRetry)
            {
                IsSent = false;
                SentDate = null;
                ErrorMessage = null;
                MarkAsDirty();
            }
        }

        /// <summary>
        /// Reschedules the notification to a new date
        /// </summary>
        /// <param name="newDate">New scheduled date</param>
        public void Reschedule(DateTime newDate)
        {
            if (newDate > DateTime.Now)
            {
                ScheduledDate = newDate;
                IsSent = false;
                SentDate = null;
                ErrorMessage = null;
                RetryCount = 0;
                MarkAsDirty();
            }
        }

        /// <summary>
        /// Creates a reminder notification for a scheduled transaction
        /// </summary>
        /// <param name="scheduledTransaction">Scheduled transaction</param>
        /// <param name="reminderDate">When to send the reminder</param>
        /// <returns>New notification</returns>
        public static TransactionNotification CreateReminder(ScheduledTransaction scheduledTransaction, DateTime reminderDate)
        {
            return new TransactionNotification
            {
                ScheduledTransactionId = scheduledTransaction.Id,
                Title = $"Upcoming Transaction: {scheduledTransaction.Name}",
                Message = $"Your scheduled transaction '{scheduledTransaction.Name}' for {scheduledTransaction.FormattedAmount} is due on {scheduledTransaction.NextDueDate?.ToString("MMM dd, yyyy")}.",
                NotificationType = Enums.AppNotificationType.BillReminder,
                ScheduledDate = reminderDate,
                Priority = NotificationPriority.Normal
            };
        }

        /// <summary>
        /// Creates an overdue notification for a scheduled transaction
        /// </summary>
        /// <param name="scheduledTransaction">Scheduled transaction</param>
        /// <returns>New notification</returns>
        public static TransactionNotification CreateOverdueAlert(ScheduledTransaction scheduledTransaction)
        {
            return new TransactionNotification
            {
                ScheduledTransactionId = scheduledTransaction.Id,
                Title = $"Overdue Transaction: {scheduledTransaction.Name}",
                Message = $"Your scheduled transaction '{scheduledTransaction.Name}' for {scheduledTransaction.FormattedAmount} was due on {scheduledTransaction.NextDueDate?.ToString("MMM dd, yyyy")} and is now overdue.",
                NotificationType = Enums.AppNotificationType.BudgetAlert,
                ScheduledDate = DateTime.Now,
                Priority = NotificationPriority.High
            };
        }

        /// <summary>
        /// Creates an approval request notification for a scheduled transaction
        /// </summary>
        /// <param name="scheduledTransaction">Scheduled transaction</param>
        /// <returns>New notification</returns>
        public static TransactionNotification CreateApprovalRequest(ScheduledTransaction scheduledTransaction)
        {
            return new TransactionNotification
            {
                ScheduledTransactionId = scheduledTransaction.Id,
                Title = $"Approval Required: {scheduledTransaction.Name}",
                Message = $"Your scheduled transaction '{scheduledTransaction.Name}' for {scheduledTransaction.FormattedAmount} is ready to be created. Please review and approve.",
                NotificationType = Enums.AppNotificationType.SystemAlert,
                ScheduledDate = DateTime.Now,
                Priority = NotificationPriority.High
            };
        }

        /// <summary>
        /// Creates a transaction created notification for a scheduled transaction
        /// </summary>
        /// <param name="scheduledTransaction">Scheduled transaction</param>
        /// <param name="createdTransaction">Created transaction</param>
        /// <returns>New notification</returns>
        public static TransactionNotification CreateTransactionCreatedNotification(ScheduledTransaction scheduledTransaction, Transaction createdTransaction)
        {
            return new TransactionNotification
            {
                ScheduledTransactionId = scheduledTransaction.Id,
                Title = $"Transaction Created: {scheduledTransaction.Name}",
                Message = $"Your scheduled transaction '{scheduledTransaction.Name}' for {scheduledTransaction.FormattedAmount} has been automatically created on {createdTransaction.Date:MMM dd, yyyy}.",
                NotificationType = Enums.AppNotificationType.SystemAlert,
                ScheduledDate = DateTime.Now,
                Priority = NotificationPriority.Normal
            };
        }

        /// <summary>
        /// Creates an error notification for a scheduled transaction
        /// </summary>
        /// <param name="scheduledTransaction">Scheduled transaction</param>
        /// <param name="errorMessage">Error message</param>
        /// <returns>New notification</returns>
        public static TransactionNotification CreateErrorNotification(ScheduledTransaction scheduledTransaction, string errorMessage)
        {
            return new TransactionNotification
            {
                ScheduledTransactionId = scheduledTransaction.Id,
                Title = $"Error Processing: {scheduledTransaction.Name}",
                Message = $"An error occurred while processing your scheduled transaction '{scheduledTransaction.Name}': {errorMessage}",
                NotificationType = Enums.AppNotificationType.SystemAlert,
                ScheduledDate = DateTime.Now,
                Priority = NotificationPriority.High,
                ErrorMessage = errorMessage
            };
        }
    }

    /// <summary>
    /// Types of notifications
    /// </summary>
    public enum NotificationType
    {
        /// <summary>
        /// Reminder notification
        /// </summary>
        Reminder = 0,

        /// <summary>
        /// Overdue transaction alert
        /// </summary>
        Overdue = 1,

        /// <summary>
        /// Approval request for manual approval mode
        /// </summary>
        ApprovalRequest = 2,

        /// <summary>
        /// Transaction was automatically created
        /// </summary>
        TransactionCreated = 3,

        /// <summary>
        /// Error occurred during processing
        /// </summary>
        Error = 4,

        /// <summary>
        /// Informational notification
        /// </summary>
        Info = 5,

        /// <summary>
        /// Approval request notification
        /// </summary>
        Approval = 6
    }
}