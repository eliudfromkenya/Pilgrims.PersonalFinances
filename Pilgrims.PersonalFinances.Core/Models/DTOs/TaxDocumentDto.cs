using System.ComponentModel.DataAnnotations.Schema;

namespace Pilgrims.PersonalFinances.Models.DTOs
{
    /// <summary>
    /// Tax supporting document reference
    /// </summary>
    public class TaxDocumentDto
    {
        public string DocumentName { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty; // W-2, 1099, Receipt, etc.
        public string FilePath { get; set; } = string.Empty;
        public DateTime DocumentDate { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Amount { get; set; }
        
        public string Description { get; set; } = string.Empty;
        public List<int> RelatedTransactionIds { get; set; } = new();
    }
}