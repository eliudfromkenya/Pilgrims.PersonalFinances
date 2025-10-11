namespace Pilgrims.PersonalFinances.Models.Enums
{
    /// <summary>
    /// Frequency of debt payments
    /// </summary>
    public enum PaymentFrequency
    {
        /// <summary>
        /// Weekly payments
        /// </summary>
        Weekly = 1,

        /// <summary>
        /// Bi-weekly payments (every 2 weeks)
        /// </summary>
        BiWeekly = 2,

        /// <summary>
        /// Monthly payments
        /// </summary>
        Monthly = 3,

        /// <summary>
        /// Quarterly payments
        /// </summary>
        Quarterly = 4,

        /// <summary>
        /// Semi-annual payments
        /// </summary>
        SemiAnnual = 5,

        /// <summary>
        /// Annual payments
        /// </summary>
        Annual = 6,

        /// <summary>
        /// One-time payment
        /// </summary>
        OneTime = 7
    }
}