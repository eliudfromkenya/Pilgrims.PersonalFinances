namespace Pilgrims.PersonalFinances.Core.Models.Enums
{
    /// <summary>
    /// Alert levels for budget notifications
    /// </summary>
    public enum BudgetAlertLevel
    {
        /// <summary>
        /// Alert at 50% of budget used
        /// </summary>
        FiftyPercent = 50,

        /// <summary>
        /// Alert at 75% of budget used
        /// </summary>
        SeventyFivePercent = 75,

        /// <summary>
        /// Alert at 90% of budget used
        /// </summary>
        NinetyPercent = 90,

        /// <summary>
        /// Alert at 100% of budget used (budget exceeded)
        /// </summary>
        OneHundredPercent = 100
    }
}
