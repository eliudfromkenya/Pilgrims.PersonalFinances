using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pilgrims.PersonalFinances.Core.Models
{
    public class AssetRegister : BaseEntity
    {
        [Required]
        public string AssetId { get; set; } = string.Empty;

        [ForeignKey("AssetId")]
        public virtual Asset Asset { get; set; }

        // Purchase Information
        public string? PurchaseReceiptPath { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public decimal? PurchasePrice { get; set; }
        public string? Vendor { get; set; }
        public string? PurchaseOrderNumber { get; set; }

        // Warranty Information
        public string? WarrantyDocumentPath { get; set; }
        public DateTime? WarrantyStartDate { get; set; }
        public DateTime? WarrantyEndDate { get; set; }
        public string? WarrantyProvider { get; set; }
        public string? WarrantyTerms { get; set; }

        // Insurance Information
        public string? InsurancePolicyPath { get; set; }
        public string? InsurancePolicyNumber { get; set; }
        public string? InsuranceProvider { get; set; }
        public DateTime? InsuranceStartDate { get; set; }
        public DateTime? InsuranceEndDate { get; set; }
        public decimal? InsuranceCoverage { get; set; }
        public decimal? InsurancePremium { get; set; }

        // Maintenance Information
        public DateTime? LastMaintenanceDate { get; set; }
        public DateTime? NextMaintenanceDate { get; set; }
        public string? MaintenanceSchedule { get; set; }
        public string? MaintenanceNotes { get; set; }

        // Additional Documents
        public string? ManualPath { get; set; }
        public string? CertificatesPath { get; set; }
        public string? AdditionalDocumentsPath { get; set; }

        // Tracking Information
        public string? SerialNumber { get; set; }
        public string? ModelNumber { get; set; }
        public string? Manufacturer { get; set; }
        public string? Location { get; set; }
        public string? ResponsiblePerson { get; set; }

        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public string? Notes { get; set; }

        // Navigation Properties
        public virtual ICollection<AssetDocument> Documents { get; set; } = new List<AssetDocument>();
        public virtual ICollection<MaintenanceRecord> MaintenanceRecords { get; set; } = new List<MaintenanceRecord>();
        
        // Alias Properties
        public virtual ICollection<AssetDocument> AssetDocuments { get { return Documents; } }
        
        public string? Status { get; internal set; }
    }
}
