using Pilgrims.PersonalFinances.Models.Enums;

namespace Pilgrims.PersonalFinances.Models.DTOs
{
    /// <summary>
    /// Summary information for debt analytics
    /// </summary>
    public class DebtSummaryDto
    {
        /// <summary>
        /// Total number of active debts
        /// </summary>
        public int TotalDebts { get; set; }

        /// <summary>
        /// Total outstanding balance across all debts
        /// </summary>
        public decimal TotalBalance { get; set; }

        /// <summary>
        /// Total original principal amount
        /// </summary>
        public decimal TotalPrincipal { get; set; }

        /// <summary>
        /// Total amount paid across all debts
        /// </summary>
        public decimal TotalPaid { get; set; }

        /// <summary>
        /// Total minimum monthly payments
        /// </summary>
        public decimal TotalMinimumPayments { get; set; }

        /// <summary>
        /// Weighted average interest rate
        /// </summary>
        public decimal AverageInterestRate { get; set; }

        /// <summary>
        /// Debt with highest balance
        /// </summary>
        public string? HighestBalanceDebt { get; set; }

        /// <summary>
        /// Debt with highest interest rate
        /// </summary>
        public string? HighestInterestDebt { get; set; }

        /// <summary>
        /// Number of debts by priority
        /// </summary>
        public Dictionary<DebtPriority, int> DebtsByPriority { get; set; } = new();

        /// <summary>
        /// Number of debts by type
        /// </summary>
        public Dictionary<DebtType, int> DebtsByType { get; set; } = new();

        /// <summary>
        /// Estimated debt-free date based on minimum payments
        /// </summary>
        public DateTime? EstimatedDebtFreeDate { get; set; }
    }
}