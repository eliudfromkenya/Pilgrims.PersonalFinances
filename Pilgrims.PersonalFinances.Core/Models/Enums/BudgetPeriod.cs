namespace Pilgrims.PersonalFinances.Models.Enums
{
    /// <summary>
    /// Defines the budget period types for categories
    /// </summary>
    public enum BudgetPeriod
    {
        /// <summary>
        /// Weekly budget period
        /// </summary>
        Weekly = 1,

        /// <summary>
        /// Monthly budget period
        /// </summary>
        Monthly = 2,

        /// <summary>
        /// Quarterly budget period (3 months)
        /// </summary>
        Quarterly = 3,

        /// <summary>
        /// Yearly budget period
        /// </summary>
        Yearly = 4,

        /// <summary>
        /// Custom budget period
        /// </summary>
        Custom = 5
    }
}