using System.ComponentModel.DataAnnotations.Schema;

namespace Pilgrims.PersonalFinances.Models.DTOs
{
    /// <summary>
    /// Data transfer object for tax reports
    /// </summary>
    public class TaxReportDto
    {
        public int TaxYear { get; set; }
        public DateTime GeneratedDate { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalIncome { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalDeductions { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxableIncome { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal EstimatedTax { get; set; }
        
        public List<TaxCategoryDto> IncomeCategories { get; set; } = new();
        public List<DeductionDto> Deductions { get; set; } = new();
        public List<TaxDocumentDto> SupportingDocuments { get; set; } = new();
    }
}