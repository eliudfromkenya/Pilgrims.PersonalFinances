namespace Pilgrims.PersonalFinances.Models.DTOs
{
    public class MonthlyComparisonDto
    {
        public string Month { get; set; } = "";
        public decimal CurrentYearAmount { get; set; }
        public decimal PreviousYearAmount { get; set; }
        public decimal Variance { get; set; }
        public decimal PercentageChange { get; set; }
    }
}