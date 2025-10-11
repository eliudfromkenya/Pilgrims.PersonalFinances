using System.ComponentModel;

namespace Pilgrims.PersonalFinances.Models.Enums
{
    /// <summary>
    /// Defines the types of financial accounts supported by the application
    /// </summary>
    public enum AccountType
    {
        [Description("Checking Account")]
        Checking = 1,

        [Description("Savings Account")]
        Savings = 2,

        [Description("Cash Account")]
        Cash = 3,

        [Description("Credit Card")]
        CreditCard = 4,

        [Description("Investment Account")]
        Investment = 5,

        [Description("Loan Account")]
        Loan = 6,

        [Description("Credit Account")]
        Credit = 7,

        [Description("Other Account Type")]
        Other = 8
    }
}