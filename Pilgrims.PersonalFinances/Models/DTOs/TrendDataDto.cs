namespace Pilgrims.PersonalFinances.Models.DTOs
{
    public class TrendDataDto
    {
        public string Period { get; set; } = "";
        public DateTime Date { get; set; }
        public decimal Income { get; set; }
        public decimal Expenses { get; set; }
        public decimal NetIncome { get; set; }
        public decimal Assets { get; set; }
        public decimal Liabilities { get; set; }
    }
}