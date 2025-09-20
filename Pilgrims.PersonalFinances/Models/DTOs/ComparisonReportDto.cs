using System.ComponentModel.DataAnnotations.Schema;

namespace Pilgrims.PersonalFinances.Models.DTOs
{
    /// <summary>
    /// Data transfer object for comparison reports between two periods
    /// </summary>
    public class ComparisonReportDto
    {
        // Period 1 (Base Period)
        public DateTime Period1StartDate { get; set; }
        public DateTime Period1EndDate { get; set; }
        public string Period1Label { get; set; } = string.Empty;
        
        // Period 2 (Comparison Period)
        public DateTime Period2StartDate { get; set; }
        public DateTime Period2EndDate { get; set; }
        public string Period2Label { get; set; } = string.Empty;
        
        // Overall Summary
        [Column(TypeName = "decimal(18,2)")]
        public decimal Period1Income { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Period2Income { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal IncomeChange { get; set; }
        
        public decimal IncomeChangePercentage { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Period1Expenses { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Period2Expenses { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal ExpenseChange { get; set; }
        
        public decimal ExpenseChangePercentage { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Period1NetIncome { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Period2NetIncome { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal NetIncomeChange { get; set; }
        
        public decimal NetIncomeChangePercentage { get; set; }
        
        // Detailed Breakdowns
        public List<CategoryComparisonDto> CategoryComparisons { get; set; } = new();
        public List<AccountComparisonDto> AccountComparisons { get; set; } = new();
    }

    /// <summary>
    /// Category comparison between two periods
    /// </summary>
    public class CategoryComparisonDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string TransactionType { get; set; } = string.Empty;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Period1Amount { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Period2Amount { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Change { get; set; }
        
        public decimal ChangePercentage { get; set; }
        
        public int Period1TransactionCount { get; set; }
        public int Period2TransactionCount { get; set; }
        public int TransactionCountChange { get; set; }
        
        public string TrendDirection { get; set; } = string.Empty; // Increasing, Decreasing, Stable
        public string ChangeSignificance { get; set; } = string.Empty; // Significant, Moderate, Minor
    }

    /// <summary>
    /// Account comparison between two periods
    /// </summary>
    public class AccountComparisonDto
    {
        public int AccountId { get; set; }
        public string AccountName { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Period1StartingBalance { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Period1EndingBalance { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Period1NetChange { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Period2StartingBalance { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Period2EndingBalance { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Period2NetChange { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal NetChangeComparison { get; set; }
        
        public decimal NetChangePercentage { get; set; }
        
        public int Period1TransactionCount { get; set; }
        public int Period2TransactionCount { get; set; }
        public int ActivityChange { get; set; }
    }
}