namespace Pilgrims.PersonalFinances.Core.Models
{
    /// <summary>
    /// Data transfer object for budget summary information
    /// </summary>
    public class BudgetSummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public decimal LimitAmount { get; set; }
        public decimal SpentAmount { get; set; }
        public decimal RemainingAmount => LimitAmount - SpentAmount;
        public decimal UtilizationPercentage => LimitAmount > 0 ? (SpentAmount / LimitAmount) * 100 : 0;
        public bool IsOverBudget => SpentAmount > LimitAmount;
        public string BudgetType { get; set; } = "";
        public string Period { get; set; } = "";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        
        // Additional properties for budget analytics
        public int TotalBudgets { get; set; }
        public decimal TotalBudgetAmount { get; set; }
        public decimal TotalSpentAmount { get; set; }
        public int OverBudgetCount { get; set; }
        public decimal AverageUtilization { get; set; }
    }
}
