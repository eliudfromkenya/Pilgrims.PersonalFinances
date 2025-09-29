using Pilgrims.PersonalFinances.Models.Enums;

namespace Pilgrims.PersonalFinances.Models.DTOs
{
    public class BankStatementItemDto
    {
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? Reference { get; set; }
        public ReconciliationItemType ItemType { get; set; }
    }
}