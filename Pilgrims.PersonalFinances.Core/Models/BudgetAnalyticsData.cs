namespace Pilgrims.PersonalFinances.Models
{
    /// <summary>
    /// Represents budget analytics overview data
    /// </summary>
    public class BudgetAnalyticsData
    {
        public int TotalBudgets { get; set; }
        public decimal AverageUtilization { get; set; }
        public int OverBudgetCount { get; set; }
        public decimal TotalVariance { get; set; }
    }
}