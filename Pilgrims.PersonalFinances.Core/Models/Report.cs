using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pilgrims.PersonalFinances.Core.Models.Enums;

namespace Pilgrims.PersonalFinances.Core.Models
{
    /// <summary>
    /// Represents a generated report instance
    /// </summary>
    public class Report : BaseEntity
    {
        /// <summary>
        /// Reference to the report template used
        /// </summary>
        public string? ReportTemplateId { get; set; }

        /// <summary>
        /// Report title/name
        /// </summary>
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Report description
        /// </summary>
        [StringLength(1000)]
        public string? Description { get; set; }

        /// <summary>
        /// Type of report
        /// </summary>
        [Required]
        public ReportType ReportType { get; set; }

        /// <summary>
        /// Chart type used for visualization
        /// </summary>
        public ChartType ChartType { get; set; } = ChartType.Table;

        /// <summary>
        /// Current status of the report
        /// </summary>
        [Required]
        public ReportStatus Status { get; set; } = ReportStatus.Pending;

        /// <summary>
        /// Date range start for the report data
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Date range end for the report data
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Date range preset used
        /// </summary>
        public DateRangePreset? DateRangePreset { get; set; }

        /// <summary>
        /// Comparison period for the report
        /// </summary>
        public ComparisonPeriod ComparisonPeriod { get; set; } = ComparisonPeriod.None;

        /// <summary>
        /// Report parameters used (JSON format)
        /// </summary>
        [Column(TypeName = "nvarchar(max)")]
        public string? Parameters { get; set; }

        /// <summary>
        /// Generated report data (JSON format)
        /// </summary>
        [Column(TypeName = "nvarchar(max)")]
        public string? Data { get; set; }

        /// <summary>
        /// Chart configuration and data (JSON format)
        /// </summary>
        [Column(TypeName = "nvarchar(max)")]
        public string? ChartData { get; set; }

        /// <summary>
        /// Summary statistics (JSON format)
        /// </summary>
        [Column(TypeName = "nvarchar(max)")]
        public string? Summary { get; set; }

        /// <summary>
        /// Error message if report generation failed
        /// </summary>
        [StringLength(1000)]
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// File path for exported report
        /// </summary>
        [StringLength(500)]
        public string? ExportPath { get; set; }

        /// <summary>
        /// Export format used
        /// </summary>
        public ExportFormat? ExportFormat { get; set; }

        /// <summary>
        /// Size of the exported file in bytes
        /// </summary>
        public long? FileSize { get; set; }

        /// <summary>
        /// Number of records/rows in the report
        /// </summary>
        public int RecordCount { get; set; } = 0;

        /// <summary>
        /// Time taken to generate the report (in milliseconds)
        /// </summary>
        public int? GenerationTimeMs { get; set; }

        /// <summary>
        /// Whether the report is scheduled/automated
        /// </summary>
        public bool IsScheduled { get; set; } = false;

        /// <summary>
        /// Frequency for scheduled reports
        /// </summary>
        public ReportFrequency? ScheduleFrequency { get; set; }

        /// <summary>
        /// Next scheduled run time
        /// </summary>
        public DateTime? NextRunTime { get; set; }

        /// <summary>
        /// Whether the report is shared with others
        /// </summary>
        public bool IsShared { get; set; } = false;

        /// <summary>
        /// Whether the report is marked as favorite
        /// </summary>
        public bool IsFavorite { get; set; } = false;

        /// <summary>
        /// Number of times this report has been viewed
        /// </summary>
        public int ViewCount { get; set; } = 0;

        /// <summary>
        /// Last time this report was viewed
        /// </summary>
        public DateTime? LastViewedAt { get; set; }

        /// <summary>
        /// Tags for categorizing and searching reports
        /// </summary>
        [StringLength(200)]
        public string? Tags { get; set; }

        /// <summary>
        /// User who created this report
        /// </summary>
        [Required]
        [StringLength(100)]
        public string CreatedBy { get; set; } = string.Empty;

        /// <summary>
        /// User who last updated this report
        /// </summary>
        [StringLength(100)]
        public string? UpdatedBy { get; set; }

        /// <summary>
        /// When this report expires (for cleanup)
        /// </summary>
        public DateTime? ExpiresAt { get; set; }

        // Navigation Properties
        [ForeignKey(nameof(ReportTemplateId))]
        public virtual ReportTemplate? ReportTemplate { get; set; }

        // Computed Properties
        [NotMapped]
        public bool IsCompleted => Status == ReportStatus.Completed;

        [NotMapped]
        public bool IsFailed => Status == ReportStatus.Failed;

        [NotMapped]
        public bool IsExpired => ExpiresAt.HasValue && ExpiresAt.Value < DateTime.UtcNow;

        [NotMapped]
        public bool IsRecent => CreatedAt > DateTime.UtcNow.AddDays(-7);

        [NotMapped]
        public string StatusDisplay => Status switch
        {
            ReportStatus.Pending => "Pending",
            ReportStatus.Processing => "Processing",
            ReportStatus.Completed => "Completed",
            ReportStatus.Failed => "Failed",
            ReportStatus.Cancelled => "Cancelled",
            _ => "Unknown"
        };

        [NotMapped]
        public string FileSizeDisplay
        {
            get
            {
                if (!FileSize.HasValue) return "N/A";
                
                var size = FileSize.Value;
                string[] sizes = { "B", "KB", "MB", "GB" };
                int order = 0;
                while (size >= 1024 && order < sizes.Length - 1)
                {
                    order++;
                    size = size / 1024;
                }
                return $"{size:0.##} {sizes[order]}";
            }
        }

        [NotMapped]
        public string GenerationTimeDisplay
        {
            get
            {
                if (!GenerationTimeMs.HasValue) return "N/A";
                
                var ms = GenerationTimeMs.Value;
                if (ms < 1000) return $"{ms} ms";
                if (ms < 60000) return $"{ms / 1000.0:F1} s";
                return $"{ms / 60000.0:F1} min";
            }
        }

        [NotMapped]
        public List<string> TagList => !string.IsNullOrEmpty(Tags) 
            ? Tags.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()).ToList()
            : new List<string>();

        [NotMapped]
        public string DateRangeDisplay
        {
            get
            {
                if (StartDate.HasValue && EndDate.HasValue)
                {
                    if (StartDate.Value.Date == EndDate.Value.Date)
                        return StartDate.Value.ToString("MMM dd, yyyy");
                    return $"{StartDate.Value:MMM dd, yyyy} - {EndDate.Value:MMM dd, yyyy}";
                }
                if (DateRangePreset.HasValue)
                {
                    return DateRangePreset.Value switch
                    {
                        DateRangePreset.Today => "Today",
                        DateRangePreset.Yesterday => "Yesterday",
                        DateRangePreset.ThisWeek => "This Week",
                        DateRangePreset.LastWeek => "Last Week",
                        DateRangePreset.ThisMonth => "This Month",
                        DateRangePreset.LastMonth => "Last Month",
                        DateRangePreset.ThisQuarter => "This Quarter",
                        DateRangePreset.LastQuarter => "Last Quarter",
                        DateRangePreset.ThisYear => "This Year",
                        DateRangePreset.LastYear => "Last Year",
                        _ => "Custom Range"
                    };
                }
                return "All Time";
            }
        }

        // Methods
        public void MarkAsViewed()
        {
            ViewCount++;
            LastViewedAt = DateTime.UtcNow;
            MarkAsDirty();
        }

        public void UpdateStatus(ReportStatus status, string? errorMessage = null)
        {
            Status = status;
            ErrorMessage = errorMessage;
            MarkAsDirty();
        }

        public void SetData(object data)
        {
            Data = System.Text.Json.JsonSerializer.Serialize(data);
            MarkAsDirty();
        }

        public void SetChartData(object chartData)
        {
            ChartData = System.Text.Json.JsonSerializer.Serialize(chartData);
            MarkAsDirty();
        }

        public void SetSummary(object summary)
        {
            Summary = System.Text.Json.JsonSerializer.Serialize(summary);
            MarkAsDirty();
        }

        public void SetParameters(Dictionary<string, object> parameters)
        {
            Parameters = System.Text.Json.JsonSerializer.Serialize(parameters);
            MarkAsDirty();
        }

        public Dictionary<string, object> GetParameters()
        {
            if (string.IsNullOrEmpty(Parameters))
                return new Dictionary<string, object>();

            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(Parameters) 
                    ?? new Dictionary<string, object>();
            }
            catch
            {
                return new Dictionary<string, object>();
            }
        }

        public T? GetData<T>() where T : class
        {
            if (string.IsNullOrEmpty(Data))
                return null;

            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<T>(Data);
            }
            catch
            {
                return null;
            }
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

        public void SetExportInfo(string filePath, ExportFormat format, long fileSize)
        {
            ExportPath = filePath;
            ExportFormat = format;
            FileSize = fileSize;
            MarkAsDirty();
        }

        public void ScheduleNext()
        {
            if (!IsScheduled || !ScheduleFrequency.HasValue)
                return;

            NextRunTime = ScheduleFrequency.Value switch
            {
                ReportFrequency.Daily => DateTime.UtcNow.AddDays(1),
                ReportFrequency.Weekly => DateTime.UtcNow.AddDays(7),
                ReportFrequency.Monthly => DateTime.UtcNow.AddMonths(1),
                ReportFrequency.Quarterly => DateTime.UtcNow.AddMonths(3),
                ReportFrequency.Yearly => DateTime.UtcNow.AddYears(1),
                _ => null
            };
            MarkAsDirty();
        }
    }
}

