namespace Pilgrims.PersonalFinances.Models.DTOs
{
    public class CategoryComparisonDto
    {
        public string CategoryId { get; set; } = "";
        public string CategoryName { get; set; } = "";
        public decimal CurrentPeriodAmount { get; set; }
        public decimal PreviousPeriodAmount { get; set; }
        public decimal Variance { get; set; }
        public decimal PercentageChange { get; set; }
        public string Trend { get; set; } = "";
    }
}