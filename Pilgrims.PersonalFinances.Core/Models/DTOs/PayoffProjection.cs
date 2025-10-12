using System;

namespace Pilgrims.PersonalFinances.Core.Models.DTOs
{
    public class PayoffProjection
    {
        public string DebtId { get; set; } = string.Empty;
        public decimal MonthlyPayment { get; set; }
        public decimal MinimumPayment { get; set; }
        public int MonthsToPayoff { get; set; }
        public DateTime PayoffDate { get; set; }
        public decimal TotalInterest { get; set; }
        public decimal TotalPayments { get; set; }
        public decimal InterestSavings { get; set; }
        public int TimeSavedMonths { get; set; }
    }
}
