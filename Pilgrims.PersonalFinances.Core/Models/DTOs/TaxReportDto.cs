//using System.ComponentModel.DataAnnotations.Schema;

//namespace Pilgrims.PersonalFinances.Core.Models.DTOs
//{
//    /// <summary>
//    /// Data transfer object for tax reports
//    /// </summary>
//    public class TaxReportDto
//    {
//        public int TaxYear { get; set; }
//        public DateTime GeneratedDate { get; set; }
        
//        [Column(TypeName = "decimal(18,2)")]
//        public decimal TotalIncome { get; set; }
        
//        [Column(TypeName = "decimal(18,2)")]
//        public decimal TotalDeductions { get; set; }
        
//        [Column(TypeName = "decimal(18,2)")]
//        public decimal TaxableIncome { get; set; }
        
//        [Column(TypeName = "decimal(18,2)")]
//        public decimal EstimatedTax { get; set; }
        
//        public List<TaxCategoryDto> IncomeCategories { get; set; } = new();
//        public List<DeductionDto> Deductions { get; set; } = new();
//        public List<TaxDocumentDto> SupportingDocuments { get; set; } = new();
//    }

//    /// <summary>
//    /// Tax category breakdown
//    /// </summary>
//    public class TaxCategoryDto
//    {
//        public string CategoryName { get; set; } = string.Empty;
//        public string TaxType { get; set; } = string.Empty; // Income, Capital Gains, etc.
        
//        [Column(TypeName = "decimal(18,2)")]
//        public decimal Amount { get; set; }
        
//        public bool IsTaxable { get; set; }
//        public string Description { get; set; } = string.Empty;
//        public List<int> TransactionIds { get; set; } = new();
//    }

//    /// <summary>
//    /// Tax deduction information
//    /// </summary>
//    public class DeductionDto
//    {
//        public string DeductionName { get; set; } = string.Empty;
//        public string DeductionType { get; set; } = string.Empty;
        
//        [Column(TypeName = "decimal(18,2)")]
//        public decimal Amount { get; set; }
        
//        public string Description { get; set; } = string.Empty;
//        public bool IsItemized { get; set; }
//        public List<int> TransactionIds { get; set; } = new();
//        public List<string> SupportingDocuments { get; set; } = new();
//    }

//    /// <summary>
//    /// Tax supporting document reference
//    /// </summary>
//    public class TaxDocumentDto
//    {
//        public string DocumentName { get; set; } = string.Empty;
//        public string DocumentType { get; set; } = string.Empty; // W-2, 1099, Receipt, etc.
//        public string FilePath { get; set; } = string.Empty;
//        public DateTime DocumentDate { get; set; }
        
//        [Column(TypeName = "decimal(18,2)")]
//        public decimal? Amount { get; set; }
        
//        public string Description { get; set; } = string.Empty;
//        public List<int> RelatedTransactionIds { get; set; } = new();
//    }
//}
