using System;
using System.Collections.Generic;
using Pilgrims.PersonalFinances.Core.Models.Enums;
using Pilgrims.PersonalFinances.Core.Models;

namespace Pilgrims.PersonalFinances.Core.Models.DTOs
{
    public class DebtStrategy
    {
        public string StrategyName { get; set; } = string.Empty;
        public int TotalMonths { get; set; }
        public decimal TotalInterest { get; set; }
        public decimal TotalPayments { get; set; }
        public List<DebtPaymentPlan> PaymentPlan { get; set; } = new();
        
        // Additional properties used in ObligationService
        public DebtPriority Priority { get; set; }
        public decimal ExtraPayment { get; set; }
        public List<Obligation> Obligations { get; set; } = new();
        public int TotalMonthsToPayoff { get; set; }
        public decimal TotalInterestSaved { get; set; }
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
