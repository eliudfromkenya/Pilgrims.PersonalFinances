using Pilgrims.PersonalFinances.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace Pilgrims.PersonalFinances.Models
{
    /// <summary>
    /// Represents an audit log entry for tracking database operations
    /// </summary>
    public class AuditLog : BaseEntity
    {
        /// <summary>
        /// The user ID who performed the operation
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// The type of operation performed (Insert, Update, Delete, View, Print, Export)
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string Operation { get; set; } = string.Empty;

        /// <summary>
        /// The name of the entity/table that was affected
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string EntityName { get; set; } = string.Empty;

        /// <summary>
        /// The ID of the entity that was affected
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string EntityId { get; set; } = string.Empty;

        /// <summary>
        /// JSON representation of the old values (for Update operations)
        /// </summary>
        public string? OldValues { get; set; }

        /// <summary>
        /// JSON representation of the new values (for Insert/Update operations)
        /// </summary>
        public string? NewValues { get; set; }

        /// <summary>
        /// Additional context or description of the operation
        /// </summary>
        [MaxLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// IP address of the user (if applicable)
        /// </summary>
        [MaxLength(45)]
        public string? IpAddress { get; set; }

        /// <summary>
        /// User agent information (if applicable)
        /// </summary>
        [MaxLength(500)]
        public string? UserAgent { get; set; }

        /// <summary>
        /// The timestamp when the operation occurred
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Collection of detailed audit entries for this log
        /// </summary>
        public virtual ICollection<AuditEntry> AuditEntries { get; set; } = new List<AuditEntry>();
    }
}