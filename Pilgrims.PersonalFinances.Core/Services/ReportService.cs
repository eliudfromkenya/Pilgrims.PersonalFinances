using Microsoft.EntityFrameworkCore;
using Pilgrims.PersonalFinances.Data;
using Pilgrims.PersonalFinances.Models;
using Pilgrims.PersonalFinances.Models.DTOs;
using Pilgrims.PersonalFinances.Models.Enums;
using Pilgrims.PersonalFinances.Services.Interfaces;

namespace Pilgrims.PersonalFinances.Services
{
    /// <summary>
    /// Service implementation for comprehensive financial reporting and analytics
    /// </summary>
    public class ReportService : IReportService
    {
        private readonly PersonalFinanceContext _context;
        private readonly IAccountService _accountService;
        private readonly IBudgetService _budgetService;
        private readonly IDebtService _debtService;
        private readonly IAssetService _assetService;

        public ReportService(
            PersonalFinanceContext context,
            IAccountService accountService,
            IBudgetService budgetService,
            IDebtService debtService,
            IAssetService assetService)
        {
            _context = context;
            _accountService = accountService;
            _budgetService = budgetService;
            _debtService = debtService;
            _assetService = assetService;
        }

        #region Income Statement

        public async Task<IncomeExpenseReportDto> GenerateIncomeStatementAsync(DateTime startDate, DateTime endDate)
        {
            var transactions = await _context.Transactions
                .Include(t => t.Category)
                .Include(t => t.Account)
                .Where(t => t.Date >= startDate && t.Date <= endDate && t.Status == TransactionStatus.Cleared)
                .ToListAsync();

            var incomeTransactions = transactions.Where(t => t.Type == TransactionType.Income).ToList();
            var expenseTransactions = transactions.Where(t => t.Type == TransactionType.Expense).ToList();

            var report = new IncomeExpenseReportDto
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalIncome = incomeTransactions.Sum(t => t.Amount),
                TotalExpenses = expenseTransactions.Sum(t => t.Amount),
                IncomeByCategory = incomeTransactions
                    .GroupBy(t => new { t.CategoryId, t.Category?.Name })
                    .Select(g => new CategorySummaryDto
                    {
                        CategoryId = g.Key.CategoryId ?? "",
                        CategoryName = g.Key.Name ?? "Uncategorized",
                        Amount = g.Sum(t => t.Amount),
                        TransactionCount = g.Count(),
                        Percentage = incomeTransactions.Sum(t => t.Amount) > 0 ? 
                            (g.Sum(t => t.Amount) / incomeTransactions.Sum(t => t.Amount)) * 100 : 0
                    }).ToList(),
                ExpensesByCategory = expenseTransactions
                    .GroupBy(t => new { t.CategoryId, t.Category?.Name })
                    .Select(g => new CategorySummaryDto
                    {
                        CategoryId = g.Key.CategoryId ?? "",
                        CategoryName = g.Key.Name ?? "Uncategorized",
                        Amount = g.Sum(t => t.Amount),
                        TransactionCount = g.Count(),
                        Percentage = expenseTransactions.Sum(t => t.Amount) > 0 ? 
                            (g.Sum(t => t.Amount) / expenseTransactions.Sum(t => t.Amount)) * 100 : 0
                    }).ToList(),
                AccountSummaries = transactions
                    .GroupBy(t => new { t.AccountId, t.Account?.Name })
                    .Select(g => new AccountSummaryDto
                    {
                        AccountId = g.Key.AccountId,
                        AccountName = g.Key.Name ?? "Unknown Account",
                        TotalIncome = g.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount),
                        TotalExpenses = g.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount),
                        NetAmount = g.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount) - 
                                   g.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount),
                        TransactionCount = g.Count()
                    }).ToList()
            };

            report.NetIncome = report.TotalIncome - report.TotalExpenses;

            // Generate monthly trends
            report.MonthlyTrends = await GenerateMonthlyTrendsAsync(startDate, endDate);

            return report;
        }

        #endregion

        #region Balance Sheet

        public async Task<BalanceSheetDto> GenerateBalanceSheetAsync(DateTime asOfDate)
        {
            var accounts = await _context.Accounts
                .Where(a => a.Status == AccountStatus.Active)
                .ToListAsync();

            var assets = await _context.Assets
                .Where(a => a.IsActive)
                .ToListAsync();

            var debts = await _context.Debts
                .Where(d => d.IsActive)
                .ToListAsync();

            var report = new BalanceSheetDto
            {
                AsOfDate = asOfDate
            };

            // Calculate assets
            var assetAccounts = accounts.Where(a => 
                a.AccountType == AccountType.Checking || 
                a.AccountType == AccountType.Savings || 
                a.AccountType == AccountType.Investment).ToList();

            report.CurrentAssets = assetAccounts.Where(a => 
                a.AccountType == AccountType.Checking || 
                a.AccountType == AccountType.Savings).Sum(a => a.CurrentBalance);

            report.FixedAssets = assets.Sum(a => a.CurrentValue ?? 0) + 
                               assetAccounts.Where(a => a.AccountType == AccountType.Investment).Sum(a => a.CurrentBalance);

            report.TotalAssets = report.CurrentAssets + report.FixedAssets;

            // Asset breakdown
            report.AssetBreakdown.AddRange(assetAccounts.Select(a => new AssetLiabilityDto
            {
                Name = a.Name,
                Category = a.AccountType.ToString(),
                Amount = a.CurrentBalance
            }));

            report.AssetBreakdown.AddRange(assets.Select(a => new AssetLiabilityDto
            {
                Name = a.Name,
                Category = a.Category?.Name ?? "Other Assets",
                Amount = a.CurrentValue ?? 0
            }));

            // Calculate liabilities
            var liabilityAccounts = accounts.Where(a => 
                a.AccountType == AccountType.CreditCard || 
                a.AccountType == AccountType.Loan).ToList();

            report.CurrentLiabilities = liabilityAccounts.Where(a => a.AccountType == AccountType.CreditCard).Sum(a => Math.Abs(a.CurrentBalance));
            report.LongTermLiabilities = debts.Sum(d => d.CurrentBalance) + 
                                       liabilityAccounts.Where(a => a.AccountType == AccountType.Loan).Sum(a => Math.Abs(a.CurrentBalance));

            report.TotalLiabilities = report.CurrentLiabilities + report.LongTermLiabilities;

            // Liability breakdown
            report.LiabilityBreakdown.AddRange(liabilityAccounts.Select(a => new AssetLiabilityDto
            {
                Name = a.Name,
                Category = a.AccountType.ToString(),
                Amount = Math.Abs(a.CurrentBalance)
            }));

            report.LiabilityBreakdown.AddRange(debts.Select(d => new AssetLiabilityDto
            {
                Name = d.Name,
                Category = d.DebtType.ToString(),
                Amount = d.CurrentBalance
            }));

            // Calculate net worth
            report.NetWorth = report.TotalAssets - report.TotalLiabilities;

            return report;
        }

        #endregion

        #region Cash Flow Statement

        public async Task<CashFlowReportDto> GenerateCashFlowStatementAsync(DateTime startDate, DateTime endDate)
        {
            var transactions = await _context.Transactions
                .Include(t => t.Category)
                .Include(t => t.Account)
                .Where(t => t.Date >= startDate && t.Date <= endDate && t.Status == TransactionStatus.Cleared)
                .ToListAsync();

            var report = new CashFlowReportDto
            {
                StartDate = startDate,
                EndDate = endDate
            };

            // Calculate starting and ending cash positions
            var cashAccounts = await _context.Accounts
                .Where(a => a.AccountType == AccountType.Checking || a.AccountType == AccountType.Savings)
                .ToListAsync();

            report.EndingCash = cashAccounts.Sum(a => a.CurrentBalance);

            // Operating activities (income and regular expenses)
            var operatingTransactions = transactions.Where(t => 
                t.Type == TransactionType.Income || 
                (t.Type == TransactionType.Expense && !IsInvestingOrFinancingTransaction(t))).ToList();

            report.OperatingCashFlow = operatingTransactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount) -
                                     operatingTransactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);

            report.OperatingActivities = operatingTransactions
                .GroupBy(t => new { t.Category?.Name, t.Type })
                .Select(g => new CashFlowCategoryDto
                {
                    CategoryName = g.Key.Name ?? "Uncategorized",
                    ActivityType = "Operating",
                    Amount = g.Key.Type == TransactionType.Income ? g.Sum(t => t.Amount) : -g.Sum(t => t.Amount)
                }).ToList();

            // Investing activities (asset purchases, investments)
            var investingTransactions = transactions.Where(t => IsInvestingTransaction(t)).ToList();
            report.InvestingCashFlow = -investingTransactions.Sum(t => t.Amount);

            report.InvestingActivities = investingTransactions
                .GroupBy(t => t.Category?.Name)
                .Select(g => new CashFlowCategoryDto
                {
                    CategoryName = g.Key ?? "Investments",
                    ActivityType = "Investing",
                    Amount = -g.Sum(t => t.Amount)
                }).ToList();

            // Financing activities (debt payments, loans)
            var financingTransactions = transactions.Where(t => IsFinancingTransaction(t)).ToList();
            report.FinancingCashFlow = -financingTransactions.Sum(t => t.Amount);

            report.FinancingActivities = financingTransactions
                .GroupBy(t => t.Category?.Name)
                .Select(g => new CashFlowCategoryDto
                {
                    CategoryName = g.Key ?? "Financing",
                    ActivityType = "Financing",
                    Amount = -g.Sum(t => t.Amount)
                }).ToList();

            report.NetCashFlow = report.OperatingCashFlow + report.InvestingCashFlow + report.FinancingCashFlow;
            report.StartingCash = report.EndingCash - report.NetCashFlow;

            // Generate monthly trends
            report.MonthlyTrends = await GenerateMonthlyTrendsAsync(startDate, endDate);

            return report;
        }

        #endregion

        #region Budget Variance Report

        public async Task<BudgetVarianceReportDto> GenerateBudgetVarianceReportAsync(DateTime startDate, DateTime endDate)
        {
            var budgets = await _context.Budgets
                .Include(b => b.Categories)
                .ThenInclude(bc => bc.Category)
                .Where(b => b.IsActive && b.StartDate <= endDate && b.EndDate >= startDate)
                .ToListAsync();

            var transactions = await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.Date >= startDate && t.Date <= endDate && t.Status == TransactionStatus.Cleared)
                .ToListAsync();

            var report = new BudgetVarianceReportDto
            {
                StartDate = startDate,
                EndDate = endDate,
                VarianceItems = new List<BudgetVarianceItem>()
            };

            foreach (var budget in budgets)
            {
                foreach (var budgetCategory in budget.Categories)
                {
                    var actualAmount = transactions
                        .Where(t => t.CategoryId == budgetCategory.CategoryId && t.Type == TransactionType.Expense)
                        .Sum(t => t.Amount);

                    var budgetedAmount = budgetCategory.AllocatedAmount;
                    var variance = budgetedAmount - actualAmount;
                    var variancePercentage = budgetedAmount > 0 ? (variance / budgetedAmount) * 100 : 0;

                    report.VarianceItems.Add(new BudgetVarianceItem
                    {
                        CategoryName = budgetCategory.Category?.Name ?? "Unknown",
                        BudgetedAmount = budgetedAmount,
                        ActualAmount = actualAmount,
                        Variance = variance,
                        VariancePercentage = variancePercentage,
                        IsFavorable = variance >= 0,
                        VarianceReason = variance >= 0 ? "Under Budget" : "Over Budget"
                    });
                }
            }

            report.TotalBudgetedAmount = report.VarianceItems.Sum(c => c.BudgetedAmount);
            report.TotalActualAmount = report.VarianceItems.Sum(c => c.ActualAmount);
            report.TotalVariance = report.VarianceItems.Sum(c => c.Variance);
            report.TotalFavorableVariance = report.VarianceItems.Where(v => v.IsFavorable).Sum(v => v.Variance);
            report.TotalUnfavorableVariance = report.VarianceItems.Where(v => !v.IsFavorable).Sum(v => Math.Abs(v.Variance));
            report.NetVariance = report.TotalFavorableVariance - report.TotalUnfavorableVariance;

            return report;
        }

        #endregion

        #region Net Worth Trend

        public async Task<List<Models.DTOs.NetWorthTrendDto>> GenerateNetWorthTrendAsync(DateTime startDate, DateTime endDate)
        {
            var trends = new List<Models.DTOs.NetWorthTrendDto>();
            var currentDate = new DateTime(startDate.Year, startDate.Month, 1);

            while (currentDate <= endDate)
            {
                var balanceSheet = await GenerateBalanceSheetAsync(currentDate.AddMonths(1).AddDays(-1));
                
                trends.Add(new Models.DTOs.NetWorthTrendDto
                {
                    StartDate = currentDate,
                    EndDate = currentDate.AddMonths(1).AddDays(-1),
                    StartingNetWorth = trends.Count > 0 ? trends.Last().EndingNetWorth : balanceSheet.NetWorth,
                    EndingNetWorth = balanceSheet.NetWorth,
                    Change = balanceSheet.NetWorth - (trends.Count > 0 ? trends.Last().EndingNetWorth : balanceSheet.NetWorth),
                    ChangePercentage = trends.Count > 0 && trends.Last().EndingNetWorth != 0 ? 
                        ((balanceSheet.NetWorth - trends.Last().EndingNetWorth) / trends.Last().EndingNetWorth) * 100 : 0
                });

                currentDate = currentDate.AddMonths(1);
            }

            return trends;
        }

        #endregion

        #region Category Analysis

        public async Task<CategoryAnalysisDto> GenerateCategoryAnalysisAsync(DateTime startDate, DateTime endDate)
        {
            var transactions = await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.Date >= startDate && t.Date <= endDate && t.Status == TransactionStatus.Cleared)
                .ToListAsync();

            var expenseTransactions = transactions.Where(t => t.Type == TransactionType.Expense).ToList();
            var totalExpenses = expenseTransactions.Sum(t => t.Amount);

            var categoryBreakdown = expenseTransactions
                .GroupBy(t => new { t.CategoryId, t.Category?.Name })
                .Select(g => new CategorySummaryDto
                {
                    CategoryId = g.Key.CategoryId ?? "",
                    Name = g.Key.Name ?? "Uncategorized",
                    CategoryName = g.Key.Name ?? "Uncategorized",
                    Amount = g.Sum(t => t.Amount),
                    TransactionCount = g.Count(),
                    Percentage = totalExpenses > 0 ? (g.Sum(t => t.Amount) / totalExpenses) * 100 : 0
                })
                .OrderByDescending(c => c.Amount)
                .ToList();

            // Generate monthly breakdown
            var monthlyBreakdown = new List<MonthlyBreakdownDto>();
            var currentDate = new DateTime(startDate.Year, startDate.Month, 1);
            var endMonth = new DateTime(endDate.Year, endDate.Month, 1);

            while (currentDate <= endMonth)
            {
                var monthStart = currentDate;
                var monthEnd = monthStart.AddMonths(1).AddDays(-1);
                
                var monthTransactions = expenseTransactions
                    .Where(t => t.Date >= monthStart && t.Date <= monthEnd)
                    .ToList();

                var monthCategories = monthTransactions
                    .GroupBy(t => new { t.CategoryId, t.Category?.Name })
                    .Select(g => new CategorySummaryDto
                    {
                        CategoryId = g.Key.CategoryId ?? "",
                        Name = g.Key.Name ?? "Uncategorized",
                        CategoryName = g.Key.Name ?? "Uncategorized",
                        Amount = g.Sum(t => t.Amount),
                        TransactionCount = g.Count()
                    })
                    .ToList();

                monthlyBreakdown.Add(new MonthlyBreakdownDto
                {
                    Month = currentDate,
                    Categories = monthCategories
                });

                currentDate = currentDate.AddMonths(1);
            }

            return new CategoryAnalysisDto
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalSpending = totalExpenses,
                Categories = categoryBreakdown,
                MonthlyBreakdown = monthlyBreakdown
            };
        }

        #endregion

        #region Account Summary

        public async Task<AccountSummaryReportDto> GenerateAccountSummaryReportAsync(DateTime startDate, DateTime endDate)
        {
            var accounts = await _context.Accounts
                .Where(a => a.Status == AccountStatus.Active || a.Status == AccountStatus.Inactive)
                .ToListAsync();

            var transactions = await _context.Transactions
                .Where(t => t.Date >= startDate && t.Date <= endDate && t.Status == TransactionStatus.Cleared)
                .ToListAsync();

            var accountSummaries = new List<AccountSummaryDto>();
            var totalBalance = 0m;

            foreach (var account in accounts)
            {
                var accountTransactions = transactions.Where(t => t.AccountId == account.Id).ToList();
                var monthlyChange = accountTransactions
                    .Where(t => t.Date >= DateTime.Now.AddMonths(-1))
                    .Sum(t => t.Type == TransactionType.Income ? t.Amount : -t.Amount);
                
                var accountSummary = new AccountSummaryDto
                {
                    AccountId = account.Id,
                    AccountName = account.Name,
                    AccountType = account.AccountType.ToString(),
                    Balance = account.CurrentBalance,
                    CurrentBalance = account.CurrentBalance,
                    TotalIncome = accountTransactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount),
                    TotalExpenses = accountTransactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount),
                    NetChange = accountTransactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount) - 
                               accountTransactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount),
                    TransactionCount = accountTransactions.Count,
                    MonthlyChange = monthlyChange,
                    AverageTransactionAmount = accountTransactions.Count > 0 ? accountTransactions.Average(t => t.Amount) : 0
                };
                
                accountSummaries.Add(accountSummary);
                totalBalance += account.CurrentBalance;
            }

            // Generate monthly breakdown
            var monthlyBreakdown = new List<MonthlyBreakdownDto>();
            var currentDate = new DateTime(startDate.Year, startDate.Month, 1);
            var endMonth = new DateTime(endDate.Year, endDate.Month, 1);

            while (currentDate <= endMonth)
            {
                var monthStart = currentDate;
                var monthEnd = monthStart.AddMonths(1).AddDays(-1);
                
                var monthAccounts = new List<AccountSummaryDto>();
                foreach (var account in accounts)
                {
                    var monthTransactions = transactions
                        .Where(t => t.AccountId == account.Id && t.Date >= monthStart && t.Date <= monthEnd)
                        .ToList();

                    monthAccounts.Add(new AccountSummaryDto
                    {
                        AccountId = account.Id,
                        AccountName = account.Name,
                        AccountType = account.AccountType.ToString(),
                        Balance = account.CurrentBalance,
                        CurrentBalance = account.CurrentBalance,
                        TransactionCount = monthTransactions.Count
                    });
                }

                monthlyBreakdown.Add(new MonthlyBreakdownDto
                {
                    Month = currentDate,
                    Accounts = monthAccounts
                });

                currentDate = currentDate.AddMonths(1);
            }

            return new AccountSummaryReportDto
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalBalance = totalBalance,
                Accounts = accountSummaries.OrderByDescending(a => a.Balance).ToList(),
                MonthlyBreakdown = monthlyBreakdown
            };
        }

        #endregion

        #region Helper Methods

        private async Task<List<MonthlyTrendDto>> GenerateMonthlyTrendsAsync(DateTime startDate, DateTime endDate)
        {
            var trends = new List<MonthlyTrendDto>();
            var currentDate = new DateTime(startDate.Year, startDate.Month, 1);

            while (currentDate <= endDate)
            {
                var monthEnd = currentDate.AddMonths(1).AddDays(-1);
                var monthTransactions = await _context.Transactions
                    .Where(t => t.Date >= currentDate && t.Date <= monthEnd && t.Status == TransactionStatus.Cleared)
                    .ToListAsync();

                var income = monthTransactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
                var expenses = monthTransactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);

                trends.Add(new MonthlyTrendDto
                {
                    Year = currentDate.Year,
                    Month = currentDate.Month,
                    MonthName = currentDate.ToString("MMM yyyy"),
                    Date = currentDate,
                    Amount = income - expenses,
                    TransactionCount = monthTransactions.Count
                });

                currentDate = currentDate.AddMonths(1);
            }

            return trends;
        }

        private bool IsInvestingTransaction(Transaction transaction)
        {
            // Logic to determine if transaction is investment-related
            var investingCategories = new[] { "Investment", "Asset Purchase", "Real Estate" };
            return investingCategories.Contains(transaction.Category?.Name) ||
                   transaction.Account?.AccountType == AccountType.Investment;
        }

        private bool IsFinancingTransaction(Transaction transaction)
        {
            // Logic to determine if transaction is financing-related
            var financingCategories = new[] { "Loan Payment", "Debt Payment", "Credit Card Payment" };
            return financingCategories.Contains(transaction.Category?.Name) ||
                   transaction.Account?.AccountType == AccountType.Loan ||
                   transaction.Account?.AccountType == AccountType.CreditCard;
        }

        private bool IsInvestingOrFinancingTransaction(Transaction transaction)
        {
            return IsInvestingTransaction(transaction) || IsFinancingTransaction(transaction);
        }

        #endregion

        #region Comparison Features

        public async Task<ComparisonReportDto> GenerateYearOverYearComparisonAsync(int currentYear, int previousYear)
        {
            var currentYearStart = new DateTime(currentYear, 1, 1);
            var currentYearEnd = new DateTime(currentYear, 12, 31);
            var previousYearStart = new DateTime(previousYear, 1, 1);
            var previousYearEnd = new DateTime(previousYear, 12, 31);

            var currentYearReport = await GenerateIncomeStatementAsync(currentYearStart, currentYearEnd);
            var previousYearReport = await GenerateIncomeStatementAsync(previousYearStart, previousYearEnd);

            return new ComparisonReportDto
            {
                Period1StartDate = currentYearStart,
                Period1EndDate = currentYearEnd,
                Period2StartDate = previousYearStart,
                Period2EndDate = previousYearEnd,
                Period1Income = currentYearReport.TotalIncome,
                Period2Income = previousYearReport.TotalIncome,
                Period1Expenses = currentYearReport.TotalExpenses,
                Period2Expenses = previousYearReport.TotalExpenses,
                IncomeChangePercentage = previousYearReport.TotalIncome > 0 ? 
                    ((currentYearReport.TotalIncome - previousYearReport.TotalIncome) / previousYearReport.TotalIncome) * 100 : 0,
                ExpenseChangePercentage = previousYearReport.TotalExpenses > 0 ? 
                    ((currentYearReport.TotalExpenses - previousYearReport.TotalExpenses) / previousYearReport.TotalExpenses) * 100 : 0
            };
        }

        public async Task<ComparisonReportDto> GenerateMonthOverMonthComparisonAsync(DateTime currentMonth, DateTime previousMonth)
        {
            var currentMonthStart = new DateTime(currentMonth.Year, currentMonth.Month, 1);
            var currentMonthEnd = currentMonthStart.AddMonths(1).AddDays(-1);
            var previousMonthStart = new DateTime(previousMonth.Year, previousMonth.Month, 1);
            var previousMonthEnd = previousMonthStart.AddMonths(1).AddDays(-1);

            var currentMonthReport = await GenerateIncomeStatementAsync(currentMonthStart, currentMonthEnd);
            var previousMonthReport = await GenerateIncomeStatementAsync(previousMonthStart, previousMonthEnd);

            return new ComparisonReportDto
            {
                Period1StartDate = currentMonthStart,
                Period1EndDate = currentMonthEnd,
                Period2StartDate = previousMonthStart,
                Period2EndDate = previousMonthEnd,
                Period1Income = currentMonthReport.TotalIncome,
                Period2Income = previousMonthReport.TotalIncome,
                Period1Expenses = currentMonthReport.TotalExpenses,
                Period2Expenses = previousMonthReport.TotalExpenses,
                IncomeChangePercentage = previousMonthReport.TotalIncome > 0 ? 
                    ((currentMonthReport.TotalIncome - previousMonthReport.TotalIncome) / previousMonthReport.TotalIncome) * 100 : 0,
                ExpenseChangePercentage = previousMonthReport.TotalExpenses > 0 ? 
                    ((currentMonthReport.TotalExpenses - previousMonthReport.TotalExpenses) / previousMonthReport.TotalExpenses) * 100 : 0
            };
        }

        #endregion
    }

    // Additional DTOs for new report types

    public class CategoryAnalysisDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalSpending { get; set; }
        public List<CategorySummaryDto> Categories { get; set; } = new();
        public List<MonthlyBreakdownDto> MonthlyBreakdown { get; set; } = new();
    }

    public class AccountSummaryReportDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalBalance { get; set; }
        public List<AccountSummaryDto> Accounts { get; set; } = new();
        public List<MonthlyBreakdownDto> MonthlyBreakdown { get; set; } = new();
    }

    public class MonthlyBreakdownDto
    {
        public DateTime Month { get; set; }
        public List<CategorySummaryDto> Categories { get; set; } = new();
        public List<AccountSummaryDto> Accounts { get; set; } = new();
    }

    public class CategoryVarianceDto
    {
        public string CategoryId { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public decimal BudgetedAmount { get; set; }
        public decimal ActualAmount { get; set; }
        public decimal Variance { get; set; }
        public decimal VariancePercentage { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}