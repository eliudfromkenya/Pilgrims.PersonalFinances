using Pilgrims.PersonalFinances.Models.DTOs;

namespace Pilgrims.PersonalFinances.Services
{
    public interface IComparisonService
    {
        Task<ComparisonResultDto> GetYearOverYearComparisonAsync(string userId, int currentYear, int previousYear);
    Task<ComparisonResultDto> GetMonthOverMonthComparisonAsync(string userId, DateTime currentMonth, DateTime previousMonth);
    Task<List<TrendDataDto>> GetTrendAnalysisAsync(string userId, DateTime startDate, DateTime endDate, string period = "monthly");
    Task<List<CategoryComparisonDto>> GetCategoryPerformanceAsync(string userId, DateTime startDate, DateTime endDate, string comparisonType = "month-over-month");
    }
}