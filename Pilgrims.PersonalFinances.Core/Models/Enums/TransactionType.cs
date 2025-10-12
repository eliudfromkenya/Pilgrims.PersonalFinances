namespace Pilgrims.PersonalFinances.Core.Models.Enums
{
    /// <summary>
    /// Defines the types of transactions supported by the system
    /// </summary>
    public enum TransactionType
    {
        /// <summary>
        /// Money coming into an account (positive amount)
        /// </summary>
        Income = 1,

        /// <summary>
        /// Money going out of an account (negative amount)
        /// </summary>
        Expense = 2,

        /// <summary>
        /// Money moving between accounts
        /// </summary>
        Transfer = 3,

        /// <summary>
        /// Initial balance when setting up an account
        /// </summary>
        InitialBalance = 4,

        /// <summary>
        /// Balance adjustment or correction
        /// </summary>
        Adjustment = 5
    }

    /// <summary>
    /// Extension methods for TransactionType enum
    /// </summary>
    public static class TransactionTypeExtensions
    {
        /// <summary>
        /// Gets the display name for the transaction type
        /// </summary>
        public static string GetDisplayName(this TransactionType type)
        {
            return type switch
            {
                TransactionType.Income => "Income",
                TransactionType.Expense => "Expense",
                TransactionType.Transfer => "Transfer",
                TransactionType.InitialBalance => "Initial Balance",
                TransactionType.Adjustment => "Adjustment",
                _ => type.ToString()
            };
        }

        /// <summary>
        /// Gets the icon name for the transaction type
        /// </summary>
        public static string GetIconName(this TransactionType type)
        {
            return type switch
            {
                TransactionType.Income => "arrow_downward",
                TransactionType.Expense => "arrow_upward",
                TransactionType.Transfer => "swap_horiz",
                TransactionType.InitialBalance => "account_balance",
                TransactionType.Adjustment => "tune",
                _ => "help"
            };
        }

        /// <summary>
        /// Gets the color for the transaction type
        /// </summary>
        public static string GetColor(this TransactionType type)
        {
            return type switch
            {
                TransactionType.Income => "#4CAF50", // Green
                TransactionType.Expense => "#F44336", // Red
                TransactionType.Transfer => "#2196F3", // Blue
                TransactionType.InitialBalance => "#9C27B0", // Purple
                TransactionType.Adjustment => "#FF9800", // Orange
                _ => "#757575" // Gray
            };
        }

        /// <summary>
        /// Determines if the transaction type affects account balance positively
        /// </summary>
        public static bool IsPositive(this TransactionType type)
        {
            return type == TransactionType.Income || type == TransactionType.InitialBalance;
        }

        /// <summary>
        /// Determines if the transaction type affects account balance negatively
        /// </summary>
        public static bool IsNegative(this TransactionType type)
        {
            return type == TransactionType.Expense;
        }

        /// <summary>
        /// Determines if the transaction type is a transfer between accounts
        /// </summary>
        public static bool IsTransfer(this TransactionType type)
        {
            return type == TransactionType.Transfer;
        }
    }
}
