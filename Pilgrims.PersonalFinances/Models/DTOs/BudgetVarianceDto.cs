using System.ComponentModel.DataAnnotations.Schema;

namespace Pilgrims.PersonalFinances.Models.DTOs
{
    /// <summary>
    /// Data transfer object for budget variance analysis
    /// </summary>
    public class BudgetVarianceDto
    {
        public string BudgetId { get; set; } = string.Empty;
        public string BudgetName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        
        public DateTime AnalysisDate { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal BudgetedAmount { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal ActualAmount { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Variance { get; set; }
        
        public decimal VariancePercentage { get; set; }
        
        public bool IsFavorable { get; set; }
        public string VarianceType { get; set; } = string.Empty; // Favorable, Unfavorable
        public string VarianceCategory { get; set; } = string.Empty; // Minor, Moderate, Significant
        
        public string Reason { get; set; } = string.Empty;
        public string Impact { get; set; } = string.Empty;
        public List<string> RecommendedActions { get; set; } = new();
        
        public int TransactionCount { get; set; }
        public decimal AverageTransactionAmount { get; set; }
    }
}