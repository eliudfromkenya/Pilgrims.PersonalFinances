using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pilgrims.PersonalFinances.Models
{
    public class AssetMaintenance : BaseEntity
    {
        // Asset relationship
        [Required]
        public string AssetId { get; set; } = string.Empty;
        public virtual Asset Asset { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        public string ServiceType { get; set; } = string.Empty; // Oil change, repair, inspection, etc.

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public DateTime ServiceDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Cost { get; set; }

        [MaxLength(200)]
        public string? ServiceProvider { get; set; } // Mechanic, service center, etc.

        [MaxLength(100)]
        public string? ServiceProviderContact { get; set; }

        // Mileage or usage at time of service (for vehicles/equipment)
        public int? MileageAtService { get; set; }
        public int? HoursAtService { get; set; }

        // Next service information
        public DateTime? NextServiceDue { get; set; }
        public int? NextServiceMileage { get; set; }
        public int? NextServiceHours { get; set; }

        // Service category
        [MaxLength(50)]
        public string ServiceCategory { get; set; } = "Maintenance"; // Maintenance, Repair, Inspection, Upgrade

        // Priority level
        [MaxLength(20)]
        public string Priority { get; set; } = "Normal"; // Low, Normal, High, Critical

        // Status
        [MaxLength(20)]
        public string Status { get; set; } = "Completed"; // Scheduled, In Progress, Completed, Cancelled

        // Warranty information
        public DateTime? WarrantyExpiryDate { get; set; }
        public int? WarrantyMileage { get; set; }
        [MaxLength(500)]
        public string? WarrantyDetails { get; set; }

        // Document attachments
        [MaxLength(1000)]
        public string? ReceiptPath { get; set; }
        [MaxLength(1000)]
        public string? ServiceReportPath { get; set; }

        // Parts and labor breakdown
        [Column(TypeName = "decimal(18,2)")]
        public decimal? PartsCost { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? LaborCost { get; set; }

        // Link to transaction if payment was recorded
        public string? TransactionId { get; set; }
        public virtual Transaction? Transaction { get; set; }

        // Calculated properties
        [NotMapped]
        public bool IsWarrantyValid => WarrantyExpiryDate.HasValue && WarrantyExpiryDate > DateTime.Now;

        [NotMapped]
        public bool IsServiceDue
        {
            get
            {
                if (NextServiceDue.HasValue && NextServiceDue <= DateTime.Now.AddDays(30))
                    return true;
                
                // Add logic for mileage/hours-based service due dates
                return false;
            }
        }

        [NotMapped]
        public int DaysSinceService => (DateTime.Now - ServiceDate).Days;

        [NotMapped]
        public string ServiceSummary => $"{ServiceType} - {ServiceDate:MMM dd, yyyy} - ${Cost:F2}";
    }
}