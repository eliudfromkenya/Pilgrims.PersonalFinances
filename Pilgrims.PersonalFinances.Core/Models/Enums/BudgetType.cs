namespace Pilgrims.PersonalFinances.Core.Models.Enums
{
    /// <summary>
    /// Defines the type of budget for classification and processing.
    /// </summary>
    public enum BudgetType
    {
        /// <summary>
        /// Budget scoped to a specific category.
        /// </summary>
        Category = 0,
        /// <summary>
        /// Budget scoped to a specific account.
        /// </summary>
        Account = 1,
        /// <summary>
        /// Budget scoped to a specific tag.
        /// </summary>
        Tag = 2,
        /// <summary>
        /// Budget scoped to a specific time period (e.g., monthly, quarterly).
        /// </summary>
        TimePeriod = 3
    }
}