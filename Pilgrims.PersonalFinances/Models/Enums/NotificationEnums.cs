namespace Pilgrims.PersonalFinances.Models.Enums
{
    /// <summary>
    /// Types of notifications in the system
    /// </summary>
    public enum NotificationType
    {
        BillReminder = 1,
        BudgetAlert = 2,
        DebtPayment = 3,
        IncomeExpectation = 4,
        ReconciliationReminder = 5,
        SystemAlert = 6
    }

    /// <summary>
    /// Notification delivery channels
    /// </summary>
    public enum NotificationChannel
    {
        InApp = 1,
        SystemNotification = 2,
        Email = 3,
        SMS = 4
    }

    /// <summary>
    /// Notification priority levels
    /// </summary>
    public enum NotificationPriority
    {
        Low = 1,
        Medium = 2,
        High = 3,
        Critical = 4
    }

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

    /// <summary>
    /// Budget alert threshold types
    /// </summary>
    public enum BudgetAlertType
    {
        Percentage = 1,
        FixedAmount = 2,
        Overspend = 3
    }

    /// <summary>
    /// Frequency for recurring notifications
    /// </summary>
    public enum NotificationFrequency
    {
        Once = 1,
        Daily = 2,
        Weekly = 3,
        Monthly = 4,
        Quarterly = 5,
        Yearly = 6
    }
}