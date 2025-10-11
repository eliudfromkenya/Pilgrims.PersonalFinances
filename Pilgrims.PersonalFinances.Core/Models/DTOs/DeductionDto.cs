using System.ComponentModel.DataAnnotations.Schema;

namespace Pilgrims.PersonalFinances.Core.Models.DTOs
{
    /// <summary>
    /// Tax deduction information
    /// </summary>
    public class DeductionDto
    {
        public string DeductionName { get; set; } = string.Empty;
        public string DeductionType { get; set; } = string.Empty;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        
        public string Description { get; set; } = string.Empty;
        public bool IsItemized { get; set; }
        public List<int> TransactionIds { get; set; } = new();
        public List<string> SupportingDocuments { get; set; } = new();
    }
}
