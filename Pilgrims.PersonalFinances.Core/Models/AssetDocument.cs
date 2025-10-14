using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pilgrims.PersonalFinances.Core.Models.Enums;

namespace Pilgrims.PersonalFinances.Core.Models
{
    public class AssetDocument : BaseEntity
    {
        [Required]
        public string AssetRegisterId { get; set; } = string.Empty;

        [ForeignKey("AssetRegisterId")]
        public virtual AssetRegister AssetRegister { get; set; }

        [Required]
        [StringLength(255)]
        public string DocumentName { get; set; }

        [Required]
        public DocumentType DocumentType { get; set; }

        [Required]
        [StringLength(500)]
        public string FilePath { get; set; }

        [StringLength(50)]
        public string? FileExtension { get; set; }

        public long? FileSize { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        public DateTime UploadDate { get; set; } = DateTime.UtcNow;

        public DateTime? ExpiryDate { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
