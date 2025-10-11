using Microsoft.EntityFrameworkCore;
using Pilgrims.PersonalFinances.Data;
using Pilgrims.PersonalFinances.Models;
using Pilgrims.PersonalFinances.Services.Interfaces;

namespace Pilgrims.PersonalFinances.Services
{
    /// <summary>
    /// Service implementation for comprehensive income management and forecasting
    /// </summary>
    public class IncomeService : IIncomeService
    {
        private readonly PersonalFinanceContext _context;
        private readonly INotificationService _notificationService;

        public IncomeService(PersonalFinanceContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        #region Basic CRUD Operations - Income

        public async Task<IEnumerable<Income>> GetAllIncomesAsync()
        {
            return await _context.Incomes
                .Include(i => i.IncomeCategory)
                .OrderByDescending(i => i.ReceivedDate)
                .ToListAsync();
        }

        public async Task<Income?> GetIncomeByIdAsync(string id)
        {
            return await _context.Incomes
                .Include(i => i.IncomeCategory)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Income> CreateIncomeAsync(Income income)
        {
            if (string.IsNullOrEmpty(income.Id))
                income.Id = Guid.NewGuid().ToString();

            income.CreatedAt = DateTime.UtcNow;
            income.MarkAsDirty();

            // Calculate net amount if not provided
            if (!income.NetAmount.HasValue)
            {
                income.NetAmount = income.CalculatedNetAmount;
            }

            // Set next expected date for recurring income
            if (income.IsRegular && !income.NextExpectedDate.HasValue)
            {
                income.NextExpectedDate = CalculateNextExpectedDate(income.ReceivedDate, income.Frequency);
            }

            _context.Incomes.Add(income);
            await _context.SaveChangesAsync();

            return income;
        }

        public async Task<Income> UpdateIncomeAsync(Income income)
        {
            var existingIncome = await _context.Incomes.FindAsync(income.Id);
            if (existingIncome == null)
                throw new ArgumentException("Income not found");

            // Update properties
            existingIncome.Name = income.Name;
            existingIncome.Description = income.Description;
            existingIncome.Amount = income.Amount;
            existingIncome.IncomeType = income.IncomeType;
            existingIncome.Frequency = income.Frequency;
            existingIncome.IsRegular = income.IsRegular;
            existingIncome.IsPreTax = income.IsPreTax;
            existingIncome.TaxRate = income.TaxRate;
            existingIncome.NetAmount = income.NetAmount ?? income.CalculatedNetAmount;
            existingIncome.ReceivedDate = income.ReceivedDate;
            existingIncome.NextExpectedDate = income.NextExpectedDate;
            existingIncome.StartDate = income.StartDate;
            existingIncome.EndDate = income.EndDate;
            existingIncome.Source = income.Source;
            existingIncome.PaymentMethod = income.PaymentMethod;
            existingIncome.ReferenceNumber = income.ReferenceNumber;
            existingIncome.IsActive = income.IsActive;
            existingIncome.IncomeCategoryId = income.IncomeCategoryId;
            existingIncome.MarkAsDirty();

            await _context.SaveChangesAsync();
            return existingIncome;
        }

        public async Task<bool> DeleteIncomeAsync(string id)
        {
            try
            {
                var income = await _context.Incomes.FindAsync(id);
                if (income == null)
                    return false;

                _context.Incomes.Remove(income);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Log the full exception details including inner exceptions
                var innerException = ex.InnerException;
                var errorMessage = $"Error deleting income with ID {id}: {ex.Message}";
                
                while (innerException != null)
                {
                    errorMessage += $"\nInner Exception: {innerException.Message}";
                    innerException = innerException.InnerException;
                }
                
                // Log to console for debugging (you can replace this with proper logging)
                Console.WriteLine(errorMessage);
                System.Diagnostics.Debug.WriteLine(errorMessage);
                
                // Re-throw the exception to maintain the original behavior
                throw;
            }
        }

        #endregion

        #region Basic CRUD Operations - Income Categories

        public async Task<IEnumerable<IncomeCategory>> GetAllIncomeCategoriesAsync()
        {
            return await _context.IncomeCategories
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<IncomeCategory?> GetIncomeCategoryByIdAsync(string id)
        {
            return await _context.IncomeCategories
                .Include(c => c.Incomes)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IncomeCategory> CreateIncomeCategoryAsync(IncomeCategory category)
        {
            if (string.IsNullOrEmpty(category.Id))
                category.Id = Guid.NewGuid().ToString();

            category.CreatedAt = DateTime.UtcNow;
            category.MarkAsDirty();

            _context.IncomeCategories.Add(category);
            await _context.SaveChangesAsync();

            return category;
        }

        public async Task<IncomeCategory> UpdateIncomeCategoryAsync(IncomeCategory category)
        {
            var existingCategory = await _context.IncomeCategories.FindAsync(category.Id);
            if (existingCategory == null)
                throw new ArgumentException("Income category not found");

            existingCategory.Name = category.Name;
            existingCategory.Description = category.Description;
            existingCategory.Color = category.Color;
            existingCategory.Icon = category.Icon;
            existingCategory.IsActive = category.IsActive;
            existingCategory.MarkAsDirty();

            await _context.SaveChangesAsync();
            return existingCategory;
        }

        public async Task<bool> DeleteIncomeCategoryAsync(string id)
        {
            var category = await _context.IncomeCategories.FindAsync(id);
            if (category == null)
                return false;

            // Check if category has associated incomes
            var hasIncomes = await _context.Incomes.AnyAsync(i => i.IncomeCategoryId == id);
            if (hasIncomes)
                throw new InvalidOperationException("Cannot delete category with associated incomes");

            _context.IncomeCategories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }

        #endregion

        #region Income Filtering and Search

        public async Task<IEnumerable<Income>> GetIncomesByTypeAsync(string incomeType)
        {
            return await _context.Incomes
                .Include(i => i.IncomeCategory)
                .Where(i => i.IncomeType == incomeType && i.IsActive)
                .OrderByDescending(i => i.ReceivedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Income>> GetIncomesByFrequencyAsync(string frequency)
        {
            return await _context.Incomes
                .Include(i => i.IncomeCategory)
                .Where(i => i.Frequency == frequency && i.IsActive)
                .OrderByDescending(i => i.ReceivedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Income>> GetRegularIncomesAsync()
        {
            return await _context.Incomes
                .Include(i => i.IncomeCategory)
                .Where(i => i.IsRegular && i.IsActive)
                .OrderByDescending(i => i.ReceivedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Income>> GetVariableIncomesAsync()
        {
            return await _context.Incomes
                .Include(i => i.IncomeCategory)
                .Where(i => !i.IsRegular && i.IsActive)
                .OrderByDescending(i => i.ReceivedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Income>> GetIncomesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Incomes
                .Include(i => i.IncomeCategory)
                .Where(i => i.ReceivedDate >= startDate && i.ReceivedDate <= endDate && i.IsActive)
                .OrderByDescending(i => i.ReceivedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Income>> GetIncomesByCategoryAsync(string categoryId)
        {
            return await _context.Incomes
                .Include(i => i.IncomeCategory)
                .Where(i => i.IncomeCategoryId == categoryId && i.IsActive)
                .OrderByDescending(i => i.ReceivedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Income>> SearchIncomesAsync(string searchTerm)
        {
            return await _context.Incomes
                .Include(i => i.IncomeCategory)
                .Where(i => i.IsActive && (
                    i.Name.Contains(searchTerm) ||
                    i.Description!.Contains(searchTerm) ||
                    i.Source!.Contains(searchTerm) ||
                    i.IncomeCategory!.Name.Contains(searchTerm)
                ))
                .OrderByDescending(i => i.ReceivedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Income>> GetActiveIncomesAsync()
        {
            return await _context.Incomes
                .Include(i => i.IncomeCategory)
                .Where(i => i.IsActive)
                .OrderByDescending(i => i.ReceivedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Income>> GetUpcomingIncomesAsync(int days = 30)
        {
            var futureDate = DateTime.Today.AddDays(days);
            return await _context.Incomes
                .Include(i => i.IncomeCategory)
                .Where(i => i.IsActive && i.IsRegular && 
                           i.NextExpectedDate.HasValue && 
                           i.NextExpectedDate.Value <= futureDate)
                .OrderBy(i => i.NextExpectedDate)
                .ToListAsync();
        }

        #endregion

        #region Income Calculations and Analytics

        public async Task<decimal> GetTotalIncomeAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Incomes.Where(i => i.IsActive);

            if (startDate.HasValue)
                query = query.Where(i => i.ReceivedDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(i => i.ReceivedDate <= endDate.Value);

            return await query.SumAsync(i => i.Amount);
        }

        public async Task<decimal> GetTotalIncomeByTypeAsync(string incomeType, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Incomes.Where(i => i.IsActive && i.IncomeType == incomeType);

            if (startDate.HasValue)
                query = query.Where(i => i.ReceivedDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(i => i.ReceivedDate <= endDate.Value);

            return await query.SumAsync(i => i.Amount);
        }

        public async Task<decimal> GetTotalIncomeByCategoryAsync(string categoryId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Incomes.Where(i => i.IsActive && i.IncomeCategoryId == categoryId);

            if (startDate.HasValue)
                query = query.Where(i => i.ReceivedDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(i => i.ReceivedDate <= endDate.Value);

            return await query.SumAsync(i => i.Amount);
        }

        public async Task<decimal> GetMonthlyIncomeAverageAsync(int months = 12)
        {
            var startDate = DateTime.Today.AddMonths(-months);
            var totalIncome = await GetTotalIncomeAsync(startDate);
            return totalIncome / months;
        }

        public async Task<decimal> GetYearlyIncomeProjectionAsync()
        {
            var monthlyAverage = await GetMonthlyIncomeAverageAsync(12);
            return monthlyAverage * 12;
        }

        public async Task<decimal> GetNetIncomeAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Incomes.Where(i => i.IsActive);

            if (startDate.HasValue)
                query = query.Where(i => i.ReceivedDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(i => i.ReceivedDate <= endDate.Value);

            var incomes = await query.ToListAsync();
            return incomes.Sum(i => i.CalculatedNetAmount);
        }

        public async Task<decimal> GetGrossIncomeAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            return await GetTotalIncomeAsync(startDate, endDate);
        }

        public async Task<int> GetIncomeCountByCategoryAsync(string categoryId)
        {
            return await _context.Incomes
                .Where(i => i.IncomeCategoryId == categoryId && i.IsActive)
                .CountAsync();
        }

        #endregion

        #region Income Forecasting

        public async Task<decimal> ForecastIncomeAsync(DateTime targetDate)
        {
            var regularIncomes = await GetRegularIncomesAsync();
            decimal forecastedAmount = 0;

            foreach (var income in regularIncomes)
            {
                if (income.NextExpectedDate.HasValue && income.NextExpectedDate.Value <= targetDate)
                {
                    var occurrences = CalculateOccurrences(income.NextExpectedDate.Value, targetDate, income.Frequency);
                    forecastedAmount += income.Amount * occurrences;
                }
            }

            return forecastedAmount;
        }

        public async Task<decimal> ForecastMonthlyIncomeAsync(int year, int month)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            return await ForecastIncomeAsync(endDate);
        }

        public async Task<decimal> ForecastYearlyIncomeAsync(int year)
        {
            var endDate = new DateTime(year, 12, 31);
            return await ForecastIncomeAsync(endDate);
        }

        public async Task<IEnumerable<(DateTime Date, decimal Amount)>> GetIncomeProjectionAsync(DateTime startDate, DateTime endDate)
        {
            var projections = new List<(DateTime Date, decimal Amount)>();
            var regularIncomes = await GetRegularIncomesAsync();

            var currentDate = startDate;
            while (currentDate <= endDate)
            {
                decimal dailyIncome = 0;
                foreach (var income in regularIncomes)
                {
                    if (IsIncomeExpectedOnDate(income, currentDate))
                    {
                        dailyIncome += income.Amount;
                    }
                }

                if (dailyIncome > 0)
                {
                    projections.Add((currentDate, dailyIncome));
                }

                currentDate = currentDate.AddDays(1);
            }

            return projections;
        }

        public async Task<IEnumerable<(string Month, decimal Amount)>> GetMonthlyIncomeHistoryAsync(int months = 12)
        {
            var history = new List<(string Month, decimal Amount)>();
            var currentDate = DateTime.Today;

            for (int i = 0; i < months; i++)
            {
                var monthStart = new DateTime(currentDate.Year, currentDate.Month, 1);
                var monthEnd = monthStart.AddMonths(1).AddDays(-1);
                
                var monthlyTotal = await GetTotalIncomeAsync(monthStart, monthEnd);
                history.Add((monthStart.ToString("MMM yyyy"), monthlyTotal));
                
                currentDate = currentDate.AddMonths(-1);
            }

            return history.OrderBy(h => h.Month);
        }

        #endregion

        #region Regular Income Management

        public async Task<IEnumerable<Income>> GetRecurringIncomesAsync()
        {
            return await _context.Incomes
                .Include(i => i.IncomeCategory)
                .Where(i => i.IsRegular && i.IsActive)
                .OrderBy(i => i.NextExpectedDate)
                .ToListAsync();
        }

        public async Task<Income?> GetNextExpectedIncomeAsync(string incomeId)
        {
            return await _context.Incomes
                .Include(i => i.IncomeCategory)
                .Where(i => i.Id == incomeId && i.IsRegular && i.IsActive)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> ProcessRecurringIncomeAsync(string incomeId)
        {
            var income = await _context.Incomes.FindAsync(incomeId);
            if (income == null || !income.IsRegular)
                return false;

            // Create new income entry for the recurring payment
            var newIncome = new Income
            {
                Id = Guid.NewGuid().ToString(),
                Name = income.Name,
                Description = income.Description,
                Amount = income.Amount,
                IncomeType = income.IncomeType,
                Frequency = income.Frequency,
                IsRegular = false, // Mark as processed, not recurring
                IsPreTax = income.IsPreTax,
                TaxRate = income.TaxRate,
                ReceivedDate = income.NextExpectedDate ?? DateTime.Today,
                Source = income.Source,
                PaymentMethod = income.PaymentMethod,
                IncomeCategoryId = income.IncomeCategoryId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Incomes.Add(newIncome);

            // Update next expected date
            income.NextExpectedDate = CalculateNextExpectedDate(
                income.NextExpectedDate ?? DateTime.Today, 
                income.Frequency);
            income.MarkAsDirty();

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Income>> GetOverdueIncomesAsync()
        {
            return await _context.Incomes
                .Include(i => i.IncomeCategory)
                .Where(i => i.IsActive && i.IsRegular && 
                           i.NextExpectedDate.HasValue && 
                           i.NextExpectedDate.Value < DateTime.Today)
                .OrderBy(i => i.NextExpectedDate)
                .ToListAsync();
        }

        public async Task<bool> UpdateNextExpectedDateAsync(string incomeId, DateTime nextDate)
        {
            var income = await _context.Incomes.FindAsync(incomeId);
            if (income == null)
                return false;

            income.NextExpectedDate = nextDate;
            income.MarkAsDirty();
            await _context.SaveChangesAsync();
            return true;
        }

        #endregion

        #region Tax Calculations

        public async Task<decimal> GetTotalTaxableIncomeAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Incomes.Where(i => i.IsActive && i.IsPreTax);

            if (startDate.HasValue)
                query = query.Where(i => i.ReceivedDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(i => i.ReceivedDate <= endDate.Value);

            return await query.SumAsync(i => i.Amount);
        }

        public async Task<decimal> GetTotalTaxPaidAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Incomes.Where(i => i.IsActive && i.IsPreTax && i.TaxRate.HasValue);

            if (startDate.HasValue)
                query = query.Where(i => i.ReceivedDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(i => i.ReceivedDate <= endDate.Value);

            var incomes = await query.ToListAsync();
            return incomes.Sum(i => i.Amount * (i.TaxRate!.Value / 100));
        }

        public async Task<decimal> GetEstimatedTaxLiabilityAsync(decimal taxRate)
        {
            var taxableIncome = await GetTotalTaxableIncomeAsync();
            return taxableIncome * (taxRate / 100);
        }

        public async Task<IEnumerable<Income>> GetPreTaxIncomesAsync()
        {
            return await _context.Incomes
                .Include(i => i.IncomeCategory)
                .Where(i => i.IsActive && i.IsPreTax)
                .OrderByDescending(i => i.ReceivedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Income>> GetPostTaxIncomesAsync()
        {
            return await _context.Incomes
                .Include(i => i.IncomeCategory)
                .Where(i => i.IsActive && !i.IsPreTax)
                .OrderByDescending(i => i.ReceivedDate)
                .ToListAsync();
        }

        #endregion

        #region Income Statistics and Reporting

        public async Task<Dictionary<string, decimal>> GetIncomeByTypeBreakdownAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Incomes.Where(i => i.IsActive);

            if (startDate.HasValue)
                query = query.Where(i => i.ReceivedDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(i => i.ReceivedDate <= endDate.Value);

            return await query
                .GroupBy(i => i.IncomeType)
                .ToDictionaryAsync(g => g.Key, g => g.Sum(i => i.Amount));
        }

        public async Task<Dictionary<string, decimal>> GetIncomeByCategoryBreakdownAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Incomes
                .Include(i => i.IncomeCategory)
                .Where(i => i.IsActive);

            if (startDate.HasValue)
                query = query.Where(i => i.ReceivedDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(i => i.ReceivedDate <= endDate.Value);

            return await query
                .GroupBy(i => i.IncomeCategory!.Name)
                .ToDictionaryAsync(g => g.Key, g => g.Sum(i => i.Amount));
        }

        public async Task<Dictionary<string, decimal>> GetIncomeByFrequencyBreakdownAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Incomes.Where(i => i.IsActive);

            if (startDate.HasValue)
                query = query.Where(i => i.ReceivedDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(i => i.ReceivedDate <= endDate.Value);

            return await query
                .GroupBy(i => i.Frequency)
                .ToDictionaryAsync(g => g.Key, g => g.Sum(i => i.Amount));
        }

        public async Task<(decimal Regular, decimal Variable)> GetRegularVsVariableIncomeAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Incomes.Where(i => i.IsActive);

            if (startDate.HasValue)
                query = query.Where(i => i.ReceivedDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(i => i.ReceivedDate <= endDate.Value);

            var incomes = await query.ToListAsync();
            var regular = incomes.Where(i => i.IsRegular).Sum(i => i.Amount);
            var variable = incomes.Where(i => !i.IsRegular).Sum(i => i.Amount);

            return (regular, variable);
        }

        public async Task<decimal> GetIncomeGrowthRateAsync(int months = 12)
        {
            var currentPeriodStart = DateTime.Today.AddMonths(-months);
            var previousPeriodStart = currentPeriodStart.AddMonths(-months);

            var currentIncome = await GetTotalIncomeAsync(currentPeriodStart);
            var previousIncome = await GetTotalIncomeAsync(previousPeriodStart, currentPeriodStart.AddDays(-1));

            if (previousIncome == 0) return 0;
            return ((currentIncome - previousIncome) / previousIncome) * 100;
        }

        public async Task<decimal> GetAverageIncomePerSourceAsync()
        {
            var sources = await _context.Incomes
                .Where(i => i.IsActive && !string.IsNullOrEmpty(i.Source))
                .GroupBy(i => i.Source)
                .Select(g => g.Sum(i => i.Amount))
                .ToListAsync();

            return sources.Any() ? sources.Average() : 0;
        }

        #endregion

        #region Income Source Management

        public async Task<IEnumerable<string>> GetIncomeSourcesAsync()
        {
            return await _context.Incomes
                .Where(i => i.IsActive && !string.IsNullOrEmpty(i.Source))
                .Select(i => i.Source!)
                .Distinct()
                .OrderBy(s => s)
                .ToListAsync();
        }

        public async Task<decimal> GetIncomeBySourceAsync(string source, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Incomes.Where(i => i.IsActive && i.Source == source);

            if (startDate.HasValue)
                query = query.Where(i => i.ReceivedDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(i => i.ReceivedDate <= endDate.Value);

            return await query.SumAsync(i => i.Amount);
        }

        public async Task<IEnumerable<Income>> GetIncomesBySourceAsync(string source)
        {
            return await _context.Incomes
                .Include(i => i.IncomeCategory)
                .Where(i => i.IsActive && i.Source == source)
                .OrderByDescending(i => i.ReceivedDate)
                .ToListAsync();
        }

        #endregion

        #region Validation and Business Rules

        public async Task<bool> ValidateIncomeNameAsync(string name, string? excludeIncomeId = null)
        {
            var query = _context.Incomes.Where(i => i.Name == name);
            
            if (!string.IsNullOrEmpty(excludeIncomeId))
                query = query.Where(i => i.Id != excludeIncomeId);

            return !await query.AnyAsync();
        }

        public async Task<bool> ValidateIncomeCategoryNameAsync(string name, string? excludeCategoryId = null)
        {
            var query = _context.IncomeCategories.Where(c => c.Name == name);
            
            if (!string.IsNullOrEmpty(excludeCategoryId))
                query = query.Where(c => c.Id != excludeCategoryId);

            return !await query.AnyAsync();
        }

        public async Task<bool> CanDeleteIncomeCategoryAsync(string categoryId)
        {
            return !await _context.Incomes.AnyAsync(i => i.IncomeCategoryId == categoryId);
        }

        public async Task<bool> HasIncomesAsync(string categoryId)
        {
            return await _context.Incomes.AnyAsync(i => i.IncomeCategoryId == categoryId);
        }

        #endregion

        #region Dashboard and Summary Data

        public async Task<(decimal ThisMonth, decimal LastMonth, decimal YearToDate)> GetIncomeSummaryAsync()
        {
            var now = DateTime.Today;
            var thisMonthStart = new DateTime(now.Year, now.Month, 1);
            var lastMonthStart = thisMonthStart.AddMonths(-1);
            var yearStart = new DateTime(now.Year, 1, 1);

            var thisMonth = await GetTotalIncomeAsync(thisMonthStart);
            var lastMonth = await GetTotalIncomeAsync(lastMonthStart, thisMonthStart.AddDays(-1));
            var yearToDate = await GetTotalIncomeAsync(yearStart);

            return (thisMonth, lastMonth, yearToDate);
        }

        public async Task<IEnumerable<Income>> GetRecentIncomesAsync(int count = 10)
        {
            return await _context.Incomes
                .Include(i => i.IncomeCategory)
                .Where(i => i.IsActive)
                .OrderByDescending(i => i.ReceivedDate)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Income>> GetTopIncomeSourcesAsync(int count = 5, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Incomes.Where(i => i.IsActive && !string.IsNullOrEmpty(i.Source));

            if (startDate.HasValue)
                query = query.Where(i => i.ReceivedDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(i => i.ReceivedDate <= endDate.Value);

            return await query
                .GroupBy(i => i.Source)
                .OrderByDescending(g => g.Sum(i => i.Amount))
                .Take(count)
                .SelectMany(g => g.Take(1))
                .Include(i => i.IncomeCategory)
                .ToListAsync();
        }

        public async Task<decimal> GetMonthlyIncomeTargetAsync()
        {
            // This could be configurable in the future
            var yearlyAverage = await GetYearlyIncomeProjectionAsync();
            return yearlyAverage / 12;
        }

        public async Task<bool> IsIncomeTargetMetAsync(int year, int month)
        {
            var target = await GetMonthlyIncomeTargetAsync();
            var actual = await ForecastMonthlyIncomeAsync(year, month);
            return actual >= target;
        }

        #endregion

        #region Private Helper Methods

        private DateTime CalculateNextExpectedDate(DateTime currentDate, string frequency)
        {
            return frequency switch
            {
                "Weekly" => currentDate.AddDays(7),
                "Bi-weekly" => currentDate.AddDays(14),
                "Monthly" => currentDate.AddMonths(1),
                "Quarterly" => currentDate.AddMonths(3),
                "Annually" => currentDate.AddYears(1),
                _ => currentDate.AddMonths(1) // Default to monthly
            };
        }

        private int CalculateOccurrences(DateTime startDate, DateTime endDate, string frequency)
        {
            var occurrences = 0;
            var currentDate = startDate;

            while (currentDate <= endDate)
            {
                occurrences++;
                currentDate = CalculateNextExpectedDate(currentDate, frequency);
            }

            return occurrences;
        }

        private bool IsIncomeExpectedOnDate(Income income, DateTime date)
        {
            if (!income.IsRegular || !income.NextExpectedDate.HasValue)
                return false;

            var nextExpected = income.NextExpectedDate.Value;
            
            // Check if the date matches the expected frequency pattern
            return income.Frequency switch
            {
                "Weekly" => (date - nextExpected).Days % 7 == 0,
                "Bi-weekly" => (date - nextExpected).Days % 14 == 0,
                "Monthly" => date.Day == nextExpected.Day,
                "Quarterly" => date.Day == nextExpected.Day && (date.Month - nextExpected.Month) % 3 == 0,
                "Annually" => date.Day == nextExpected.Day && date.Month == nextExpected.Month,
                _ => false
            };
        }

        #endregion
    }
}