namespace Pilgrims.PersonalFinances.Core.Models
{
    /// <summary>
    /// Represents budget dashboard overview data
    /// </summary>
    public class BudgetDashboardData
    {
        public decimal TotalBudgeted { get; set; }
        public decimal TotalSpent { get; set; }
        public decimal Remaining => TotalBudgeted - TotalSpent;
        public int BudgetsOverLimit { get; set; }
        public int TotalActiveBudgets { get; set; }
        public decimal AverageUtilization { get; set; }

        /// <summary>
        /// Number of active budgets
        /// </summary>
        public int ActiveBudgetsCount => TotalActiveBudgets;

        /// <summary>
        /// Total remaining amount across all budgets
        /// </summary>
        public decimal TotalRemaining => Remaining;

        /// <summary>
        /// Monthly trend percentage (positive for increase, negative for decrease)
        /// </summary>
        public decimal MonthlyTrend { get; set; }
    }
}
