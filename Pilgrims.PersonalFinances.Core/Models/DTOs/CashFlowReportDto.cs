using System.ComponentModel.DataAnnotations.Schema;

namespace Pilgrims.PersonalFinances.Core.Models.DTOs
{
    /// <summary>
    /// Data transfer object for cash flow reports
    /// </summary>
    public class CashFlowReportDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal StartingCash { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal EndingCash { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal NetCashFlow { get; set; }
        
        // Operating Activities
        [Column(TypeName = "decimal(18,2)")]
        public decimal OperatingCashFlow { get; set; }
        
        public List<CashFlowCategoryDto> OperatingActivities { get; set; } = new();
        
        // Investing Activities
        [Column(TypeName = "decimal(18,2)")]
        public decimal InvestingCashFlow { get; set; }
        
        public List<CashFlowCategoryDto> InvestingActivities { get; set; } = new();
        
        // Financing Activities
        [Column(TypeName = "decimal(18,2)")]
        public decimal FinancingCashFlow { get; set; }
        
        public List<CashFlowCategoryDto> FinancingActivities { get; set; } = new();
        
        // Monthly breakdown
        public List<MonthlyTrendDto> MonthlyTrends { get; set; } = new();
    }

    /// <summary>
    /// Cash flow category breakdown
    /// </summary>
    public class CashFlowCategoryDto
    {
        public string CategoryName { get; set; } = string.Empty;
        public string ActivityType { get; set; } = string.Empty; // Operating, Investing, Financing
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        
        public int TransactionCount { get; set; }
        public decimal Percentage { get; set; }
    }
}
