using System.ComponentModel.DataAnnotations.Schema;

namespace Pilgrims.PersonalFinances.Models.DTOs
{
    /// <summary>
    /// Data transfer object for budget reports
    /// </summary>
    public class BudgetReportDto
    {
        public int BudgetId { get; set; }
        public string BudgetName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalBudgeted { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalActual { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalVariance { get; set; }
        
        public decimal VariancePercentage { get; set; }
        
        public List<BudgetCategoryPerformanceDto> CategoryPerformance { get; set; } = new();
        public List<MonthlyBudgetPerformanceDto> MonthlyPerformance { get; set; } = new();
    }

    /// <summary>
    /// Budget performance by category
    /// </summary>
    public class BudgetCategoryPerformanceDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal BudgetedAmount { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal ActualAmount { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Variance { get; set; }
        
        public decimal VariancePercentage { get; set; }
        public bool IsOverBudget { get; set; }
        public int TransactionCount { get; set; }
    }

    /// <summary>
    /// Monthly budget performance
    /// </summary>
    public class MonthlyBudgetPerformanceDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; } = string.Empty;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal BudgetedAmount { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal ActualAmount { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Variance { get; set; }
        
        public decimal VariancePercentage { get; set; }
        public bool IsOverBudget { get; set; }
    }

    /// <summary>
    /// Budget variance report
    /// </summary>
    public class BudgetVarianceReportDto
    {
        public int BudgetId { get; set; }
        public string BudgetName { get; set; } = string.Empty;
        public DateTime ReportDate { get; set; }
        
        public List<BudgetVarianceItem> VarianceItems { get; set; } = new();
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalFavorableVariance { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalUnfavorableVariance { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal NetVariance { get; set; }
    }

    /// <summary>
    /// Individual budget variance item
    /// </summary>
    public class BudgetVarianceItem
    {
        public string CategoryName { get; set; } = string.Empty;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal BudgetedAmount { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal ActualAmount { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Variance { get; set; }
        
        public decimal VariancePercentage { get; set; }
        public bool IsFavorable { get; set; }
        public string VarianceReason { get; set; } = string.Empty;
    }
}