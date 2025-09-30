using Pilgrims.PersonalFinances.Core.Messaging.Interfaces;
using Pilgrims.PersonalFinances.Models;

namespace Pilgrims.PersonalFinances.Core.Messaging.Messages
{
    /// <summary>
    /// Base message for all audit-related events
    /// </summary>
    public abstract class AuditMessage : IMessage
    {
        public string UserId { get; }
        public string EntityName { get; }
        public string EntityId { get; }
        public DateTime Timestamp { get; }
        public string? Description { get; }

        protected AuditMessage(string userId, string entityName, string entityId, string? description = null)
        {
            UserId = userId;
            EntityName = entityName;
            EntityId = entityId;
            Timestamp = DateTime.UtcNow;
            Description = description;
        }
    }

    /// <summary>
    /// Message published when an entity is created
    /// </summary>
    public class EntityCreatedMessage : AuditMessage
    {
        public object NewValues { get; }

        public EntityCreatedMessage(string userId, string entityName, string entityId, object newValues, string? description = null)
            : base(userId, entityName, entityId, description)
        {
            NewValues = newValues;
        }
    }

    /// <summary>
    /// Message published when an entity is updated
    /// </summary>
    public class EntityUpdatedMessage : AuditMessage
    {
        public object OldValues { get; }
        public object NewValues { get; }
        public IEnumerable<string> ChangedProperties { get; }

        public EntityUpdatedMessage(string userId, string entityName, string entityId, object oldValues, object newValues, IEnumerable<string> changedProperties, string? description = null)
            : base(userId, entityName, entityId, description)
        {
            OldValues = oldValues;
            NewValues = newValues;
            ChangedProperties = changedProperties;
        }
    }

    /// <summary>
    /// Message published when an entity is deleted
    /// </summary>
    public class EntityDeletedMessage : AuditMessage
    {
        public object OldValues { get; }

        public EntityDeletedMessage(string userId, string entityName, string entityId, object oldValues, string? description = null)
            : base(userId, entityName, entityId, description)
        {
            OldValues = oldValues;
        }
    }

    /// <summary>
    /// Message published when a report is viewed
    /// </summary>
    public class ReportViewedMessage : AuditMessage
    {
        public string ReportType { get; }
        public Dictionary<string, object>? Parameters { get; }

        public ReportViewedMessage(string userId, string reportType, Dictionary<string, object>? parameters = null, string? description = null)
            : base(userId, "Report", reportType, description)
        {
            ReportType = reportType;
            Parameters = parameters;
        }
    }

    /// <summary>
    /// Message published when a report is printed
    /// </summary>
    public class ReportPrintedMessage : AuditMessage
    {
        public string ReportType { get; }
        public Dictionary<string, object>? Parameters { get; }
        public string? PrinterName { get; }

        public ReportPrintedMessage(string userId, string reportType, Dictionary<string, object>? parameters = null, string? printerName = null, string? description = null)
            : base(userId, "Report", reportType, description)
        {
            ReportType = reportType;
            Parameters = parameters;
            PrinterName = printerName;
        }
    }

    /// <summary>
    /// Message published when a report is exported
    /// </summary>
    public class ReportExportedMessage : AuditMessage
    {
        public string ReportType { get; }
        public string ExportFormat { get; }
        public string? FilePath { get; }
        public Dictionary<string, object>? Parameters { get; }

        public ReportExportedMessage(string userId, string reportType, string exportFormat, string? filePath = null, Dictionary<string, object>? parameters = null, string? description = null)
            : base(userId, "Report", reportType, description)
        {
            ReportType = reportType;
            ExportFormat = exportFormat;
            FilePath = filePath;
            Parameters = parameters;
        }
    }

    /// <summary>
    /// Message published when an audit log entry is created
    /// </summary>
    public class AuditLogCreatedMessage : IMessage
    {
        public AuditLog AuditLog { get; }
        public DateTime Timestamp { get; }

        public AuditLogCreatedMessage(AuditLog auditLog)
        {
            AuditLog = auditLog;
            Timestamp = DateTime.UtcNow;
        }
    }
}