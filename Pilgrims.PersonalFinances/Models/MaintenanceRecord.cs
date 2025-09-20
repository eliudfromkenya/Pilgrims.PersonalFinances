using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pilgrims.PersonalFinances.Models.Enums;

namespace Pilgrims.PersonalFinances.Models
{
    public class MaintenanceRecord
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AssetRegisterId { get; set; }

        [ForeignKey("AssetRegisterId")]
        public virtual AssetRegister AssetRegister { get; set; }

        [Required]
        [StringLength(255)]
        public string MaintenanceType { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        public DateTime MaintenanceDate { get; set; }

        public DateTime? NextMaintenanceDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Cost { get; set; }

        [StringLength(255)]
        public string? ServiceProvider { get; set; }

        [StringLength(100)]
        public string? ServiceProviderContact { get; set; }

        [StringLength(500)]
        public string? DocumentPath { get; set; }

        public MaintenanceStatus Status { get; set; } = MaintenanceStatus.Completed;

        [StringLength(1000)]
        public string? Notes { get; set; }

        public bool IsRecurring { get; set; } = false;

        public int? RecurrenceIntervalDays { get; set; }

        // Audit Fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}