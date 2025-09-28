using System.ComponentModel.DataAnnotations.Schema;

namespace Pilgrims.PersonalFinances.Models.DTOs
{
    /// <summary>
    /// Data transfer object for budget analysis
    /// </summary>
    public class BudgetAnalysisDto
    {
        public string BudgetId { get; set; } = string.Empty;
        public string BudgetName { get; set; } = string.Empty;
        public string BudgetType { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal BudgetedAmount { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal SpentAmount { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal RemainingAmount { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Variance { get; set; }
        
        public decimal UtilizationPercentage { get; set; }
        public bool IsOverBudget { get; set; }
        public int TransactionCount { get; set; }
        
        public string CategoryName { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string Tags { get; set; } = string.Empty;
    }
}