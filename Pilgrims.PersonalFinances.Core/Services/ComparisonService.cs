using Microsoft.EntityFrameworkCore;
using Pilgrims.PersonalFinances.Data;
using Pilgrims.PersonalFinances.Core.Models.DTOs;
using Pilgrims.PersonalFinances.Core.Models;

namespace Pilgrims.PersonalFinances.Core.Services
{
    public class ComparisonService : IComparisonService
    {
        private readonly PersonalFinanceContext _context;

        public ComparisonService(PersonalFinanceContext context)
        {
            _context = context;
        }

        public async Task<ComparisonResultDto> GetYearOverYearComparisonAsync(string userId, int currentYear, int previousYear)
        {
            var currentYearData = await GetYearlyDataAsync(userId, currentYear);
            var previousYearData = await GetYearlyDataAsync(userId, previousYear);

            return new ComparisonResultDto
            {
                ComparisonType = "Year-over-Year",
                CurrentPeriod = $"{currentYear}",
                PreviousPeriod = $"{previousYear}",
                CurrentData = currentYearData,
                PreviousData = previousYearData,
                Variance = CalculateVariance(currentYearData, previousYearData),
                PercentageChange = CalculatePercentageChange(currentYearData, previousYearData),
                MonthlyBreakdown = await GetMonthlyComparisonAsync(userId, currentYear, previousYear)
            };
        }

        public async Task<ComparisonResultDto> GetMonthOverMonthComparisonAsync(string userId, DateTime currentMonth, DateTime previousMonth)
        {
            var currentMonthData = await GetMonthlyDataAsync(userId, currentMonth);
            var previousMonthData = await GetMonthlyDataAsync(userId, previousMonth);

            return new ComparisonResultDto
            {
                ComparisonType = "Month-over-Month",
                CurrentPeriod = currentMonth.ToString("MMMM yyyy"),
                PreviousPeriod = previousMonth.ToString("MMMM yyyy"),
                CurrentData = currentMonthData,
                PreviousData = previousMonthData,
                Variance = CalculateVariance(currentMonthData, previousMonthData),
                PercentageChange = CalculatePercentageChange(currentMonthData, previousMonthData),
                CategoryBreakdown = await GetCategoryComparisonAsync(userId, currentMonth, previousMonth)
            };
        }

        public async Task<List<TrendDataDto>> GetTrendAnalysisAsync(string userId, DateTime startDate, DateTime endDate, string period = "monthly")
        {
            var trendData = new List<TrendDataDto>();

            if (period.ToLower() == "monthly")
            {
                var current = new DateTime(startDate.Year, startDate.Month, 1);
                var end = new DateTime(endDate.Year, endDate.Month, 1);

                while (current <= end)
                {
                    var monthData = await GetMonthlyDataAsync(userId, current);
                    trendData.Add(new TrendDataDto
                    {
                        Period = current.ToString("MMM yyyy"),
                        Date = current,
                        Income = monthData.TotalIncome,
                        Expenses = monthData.TotalExpenses,
                        NetIncome = monthData.NetIncome,
                        Assets = await GetTotalAssetsAsync(userId, current),
                        Liabilities = await GetTotalLiabilitiesAsync(userId, current)
                    });

                    current = current.AddMonths(1);
                }
            }
            else if (period.ToLower() == "yearly")
            {
                for (int year = startDate.Year; year <= endDate.Year; year++)
                {
                    var yearData = await GetYearlyDataAsync(userId, year);
                    trendData.Add(new TrendDataDto
                    {
                        Period = year.ToString(),
                        Date = new DateTime(year, 1, 1),
                        Income = yearData.TotalIncome,
                        Expenses = yearData.TotalExpenses,
                        NetIncome = yearData.NetIncome,
                        Assets = await GetTotalAssetsAsync(userId, new DateTime(year, 12, 31)),
                        Liabilities = await GetTotalLiabilitiesAsync(userId, new DateTime(year, 12, 31))
                    });
                }
            }

            return trendData;
        }

        public async Task<List<CategoryComparisonDto>> GetCategoryPerformanceAsync(string userId, DateTime startDate, DateTime endDate, string comparisonType = "month-over-month")
        {
            var categories = await _context.Categories
                .Where(c => c.UserId == userId)
                .ToListAsync();

            var comparisons = new List<CategoryComparisonDto>();

            foreach (var category in categories)
            {
                var currentPeriodExpenses = await GetCategoryExpensesAsync(userId, category.Id, startDate, endDate);
                
                DateTime previousStart, previousEnd;
                if (comparisonType == "month-over-month")
                {
                    var monthsDiff = ((endDate.Year - startDate.Year) * 12) + endDate.Month - startDate.Month + 1;
                    previousStart = startDate.AddMonths(-monthsDiff);
                    previousEnd = endDate.AddMonths(-monthsDiff);
                }
                else // year-over-year
                {
                    previousStart = startDate.AddYears(-1);
                    previousEnd = endDate.AddYears(-1);
                }

                var previousPeriodExpenses = await GetCategoryExpensesAsync(userId, category.Id, previousStart, previousEnd);

                var variance = currentPeriodExpenses - previousPeriodExpenses;
                var percentageChange = previousPeriodExpenses != 0 ? (variance / previousPeriodExpenses) * 100 : 0;

                comparisons.Add(new CategoryComparisonDto
                {
                    CategoryId = category.Id,
                    CategoryName = category.Name,
                    CurrentPeriodAmount = currentPeriodExpenses,
                    PreviousPeriodAmount = previousPeriodExpenses,
                    Variance = variance,
                    PercentageChange = percentageChange,
                    Trend = variance > 0 ? "Increasing" : variance < 0 ? "Decreasing" : "Stable"
                });
            }

            return comparisons.OrderByDescending(c => Math.Abs(c.PercentageChange)).ToList();
        }

        private async Task<PeriodDataDto> GetYearlyDataAsync(string userId, int year)
        {
            var startDate = new DateTime(year, 1, 1);
            var endDate = new DateTime(year, 12, 31);

            var income = await _context.Transactions
                .Where(t => t.UserId == userId && t.Date >= startDate && t.Date <= endDate && t.Amount > 0)
                .SumAsync(t => t.Amount);

            var expenses = await _context.Transactions
                .Where(t => t.UserId == userId && t.Date >= startDate && t.Date <= endDate && t.Amount < 0)
                .SumAsync(t => Math.Abs(t.Amount));

            return new PeriodDataDto
            {
                TotalIncome = income,
                TotalExpenses = expenses,
                NetIncome = income - expenses,
                TransactionCount = await _context.Transactions
                    .CountAsync(t => t.UserId == userId && t.Date >= startDate && t.Date <= endDate)
            };
        }

        private async Task<PeriodDataDto> GetMonthlyDataAsync(string userId, DateTime month)
        {
            var startDate = new DateTime(month.Year, month.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var income = await _context.Transactions
                .Where(t => t.UserId == userId && t.Date >= startDate && t.Date <= endDate && t.Amount > 0)
                .SumAsync(t => t.Amount);

            var expenses = await _context.Transactions
                .Where(t => t.UserId == userId && t.Date >= startDate && t.Date <= endDate && t.Amount < 0)
                .SumAsync(t => Math.Abs(t.Amount));

            return new PeriodDataDto
            {
                TotalIncome = income,
                TotalExpenses = expenses,
                NetIncome = income - expenses,
                TransactionCount = await _context.Transactions
                    .CountAsync(t => t.UserId == userId && t.Date >= startDate && t.Date <= endDate)
            };
        }

        private async Task<List<MonthlyComparisonDto>> GetMonthlyComparisonAsync(string userId, int currentYear, int previousYear)
        {
            var comparisons = new List<MonthlyComparisonDto>();

            for (int month = 1; month <= 12; month++)
            {
                var currentMonthData = await GetMonthlyDataAsync(userId, new DateTime(currentYear, month, 1));
                var previousMonthData = await GetMonthlyDataAsync(userId, new DateTime(previousYear, month, 1));

                var variance = currentMonthData.NetIncome - previousMonthData.NetIncome;
                var percentageChange = previousMonthData.NetIncome != 0 ? (variance / previousMonthData.NetIncome) * 100 : 0;

                comparisons.Add(new MonthlyComparisonDto
                {
                    Month = new DateTime(currentYear, month, 1).ToString("MMMM"),
                    CurrentYearAmount = currentMonthData.NetIncome,
                    PreviousYearAmount = previousMonthData.NetIncome,
                    Variance = variance,
                    PercentageChange = percentageChange
                });
            }

            return comparisons;
        }

        private async Task<List<CategoryComparisonDto>> GetCategoryComparisonAsync(string userId, DateTime currentMonth, DateTime previousMonth)
        {
            var categories = await _context.Categories
                .Where(c => c.UserId == userId)
                .ToListAsync();

            var comparisons = new List<CategoryComparisonDto>();

            foreach (var category in categories)
            {
                var currentMonthStart = new DateTime(currentMonth.Year, currentMonth.Month, 1);
                var currentMonthEnd = currentMonthStart.AddMonths(1).AddDays(-1);
                var previousMonthStart = new DateTime(previousMonth.Year, previousMonth.Month, 1);
                var previousMonthEnd = previousMonthStart.AddMonths(1).AddDays(-1);

                var currentAmount = await GetCategoryExpensesAsync(userId, category.Id, currentMonthStart, currentMonthEnd);
                var previousAmount = await GetCategoryExpensesAsync(userId, category.Id, previousMonthStart, previousMonthEnd);

                var variance = currentAmount - previousAmount;
                var percentageChange = previousAmount != 0 ? (variance / previousAmount) * 100 : 0;

                comparisons.Add(new CategoryComparisonDto
                {
                    CategoryId = category.Id,
                    CategoryName = category.Name,
                    CurrentPeriodAmount = currentAmount,
                    PreviousPeriodAmount = previousAmount,
                    Variance = variance,
                    PercentageChange = percentageChange,
                    Trend = variance > 0 ? "Increasing" : variance < 0 ? "Decreasing" : "Stable"
                });
            }

            return comparisons.OrderByDescending(c => Math.Abs(c.PercentageChange)).ToList();
        }

        private async Task<decimal> GetCategoryExpensesAsync(string userId, string categoryId, DateTime startDate, DateTime endDate)
        {
            return await _context.Transactions
                .Where(t => t.UserId == userId && 
                           t.CategoryId == categoryId && 
                           t.Date >= startDate && 
                           t.Date <= endDate && 
                           t.Amount < 0)
                .SumAsync(t => Math.Abs(t.Amount));
        }

        private async Task<decimal> GetTotalAssetsAsync(string userId, DateTime asOfDate)
        {
            // Remove Asset.UserId filtering since Asset model doesn't have UserId property
            return await _context.Assets
                .Where(a => a.CreatedAt <= asOfDate)
                .SumAsync(a => a.CurrentValue ?? 0);
        }

        private async Task<decimal> GetTotalLiabilitiesAsync(string userId, DateTime asOfDate)
        {
            // Remove Debt.UserId filtering since Debt model doesn't have UserId property  
            return await _context.Debts
                .Where(d => d.CreatedAt <= asOfDate)
                .SumAsync(d => d.CurrentBalance);
        }

        private decimal CalculateVariance(PeriodDataDto current, PeriodDataDto previous)
        {
            return current.NetIncome - previous.NetIncome;
        }

        private decimal CalculatePercentageChange(PeriodDataDto current, PeriodDataDto previous)
        {
            return previous.NetIncome != 0 ? ((current.NetIncome - previous.NetIncome) / previous.NetIncome) * 100 : 0;
        }
    }

}
