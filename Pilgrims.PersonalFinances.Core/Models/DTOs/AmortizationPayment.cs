using System;

namespace Pilgrims.PersonalFinances.Models.DTOs
{
    public class AmortizationPayment
    {
        public int PaymentNumber { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal PrincipalAmount { get; set; }
        public decimal InterestAmount { get; set; }
        public decimal RemainingBalance { get; set; }
    }
}