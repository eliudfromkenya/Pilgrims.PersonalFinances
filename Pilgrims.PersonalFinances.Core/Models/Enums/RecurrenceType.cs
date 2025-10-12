namespace Pilgrims.PersonalFinances.Core.Models.Enums
{
    /// <summary>
    /// Defines the types of recurrence patterns for scheduled transactions
    /// </summary>
    public enum RecurrenceType
    {
        /// <summary>
        /// No recurrence - one-time scheduled transaction
        /// </summary>
        None = 0,

        /// <summary>
        /// Daily recurrence (every N days)
        /// </summary>
        Daily = 1,

        /// <summary>
        /// Weekly recurrence (specific days of the week)
        /// </summary>
        Weekly = 2,

        /// <summary>
        /// Bi-weekly recurrence (every 2 weeks)
        /// </summary>
        BiWeekly = 3,

        /// <summary>
        /// Monthly recurrence (specific date or relative date)
        /// </summary>
        Monthly = 4,

        /// <summary>
        /// Quarterly recurrence (every 3 months)
        /// </summary>
        Quarterly = 5,

        /// <summary>
        /// Semi-annual recurrence (every 6 months)
        /// </summary>
        SemiAnnually = 6,

        /// <summary>
        /// Annual recurrence (yearly)
        /// </summary>
        Annually = 7,

        /// <summary>
        /// Custom recurrence pattern with user-defined intervals
        /// </summary>
        Custom = 8
    }
}
