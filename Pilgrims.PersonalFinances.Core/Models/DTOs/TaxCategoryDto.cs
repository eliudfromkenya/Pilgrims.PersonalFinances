using System.ComponentModel.DataAnnotations.Schema;

namespace Pilgrims.PersonalFinances.Models.DTOs
{
    /// <summary>
    /// Tax category breakdown
    /// </summary>
    public class TaxCategoryDto
    {
        public string CategoryName { get; set; } = string.Empty;
        public string TaxType { get; set; } = string.Empty; // Income, Capital Gains, etc.
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        
        public bool IsTaxable { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<int> TransactionIds { get; set; } = new();
    }
}