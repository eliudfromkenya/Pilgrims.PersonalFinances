namespace Pilgrims.PersonalFinances.Core.Models.Enums
{
    /// <summary>
    /// Defines how scheduled transactions should be processed
    /// </summary>
    public enum SchedulingMode
    {
        /// <summary>
        /// Automatically create transactions on due date
        /// </summary>
        AutoPost = 0,

        /// <summary>
        /// Prompt user for approval before creating transaction
        /// </summary>
        ManualApproval = 1,

        /// <summary>
        /// Create as draft transaction requiring user review
        /// </summary>
        CreateAsDraft = 2
    }
}
