namespace Pilgrims.PersonalFinances.Core.Models.Enums
{
    /// <summary>
    /// Represents the severity level of a budget alert
    /// </summary>
    public enum BudgetAlertSeverity
    {
        /// <summary>
        /// Warning level alert (typically at 75% of budget)
        /// </summary>
        Warning = 1,

        /// <summary>
        /// Critical level alert (typically at 90% or 100% of budget)
        /// </summary>
        Critical = 2
    }
}
