using System.ComponentModel.DataAnnotations;

namespace Pilgrims.PersonalFinances.Core.Models.Enums
{
    /// <summary>
    /// Types of financial reports available in the system
    /// </summary>
    public enum ReportType
    {
        [Display(Name = "Income Statement")]
        IncomeStatement,
        
        [Display(Name = "Balance Sheet")]
        BalanceSheet,
        
        [Display(Name = "Cash Flow Statement")]
        CashFlowStatement,
        
        [Display(Name = "Budget Variance Report")]
        BudgetVarianceReport,
        
        [Display(Name = "Net Worth Trend")]
        NetWorthTrend,
        
        [Display(Name = "Category Analysis")]
        CategoryAnalysis,
        
        [Display(Name = "Account Summary")]
        AccountSummary,
        
        [Display(Name = "Transaction Summary")]
        TransactionSummary,
        
        [Display(Name = "Debt Analysis")]
        DebtAnalysis,
        
        [Display(Name = "Investment Performance")]
        InvestmentPerformance,
        
        [Display(Name = "Custom Report")]
        CustomReport
    }
}
