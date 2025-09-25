using System;
using System.Collections.Generic;

namespace Pilgrims.PersonalFinances.Models.DTOs
{
    public class DebtStrategy
    {
        public string StrategyName { get; set; } = string.Empty;
        public int TotalMonths { get; set; }
        public decimal TotalInterest { get; set; }
        public decimal TotalPayments { get; set; }
        public List<DebtPaymentPlan> PaymentPlan { get; set; } = new();
    }

    public class DebtPaymentPlan
    {
        public string DebtId { get; set; } = string.Empty;
        public string DebtName { get; set; } = string.Empty;
        public int PayoffOrder { get; set; }
        public decimal MonthlyPayment { get; set; }
        public int MonthsToPayoff { get; set; }
        public decimal TotalInterest { get; set; }
    }
}