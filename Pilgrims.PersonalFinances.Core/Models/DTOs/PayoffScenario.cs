using Pilgrims.PersonalFinances.Models.Enums;

namespace Pilgrims.PersonalFinances.Models.DTOs
{
    public class PayoffScenario
    {
        public string Strategy { get; set; } = "";
        public int PayoffMonths { get; set; }
        public decimal TotalInterest { get; set; }
        public decimal TotalPayments { get; set; }
        public List<DebtPayoffItem> PayoffSchedule { get; set; } = new();
    }

    public class DebtPayoffItem
    {
        public string DebtId { get; set; } = "";
        public string DebtName { get; set; } = "";
        public ObligationType ObligationType { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal InterestRate { get; set; }
        public decimal MonthlyPayment { get; set; }
        public DateTime PayoffDate { get; set; }
        public decimal TotalInterest { get; set; }
    }
}