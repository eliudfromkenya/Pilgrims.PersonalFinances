using System.ComponentModel.DataAnnotations.Schema;

namespace Pilgrims.PersonalFinances.Models.DTOs
{
    /// <summary>
    /// Data transfer object for budget performance metrics
    /// </summary>
    public class BudgetPerformanceDto
    {
        public string BudgetId { get; set; } = string.Empty;
        public string BudgetName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal BudgetedAmount { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal ActualAmount { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Variance { get; set; }
        
        public decimal VariancePercentage { get; set; }
        public decimal UtilizationPercentage { get; set; }
        
        public bool IsOverBudget { get; set; }
        public bool IsOnTrack { get; set; }
        
        public int TotalTransactions { get; set; }
        public decimal AverageTransactionAmount { get; set; }
        
        public DateTime LastTransactionDate { get; set; }
        public string PerformanceRating { get; set; } = string.Empty; // Excellent, Good, Fair, Poor
        
        public List<string> Recommendations { get; set; } = new();
    }
}