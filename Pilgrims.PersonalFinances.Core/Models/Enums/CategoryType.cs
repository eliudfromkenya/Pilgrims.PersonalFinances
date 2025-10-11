using System.ComponentModel;

namespace Pilgrims.PersonalFinances.Models.Enums
{
    /// <summary>
    /// Defines the type of category for financial transactions
    /// </summary>
    public enum CategoryType
    {
        /// <summary>
        /// Income category - for money coming in
        /// </summary>
        [Description("Income")]
        Income = 1,

        /// <summary>
        /// Expense category - for money going out
        /// </summary>
        [Description("Expense")]
        Expense = 2,

        /// <summary>
        /// Transfer category - for money moving between accounts
        /// </summary>
        [Description("Transfer")]
        Transfer = 3,

        /// <summary>
        /// Mixed category - can be used for both income and expenses
        /// </summary>
        [Description("Mixed")]
        Mixed = 4
    }
}