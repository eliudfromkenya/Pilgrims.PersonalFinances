namespace Pilgrims.PersonalFinances.Core.Models.DTOs
{
    public class AccountSummaryReportDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalBalance { get; set; }
        public List<AccountSummaryDto> Accounts { get; set; } = new();
        public List<MonthlyBreakdownDto> MonthlyBreakdown { get; set; } = new();
        public List<MonthlyBreakdownDto> MonthlyTrends { get; set; } = new();
    }
}
