namespace Pilgrims.PersonalFinances.Models.DTOs
{
    /// <summary>
    /// Projection data for debt payoff scenarios
    /// </summary>
    public class PayoffProjectionDto
    {
        /// <summary>
        /// Debt ID for this projection
        /// </summary>
        public int DebtId { get; set; }

        /// <summary>
        /// Debt name
        /// </summary>
        public string DebtName { get; set; } = string.Empty;

        /// <summary>
        /// Current balance
        /// </summary>
        public decimal CurrentBalance { get; set; }

        /// <summary>
        /// Monthly payment amount for this scenario
        /// </summary>
        public decimal MonthlyPayment { get; set; }

        /// <summary>
        /// Interest rate used in calculation
        /// </summary>
        public decimal InterestRate { get; set; }

        /// <summary>
        /// Number of months to pay off
        /// </summary>
        public int MonthsToPayoff { get; set; }

        /// <summary>
        /// Total interest that will be paid
        /// </summary>
        public decimal TotalInterest { get; set; }

        /// <summary>
        /// Total amount that will be paid (principal + interest)
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Projected payoff date
        /// </summary>
        public DateTime PayoffDate { get; set; }

        /// <summary>
        /// Monthly payment breakdown
        /// </summary>
        public List<PaymentBreakdownDto> PaymentSchedule { get; set; } = new();
    }

    /// <summary>
    /// Monthly payment breakdown
    /// </summary>
    public class PaymentBreakdownDto
    {
        /// <summary>
        /// Payment number (month)
        /// </summary>
        public int PaymentNumber { get; set; }

        /// <summary>
        /// Payment date
        /// </summary>
        public DateTime PaymentDate { get; set; }

        /// <summary>
        /// Total payment amount
        /// </summary>
        public decimal PaymentAmount { get; set; }

        /// <summary>
        /// Principal portion of payment
        /// </summary>
        public decimal PrincipalAmount { get; set; }

        /// <summary>
        /// Interest portion of payment
        /// </summary>
        public decimal InterestAmount { get; set; }

        /// <summary>
        /// Remaining balance after payment
        /// </summary>
        public decimal RemainingBalance { get; set; }
    }
}