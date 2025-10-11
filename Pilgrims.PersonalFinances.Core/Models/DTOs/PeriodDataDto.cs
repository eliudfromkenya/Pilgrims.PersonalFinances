namespace Pilgrims.PersonalFinances.Core.Models.DTOs
{
    public class PeriodDataDto
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetIncome { get; set; }
        public int TransactionCount { get; set; }
    }
}
