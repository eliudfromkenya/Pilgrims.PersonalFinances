using System;

namespace Pilgrims.PersonalFinances.Core.Models.DTOs
{
    /// <summary>
    /// Represents a single amortization payment entry in a schedule.
    /// </summary>
    public class AmortizationPayment
    {
        /// <summary>
        /// Sequential payment number within the schedule.
        /// </summary>
        public int PaymentNumber { get; set; }

        /// <summary>
        /// The date when the payment is scheduled.
        /// </summary>
        public DateTime PaymentDate { get; set; }

        /// <summary>
        /// The total amount paid for this installment.
        /// </summary>
        public decimal PaymentAmount { get; set; }

        /// <summary>
        /// The portion of the payment that goes towards principal.
        /// </summary>
        public decimal PrincipalAmount { get; set; }

        /// <summary>
        /// The portion of the payment that goes towards interest.
        /// </summary>
        public decimal InterestAmount { get; set; }

        /// <summary>
        /// The remaining balance after this payment is applied.
        /// </summary>
        public decimal RemainingBalance { get; set; }
    }
}