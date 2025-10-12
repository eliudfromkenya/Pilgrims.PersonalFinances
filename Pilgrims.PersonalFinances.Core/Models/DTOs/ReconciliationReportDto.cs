using Pilgrims.PersonalFinances.Core.Models;

namespace Pilgrims.PersonalFinances.Core.Models.DTOs
{
    public class ReconciliationReportDto
    {
        public ReconciliationSession Session { get; set; } = null!;
        public IEnumerable<ReconciliationItem> ClearedItems { get; set; } = new List<ReconciliationItem>();
        public IEnumerable<ReconciliationItem> UnmatchedItems { get; set; } = new List<ReconciliationItem>();
        public IEnumerable<ReconciliationItem> DisputedItems { get; set; } = new List<ReconciliationItem>();
        public decimal TotalClearedAmount { get; set; }
        public decimal TotalUnmatchedAmount { get; set; }
        public decimal TotalDisputedAmount { get; set; }
        public int ClearedItemsCount { get; set; }
        public int UnmatchedItemsCount { get; set; }
        public int DisputedItemsCount { get; set; }
    }
}
