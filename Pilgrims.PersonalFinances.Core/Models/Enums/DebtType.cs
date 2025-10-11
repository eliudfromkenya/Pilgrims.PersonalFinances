namespace Pilgrims.PersonalFinances.Models.Enums
{
    /// <summary>
    /// Types of debt obligations
    /// </summary>
    public enum DebtType
    {
        /// <summary>
        /// Credit card debt
        /// </summary>
        CreditCard = 1,

        /// <summary>
        /// Personal loan
        /// </summary>
        PersonalLoan = 2,

        /// <summary>
        /// Mortgage loan
        /// </summary>
        Mortgage = 3,

        /// <summary>
        /// Auto loan
        /// </summary>
        AutoLoan = 4,

        /// <summary>
        /// Student loan
        /// </summary>
        StudentLoan = 5,

        /// <summary>
        /// Business loan
        /// </summary>
        BusinessLoan = 6,

        /// <summary>
        /// Medical debt
        /// </summary>
        MedicalDebt = 7,

        /// <summary>
        /// Other type of debt
        /// </summary>
        Other = 99
    }
}