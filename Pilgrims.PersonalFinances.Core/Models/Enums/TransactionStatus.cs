namespace Pilgrims.PersonalFinances.Core.Models.Enums
{
    /// <summary>
    /// Defines the status of a transaction in the processing pipeline
    /// </summary>
    public enum TransactionStatus
    {
        /// <summary>
        /// Transaction is pending processing
        /// </summary>
        Pending = 1,

        /// <summary>
        /// Transaction has been processed successfully
        /// </summary>
        Processed = 2,

        /// <summary>
        /// Transaction failed processing
        /// </summary>
        Failed = 3,

        /// <summary>
        /// Transaction is being reviewed
        /// </summary>
        Review = 4,

        /// <summary>
        /// Transaction has been cancelled
        /// </summary>
        Cancelled = 5,

        /// <summary>
        /// Transaction has been cleared by the bank
        /// </summary>
        Cleared = 6,

        /// <summary>
        /// Transaction has been reconciled
        /// </summary>
        Reconciled = 7
    }

    /// <summary>
    /// Extension methods for TransactionStatus enum
    /// </summary>
    public static class TransactionStatusExtensions
    {
        /// <summary>
        /// Gets the display name for the transaction status
        /// </summary>
        public static string GetDisplayName(this TransactionStatus status)
        {
            return status switch
            {
                TransactionStatus.Pending => "Pending",
                TransactionStatus.Processed => "Processed",
                TransactionStatus.Failed => "Failed",
                TransactionStatus.Review => "Under Review",
                TransactionStatus.Cancelled => "Cancelled",
                TransactionStatus.Cleared => "Cleared",
                _ => status.ToString()
            };
        }

        /// <summary>
        /// Gets the CSS class for styling the status
        /// </summary>
        public static string GetCssClass(this TransactionStatus status)
        {
            return status switch
            {
                TransactionStatus.Pending => "status-pending",
                TransactionStatus.Processed => "status-success",
                TransactionStatus.Failed => "status-error",
                TransactionStatus.Review => "status-warning",
                TransactionStatus.Cancelled => "status-cancelled",
                TransactionStatus.Cleared => "status-cleared",
                _ => "status-default"
            };
        }

        /// <summary>
        /// Checks if the status indicates the transaction is finalized
        /// </summary>
        public static bool IsFinalized(this TransactionStatus status)
        {
            return status is TransactionStatus.Processed or 
                           TransactionStatus.Cancelled or 
                           TransactionStatus.Cleared;
        }

        /// <summary>
        /// Checks if the status indicates the transaction can be modified
        /// </summary>
        public static bool CanBeModified(this TransactionStatus status)
        {
            return status is TransactionStatus.Pending or 
                           TransactionStatus.Review;
        }
    }
}
