namespace Pilgrims.PersonalFinances.Models.DTOs
{
    public class ComparisonResultDto
    {
        public string ComparisonType { get; set; } = "";
        public string CurrentPeriod { get; set; } = "";
        public string PreviousPeriod { get; set; } = "";
        public PeriodDataDto CurrentData { get; set; } = new();
        public PeriodDataDto PreviousData { get; set; } = new();
        public decimal Variance { get; set; }
        public decimal PercentageChange { get; set; }
        public List<MonthlyComparisonDto> MonthlyBreakdown { get; set; } = new();
        public List<CategoryComparisonDto> CategoryBreakdown { get; set; } = new();
    }
}