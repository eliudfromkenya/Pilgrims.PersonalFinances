using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pilgrims.PersonalFinances.Core.Models.Enums;

namespace Pilgrims.PersonalFinances.Core.Models
{
    /// <summary>
    /// Represents a reusable report template with predefined configuration
    /// </summary>
    public class ReportTemplate : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public ReportType ReportType { get; set; }

        [Required]
        public ChartType DefaultChartType { get; set; }

        /// <summary>
        /// JSON configuration for the report template
        /// </summary>
        [Column(TypeName = "nvarchar(max)")]
        public string? Configuration { get; set; }

        /// <summary>
        /// Default date range preset for this template
        /// </summary>
        public DateRangePreset DefaultDateRange { get; set; } = DateRangePreset.ThisMonth;

        /// <summary>
        /// Default comparison period for this template
        /// </summary>
        public ComparisonPeriod DefaultComparisonPeriod { get; set; } = ComparisonPeriod.None;

        /// <summary>
        /// Whether this template is built-in (system) or user-created
        /// </summary>
        public bool IsSystemTemplate { get; set; } = false;

        /// <summary>
        /// Whether this template is active and available for use
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Display order for template listing
        /// </summary>
        public int DisplayOrder { get; set; } = 0;

        /// <summary>
        /// Category or group for organizing templates
        /// </summary>
        [StringLength(50)]
        public string? Category { get; set; }

        /// <summary>
        /// Tags for searching and filtering templates
        /// </summary>
        [StringLength(200)]
        public string? Tags { get; set; }

        /// <summary>
        /// Icon or image for the template
        /// </summary>
        [StringLength(100)]
        public string? Icon { get; set; }

        /// <summary>
        /// Color theme for the template
        /// </summary>
        [StringLength(20)]
        public string? ColorTheme { get; set; }

        /// <summary>
        /// Minimum role required to access this template
        /// </summary>
        [StringLength(50)]
        public string? RequiredRole { get; set; }

        /// <summary>
        /// Usage count for analytics
        /// </summary>
        public int UsageCount { get; set; } = 0;

        /// <summary>
        /// Last time this template was used
        /// </summary>
        public DateTime? LastUsedAt { get; set; }

        /// <summary>
        /// User who created this template (null for system templates)
        /// </summary>
        [StringLength(100)]
        public string? CreatedBy { get; set; }

        /// <summary>
        /// User who last updated this template
        /// </summary>
        [StringLength(100)]
        public string? UpdatedBy { get; set; }

        // Navigation Properties
        public virtual ICollection<ReportParameter> Parameters { get; set; } = new List<ReportParameter>();
        public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

        // Computed Properties
        [NotMapped]
        public bool IsPopular => UsageCount > 10;

        [NotMapped]
        public bool IsRecentlyUsed => LastUsedAt.HasValue && LastUsedAt.Value > DateTime.UtcNow.AddDays(-7);

        [NotMapped]
        public string DisplayName => !string.IsNullOrEmpty(Description) ? $"{Name} - {Description}" : Name;

        [NotMapped]
        public List<string> TagList => !string.IsNullOrEmpty(Tags) 
            ? Tags.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()).ToList()
            : new List<string>();

        // Methods
        public void IncrementUsage()
        {
            UsageCount++;
            LastUsedAt = DateTime.UtcNow;
            MarkAsDirty();
        }

        public void UpdateConfiguration(string configuration, string? updatedBy = null)
        {
            Configuration = configuration;
            UpdatedBy = updatedBy;
            MarkAsDirty();
        }

        public void AddTag(string tag)
        {
            var currentTags = TagList;
            if (!currentTags.Contains(tag, StringComparer.OrdinalIgnoreCase))
            {
                currentTags.Add(tag);
                Tags = string.Join(", ", currentTags);
                MarkAsDirty();
            }
        }

        public void RemoveTag(string tag)
        {
            var currentTags = TagList;
            var removed = currentTags.RemoveAll(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase));
            if (removed > 0)
            {
                Tags = string.Join(", ", currentTags);
                MarkAsDirty();
            }
        }

        public bool HasTag(string tag)
        {
            return TagList.Contains(tag, StringComparer.OrdinalIgnoreCase);
        }

        public ReportTemplate Clone(string newName, string? createdBy = null)
        {
            return new ReportTemplate
            {
                Name = newName,
                Description = Description,
                ReportType = ReportType,
                DefaultChartType = DefaultChartType,
                Configuration = Configuration,
                DefaultDateRange = DefaultDateRange,
                DefaultComparisonPeriod = DefaultComparisonPeriod,
                IsSystemTemplate = false,
                IsActive = true,
                Category = Category,
                Tags = Tags,
                Icon = Icon,
                ColorTheme = ColorTheme,
                CreatedBy = createdBy
            };
        }
    }
}
