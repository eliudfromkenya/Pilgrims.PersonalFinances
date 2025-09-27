using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pilgrims.PersonalFinances.Models.Enums;

namespace Pilgrims.PersonalFinances.Models
{
    /// <summary>
    /// Represents notifications related to insurance policies and claims
    /// </summary>
    public class InsuranceNotification : BaseEntity
    {
        /// <summary>
        /// Reference to the insurance policy
        /// </summary>
        [Required]
        public string InsuranceId { get; set; } = string.Empty;
        public virtual Insurance Insurance { get; set; } = null!;

        /// <summary>
        /// Reference to insurance claim (if applicable)
        /// </summary>
        public string? InsuranceClaimId { get; set; }
        public virtual InsuranceClaim? InsuranceClaim { get; set; }

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
        public NotificationType NotificationType { get; set; }

        /// <summary>
        /// Priority level of the notification
        /// </summary>
        [Required]
        public Enums.NotificationPriority Priority { get; set; } = Enums.NotificationPriority.Medium;

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
        /// Creates a premium due notification
        /// </summary>
        public static InsuranceNotification CreatePremiumDueNotification(Insurance insurance, DateTime dueDate, int daysInAdvance = 7)
        {
            var scheduledDate = dueDate.AddDays(-daysInAdvance);
            return new InsuranceNotification
            {
                InsuranceId = insurance.Id,
                Title = $"Insurance Premium Due: {insurance.PolicyNumber}",
                Message = $"Your {insurance.PolicyType} insurance premium of {insurance.PremiumAmount:C} is due on {dueDate:MMM dd, yyyy}. Policy: {insurance.PolicyNumber}",
                NotificationType = (NotificationType)Enums.AppNotificationType.InsurancePremiumDue,
                Priority = NotificationPriority.High,
                ScheduledDate = scheduledDate
            };
        }

        /// <summary>
        /// Creates a policy expiry notification
        /// </summary>
        public static InsuranceNotification CreatePolicyExpiryNotification(Insurance insurance, int daysInAdvance = 30)
        {
            var scheduledDate = insurance.PolicyEndDate?.AddDays(-daysInAdvance) ?? DateTime.Now.AddDays(-daysInAdvance);
            return new InsuranceNotification
            {
                InsuranceId = insurance.Id,
                Title = $"Insurance Policy Expiring: {insurance.PolicyNumber}",
                Message = $"Your {insurance.PolicyType} insurance policy expires on {insurance.PolicyEndDate:MMM dd, yyyy}. Please renew to maintain coverage.",
                NotificationType = (NotificationType)Enums.AppNotificationType.InsurancePolicyExpiry,
                Priority = NotificationPriority.Critical,
                ScheduledDate = scheduledDate
            };
        }

        /// <summary>
        /// Creates a claim update notification
        /// </summary>
        public static InsuranceNotification CreateClaimUpdateNotification(Insurance insurance, InsuranceClaim claim)
        {
            return new InsuranceNotification
            {
                InsuranceId = insurance.Id,
                InsuranceClaimId = claim.Id,
                Title = $"Claim Update: {claim.ClaimNumber}",
                Message = $"Your insurance claim {claim.ClaimNumber} status has been updated to {claim.Status}. Amount: {claim.ClaimAmount:C}",
                NotificationType = (NotificationType)Enums.AppNotificationType.InsuranceClaimUpdate,
                Priority = NotificationPriority.Medium,
                ScheduledDate = DateTime.Now
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