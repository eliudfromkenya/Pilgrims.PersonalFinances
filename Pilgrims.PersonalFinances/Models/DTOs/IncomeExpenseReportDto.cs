using System.ComponentModel.DataAnnotations.Schema;

namespace Pilgrims.PersonalFinances.Models.DTOs
{
    /// <summary>
    /// Data transfer object for income and expense reports
    /// </summary>
    public class IncomeExpenseReportDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalIncome { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalExpenses { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal NetIncome { get; set; }
        
        public List<CategorySummaryDto> IncomeByCategory { get; set; } = new();
        public List<CategorySummaryDto> ExpensesByCategory { get; set; } = new();
        public List<AccountSummaryDto> AccountSummaries { get; set; } = new();
        public List<MonthlyTrendDto> MonthlyTrends { get; set; } = new();
    }

    /// <summary>
    /// Summary data for categories in reports
    /// </summary>
    public class CategorySummaryDto
    {
        public string CategoryId { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        
        public int TransactionCount { get; set; }
        public decimal Percentage { get; set; }
    }

    /// <summary>
    /// Summary data for accounts in reports
    /// </summary>
    public class AccountSummaryDto
    {
        public string? AccountId { get; set; }
        public string AccountName { get; set; } = string.Empty;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal StartingBalance { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal EndingBalance { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalIncome { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalExpenses { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal NetChange { get; set; }
        
        public int TransactionCount { get; set; }
    }

    /// <summary>
    /// Monthly trend data for reports
    /// </summary>
    public class MonthlyTrendDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; } = string.Empty;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        
        public int TransactionCount { get; set; }
    }
}