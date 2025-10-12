namespace Pilgrims.PersonalFinances.Core.Models.DTOs
{
    public class ReconciliationSummaryDto
    {
        public string AccountId { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public int TotalSessions { get; set; }
        public int CompletedSessions { get; set; }
        public int PendingSessions { get; set; }
        public decimal TotalReconciled { get; set; }
        public decimal TotalDifferences { get; set; }
        public DateTime? LastReconciliationDate { get; set; }
        public decimal CurrentBookBalance { get; set; }
        public int UnreconciledTransactions { get; set; }
    }
}
