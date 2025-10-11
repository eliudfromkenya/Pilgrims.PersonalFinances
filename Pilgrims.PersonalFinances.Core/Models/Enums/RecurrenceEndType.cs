namespace Pilgrims.PersonalFinances.Models.Enums
{
    /// <summary>
    /// Defines how a recurring transaction schedule should end
    /// </summary>
    public enum RecurrenceEndType
    {
        /// <summary>
        /// Never ends - continues indefinitely
        /// </summary>
        Never = 0,

        /// <summary>
        /// Ends on a specific date
        /// </summary>
        EndDate = 1,
        
        /// <summary>
        /// Alias for EndDate to match expected property name
        /// </summary>
        OnDate = EndDate,

        /// <summary>
        /// Ends after a specific number of occurrences
        /// </summary>
        AfterOccurrences = 2
    }
}