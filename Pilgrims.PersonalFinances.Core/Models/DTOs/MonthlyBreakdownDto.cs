namespace Pilgrims.PersonalFinances.Models.DTOs
{
    public class MonthlyBreakdownDto
    {
        public DateTime Month { get; set; }
        public List<CategorySummaryDto> Categories { get; set; } = new();
        public List<AccountSummaryDto> Accounts { get; set; } = new();
    }
}