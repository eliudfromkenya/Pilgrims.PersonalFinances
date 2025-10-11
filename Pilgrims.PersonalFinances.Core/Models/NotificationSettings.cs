using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pilgrims.PersonalFinances.Models.Enums;

namespace Pilgrims.PersonalFinances.Models
{
    /// <summary>
    /// User notification preferences and settings
    /// </summary>
    public class NotificationSettings : BaseEntity
    {
        [Required]
        public AppNotificationType NotificationType { get; set; }

        [Required]
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Days in advance to send notification (for bill reminders, debt payments)
        /// </summary>
        public int DaysInAdvance { get; set; } = 3;

        /// <summary>
        /// Hours in advance to send notification
        /// </summary>
        public int HoursInAdvance { get; set; } = 0;

        /// <summary>
        /// Preferred notification channels (can be multiple)
        /// </summary>
        [Required]
        public NotificationChannel PreferredChannels { get; set; } = NotificationChannel.InApp;

        /// <summary>
        /// Time of day to send notifications (24-hour format)
        /// </summary>
        public TimeSpan PreferredTime { get; set; } = new TimeSpan(9, 0, 0); // 9:00 AM

        /// <summary>
        /// Allow snoozing for this notification type
        /// </summary>
        public bool AllowSnooze { get; set; } = true;

        /// <summary>
        /// Default snooze duration in minutes
        /// </summary>
        public int DefaultSnoozeDurationMinutes { get; set; } = 60;

        /// <summary>
        /// Maximum number of snoozes allowed
        /// </summary>
        public int MaxSnoozeCount { get; set; } = 3;

        /// <summary>
        /// For budget alerts - threshold percentage (0-100)
        /// </summary>
        [Column(TypeName = "decimal(5,2)")]
        public decimal? BudgetThresholdPercentage { get; set; } = 80.0m;

        /// <summary>
        /// For budget alerts - threshold type
        /// </summary>
        public BudgetAlertType? BudgetAlertType { get; set; }

        /// <summary>
        /// For budget alerts - fixed amount threshold
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? BudgetThresholdAmount { get; set; }

        /// <summary>
        /// Send notifications on weekends
        /// </summary>
        public bool SendOnWeekends { get; set; } = true;

        /// <summary>
        /// Send notifications on holidays
        /// </summary>
        public bool SendOnHolidays { get; set; } = false;

        /// <summary>
        /// Quiet hours start time
        /// </summary>
        public TimeSpan? QuietHoursStart { get; set; } = new TimeSpan(22, 0, 0); // 10:00 PM

        /// <summary>
        /// Quiet hours end time
        /// </summary>
        public TimeSpan? QuietHoursEnd { get; set; } = new TimeSpan(7, 0, 0); // 7:00 AM

        /// <summary>
        /// Custom message template for this notification type
        /// </summary>
        [StringLength(500)]
        public string? CustomMessageTemplate { get; set; }

        // Computed properties
        [NotMapped]
        public bool IsInQuietHours
        {
            get
            {
                if (!QuietHoursStart.HasValue || !QuietHoursEnd.HasValue)
                    return false;

                var now = DateTime.Now.TimeOfDay;
                var start = QuietHoursStart.Value;
                var end = QuietHoursEnd.Value;

                // Handle overnight quiet hours (e.g., 10 PM to 7 AM)
                if (start > end)
                    return now >= start || now <= end;
                
                return now >= start && now <= end;
            }
        }

        [NotMapped]
        public bool ShouldSendToday
        {
            get
            {
                var today = DateTime.Today.DayOfWeek;
                
                if (!SendOnWeekends && (today == DayOfWeek.Saturday || today == DayOfWeek.Sunday))
                    return false;

                // Additional holiday check would go here if holiday calendar is implemented
                
                return true;
            }
        }
    }
}