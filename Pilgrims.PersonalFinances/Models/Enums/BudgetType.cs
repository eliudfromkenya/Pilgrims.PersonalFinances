namespace Pilgrims.PersonalFinances.Models.Enums
{
    /// <summary>
    /// Types of budgets supported by the system
    /// </summary>
    public enum BudgetType
    {
        /// <summary>
        /// Budget based on transaction categories
        /// </summary>
        Category = 0,

        /// <summary>
        /// Budget based on specific accounts
        /// </summary>
        Account = 1,

        /// <summary>
        /// Budget based on transaction tags
        /// </summary>
        Tag = 2,

        /// <summary>
        /// Overall budget for a time period
        /// </summary>
        TimePeriod = 3
    }
}