namespace Pilgrims.PersonalFinances.Core.Models.Enums
{
    /// <summary>
    /// Status of individual reconciliation items within a reconciliation session
    /// </summary>
    public enum ReconciliationItemStatus
    {
        /// <summary>
        /// Item has not been matched to a corresponding transaction/statement
        /// </summary>
        Unmatched = 1,
        
        /// <summary>
        /// Item has been successfully matched
        /// </summary>
        Matched = 2,
        
        /// <summary>
        /// Item has a dispute and requires investigation
        /// </summary>
        Disputed = 3,
        
        /// <summary>
        /// Dispute has been resolved
        /// </summary>
        Resolved = 4,
        
        /// <summary>
        /// Item is ignored and excluded from reconciliation processing
        /// </summary>
        Ignored = 5
    }
}