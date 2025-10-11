namespace Pilgrims.PersonalFinances.Core.Models
{
    public class MonthlyBudgetData
    {
        public string Month { get; set; } = "";
        public decimal TotalBudgeted { get; set; }
        public decimal TotalSpent { get; set; }
        public decimal Variance { get; set; }
    }
}
