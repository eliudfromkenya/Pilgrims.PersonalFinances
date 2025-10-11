namespace Pilgrims.PersonalFinances.Models.DTOs
{
    public class CategoryAnalysisDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalSpending { get; set; }
        public List<CategorySummaryDto> Categories { get; set; } = new();
        public List<MonthlyBreakdownDto> MonthlyBreakdown { get; set; } = new();
    }
}