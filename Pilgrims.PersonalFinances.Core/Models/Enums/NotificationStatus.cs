namespace Pilgrims.PersonalFinances.Core.Models.Enums
{
    /// <summary>
    /// Notification status
    /// </summary>
    public enum NotificationStatus
    {
        Pending = 1,
        Sent = 2,
        Read = 3,
        Dismissed = 4,
        Snoozed = 5,
        Failed = 6
    }
}
