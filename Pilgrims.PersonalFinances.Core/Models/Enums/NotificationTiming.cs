namespace Pilgrims.PersonalFinances.Core.Models.Enums
{
    /// <summary>
    /// Defines when notifications should be sent for scheduled transactions
    /// </summary>
    public enum NotificationTiming
    {
        /// <summary>
        /// No notification
        /// </summary>
        None = 0,

        /// <summary>
        /// On the day of the transaction
        /// </summary>
        SameDay = 1,

        /// <summary>
        /// 1 day before the transaction
        /// </summary>
        OneDayBefore = 2,

        /// <summary>
        /// 3 days before the transaction
        /// </summary>
        ThreeDaysBefore = 3,

        /// <summary>
        /// 1 week before the transaction
        /// </summary>
        OneWeekBefore = 4,

        /// <summary>
        /// 2 weeks before the transaction
        /// </summary>
        TwoWeeksBefore = 5,

        /// <summary>
        /// 1 month before the transaction
        /// </summary>
        OneMonthBefore = 6
    }
}
