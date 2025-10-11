using Pilgrims.PersonalFinances.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace Pilgrims.PersonalFinances.Models
{
    /// <summary>
    /// Represents a detailed audit entry for tracking individual field changes
    /// </summary>
    public class AuditEntry : BaseEntity
    {
        /// <summary>
        /// The ID of the parent audit log
        /// </summary>
        [Required]
        public string AuditLogId { get; set; } = string.Empty;

        /// <summary>
        /// The name of the property/field that was changed
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string PropertyName { get; set; } = string.Empty;

        /// <summary>
        /// The old value of the property (before change)
        /// </summary>
        public string? OldValue { get; set; }

        /// <summary>
        /// The new value of the property (after change)
        /// </summary>
        public string? NewValue { get; set; }

        /// <summary>
        /// The data type of the property
        /// </summary>
        [MaxLength(50)]
        public string? DataType { get; set; }

        /// <summary>
        /// Whether this property change is considered sensitive
        /// </summary>
        public bool IsSensitive { get; set; } = false;

        /// <summary>
        /// Navigation property to the parent audit log
        /// </summary>
        public virtual AuditLog AuditLog { get; set; } = null!;
    }
}