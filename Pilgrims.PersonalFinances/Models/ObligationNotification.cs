using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pilgrims.PersonalFinances.Models.Enums;

namespace Pilgrims.PersonalFinances.Models
{
    /// <summary>
    /// Represents notifications related to obligations (group welfare, chamas, saccos)
    /// </summary>
    public class ObligationNotification : BaseEntity
    {
        /// <summary>
        /// Reference to the obligation
        /// </summary>
        [Required]
        public string ObligationId { get; set; } = string.Empty;
        public virtual Obligation Obligation { get; set; } = null!;

        /// <summary>
        /// Reference to obligation payment (if applicable)
        /// </summary>
        public string? ObligationPaymentId { get; set; }
        public virtual ObligationPayment? ObligationPayment { get; set; }

        /// <summary>
        /// Reference to obligation benefit (if applicable)
        /// </summary>
        public string? ObligationBenefitId { get; set; }
        public virtual ObligationBenefit? ObligationBenefit { get; set; }

        /// <summary>
        /// Notification title
        /// </summary>
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Notification message
        /// </summary>
        [Required]
        [StringLength(1000)]
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Type of notification
        /// </summary>
        [Required]
        public Enums.NotificationType NotificationType { get; set; }

        /// <summary>
        /// Priority level
        /// </summary>
        [Required]
        public NotificationPriority Priority { get; set; } = NotificationPriority.Medium;

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
        /// Whether the notification has been read
        /// </summary>
        public bool IsRead { get; set; } = false;

        /// <summary>
        /// When the notification was read
        /// </summary>
        public DateTime? ReadDate { get; set; }

        /// <summary>
        /// Whether the notification is dismissed
        /// </summary>
        public bool IsDismissed { get; set; } = false;

        /// <summary>
        /// Additional data in JSON format
        /// </summary>
        [StringLength(2000)]
        public string? AdditionalData { get; set; }

        /// <summary>
        /// Creates a payment due notification
        /// </summary>
        public static ObligationNotification CreatePaymentDueNotification(Obligation obligation, DateTime dueDate, decimal amount, int daysInAdvance = 3)
        {
            var scheduledDate = dueDate.AddDays(-daysInAdvance);
            return new ObligationNotification
            {
                ObligationId = obligation.Id,
                Title = $"{obligation.Type} Payment Due: {obligation.Name}",
                Message = $"Your {obligation.Type.ToString().ToLower()} contribution of {amount:C} to {obligation.Name} is due on {dueDate:MMM dd, yyyy}.",
                NotificationType = Enums.NotificationType.ObligationPaymentDue,
                Priority = NotificationPriority.High,
                ScheduledDate = scheduledDate
            };
        }

        /// <summary>
        /// Creates an arrears alert notification
        /// </summary>
        public static ObligationNotification CreateArrearsNotification(Obligation obligation, decimal arrearsAmount)
        {
            return new ObligationNotification
            {
                ObligationId = obligation.Id,
                Title = $"Arrears Alert: {obligation.Name}",
                Message = $"You have outstanding arrears of {arrearsAmount:C} for {obligation.Name}. Please make a payment to avoid penalties.",
                NotificationType = Enums.NotificationType.ObligationArrears,
                Priority = NotificationPriority.Critical,
                ScheduledDate = DateTime.Now
            };
        }

        /// <summary>
        /// Creates a benefit available notification
        /// </summary>
        public static ObligationNotification CreateBenefitAvailableNotification(Obligation obligation, ObligationBenefit benefit)
        {
            return new ObligationNotification
            {
                ObligationId = obligation.Id,
                ObligationBenefitId = benefit.Id,
                Title = $"Benefit Available: {obligation.Name}",
                Message = $"A new benefit of {benefit.Amount:C} is available from {obligation.Name}. Benefit type: {benefit.BenefitType}",
                NotificationType = Enums.NotificationType.ObligationBenefitAvailable,
                Priority = NotificationPriority.Medium,
                ScheduledDate = DateTime.Now
            };
        }

        /// <summary>
        /// Creates a group contribution reminder
        /// </summary>
        public static ObligationNotification CreateContributionReminderNotification(Obligation obligation, DateTime nextContributionDate, decimal amount)
        {
            var scheduledDate = nextContributionDate.AddDays(-2); // 2 days before
            return new ObligationNotification
            {
                ObligationId = obligation.Id,
                Title = $"Contribution Reminder: {obligation.Name}",
                Message = $"Don't forget your upcoming {obligation.Type.ToString().ToLower()} contribution of {amount:C} to {obligation.Name} on {nextContributionDate:MMM dd, yyyy}.",
                NotificationType = Enums.NotificationType.GroupContributionReminder,
                Priority = NotificationPriority.Medium,
                ScheduledDate = scheduledDate
            };
        }

        /// <summary>
        /// Marks the notification as read
        /// </summary>
        public void MarkAsRead()
        {
            IsRead = true;
            ReadDate = DateTime.Now;
        }

        /// <summary>
        /// Dismisses the notification
        /// </summary>
        public void Dismiss()
        {
            IsDismissed = true;
        }

        /// <summary>
        /// Marks the notification as sent
        /// </summary>
        public void MarkAsSent()
        {
            IsSent = true;
            SentDate = DateTime.Now;
        }
    }
}