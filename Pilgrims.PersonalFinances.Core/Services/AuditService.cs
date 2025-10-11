using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pilgrims.PersonalFinances.Core.Messaging.Interfaces;
using Pilgrims.PersonalFinances.Core.Messaging.Messages;
using Pilgrims.PersonalFinances.Data;
using Pilgrims.PersonalFinances.Core.Models;
using Pilgrims.PersonalFinances.Core.Services.Interfaces;

namespace Pilgrims.PersonalFinances.Core.Services
{
    /// <summary>
    /// Service for managing audit trail operations
    /// </summary>
    public class AuditService : IAuditService
    {
        private readonly PersonalFinanceContext _context;
        private readonly IMessagingService _messagingService;
        private readonly ILogger<AuditService> _logger;

        public AuditService(
            PersonalFinanceContext context,
            IMessagingService messagingService,
            ILogger<AuditService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _messagingService = messagingService ?? throw new ArgumentNullException(nameof(messagingService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<AuditLog> LogEntityCreatedAsync(string userId, string entityName, string entityId, object newValues, string? description = null, string? ipAddress = null, string? userAgent = null)
        {
            try
            {
                var auditLog = new AuditLog
                {
                    UserId = userId,
                    Operation = "Insert",
                    EntityName = entityName,
                    EntityId = entityId,
                    NewValues = JsonConvert.SerializeObject(newValues, Formatting.Indented),
                    Description = description,
                    IpAddress = ipAddress,
                    UserAgent = userAgent,
                    Timestamp = DateTime.UtcNow
                };

                _context.AuditLogs.Add(auditLog);
                await _context.SaveChangesAsync();

                // Publish audit message
                var message = new EntityCreatedMessage(userId, entityName, entityId, newValues, description);
                _messagingService.Send(message);

                // Publish audit log created message
                var auditLogMessage = new AuditLogCreatedMessage(auditLog);
                _messagingService.Send(auditLogMessage);

                _logger.LogInformation("Entity created audit log recorded: {EntityName} {EntityId} by user {UserId}", entityName, entityId, userId);

                return auditLog;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating audit log for entity creation: {EntityName} {EntityId}", entityName, entityId);
                throw;
            }
        }

        public async Task<AuditLog> LogEntityUpdatedAsync(string userId, string entityName, string entityId, object oldValues, object newValues, IEnumerable<string> changedProperties, string? description = null, string? ipAddress = null, string? userAgent = null)
        {
            try
            {
                var auditLog = new AuditLog
                {
                    UserId = userId,
                    Operation = "Update",
                    EntityName = entityName,
                    EntityId = entityId,
                    OldValues = JsonConvert.SerializeObject(oldValues, Formatting.Indented),
                    NewValues = JsonConvert.SerializeObject(newValues, Formatting.Indented),
                    Description = description,
                    IpAddress = ipAddress,
                    UserAgent = userAgent,
                    Timestamp = DateTime.UtcNow
                };

                // Create detailed audit entries for each changed property
                foreach (var propertyName in changedProperties)
                {
                    var oldProperty = oldValues.GetType().GetProperty(propertyName);
                    var newProperty = newValues.GetType().GetProperty(propertyName);

                    var auditEntry = new AuditEntry
                    {
                        AuditLogId = auditLog.Id,
                        PropertyName = propertyName,
                        OldValue = oldProperty?.GetValue(oldValues)?.ToString(),
                        NewValue = newProperty?.GetValue(newValues)?.ToString(),
                        DataType = oldProperty?.PropertyType.Name,
                        IsSensitive = IsSensitiveProperty(entityName, propertyName)
                    };

                    auditLog.AuditEntries.Add(auditEntry);
                }

                _context.AuditLogs.Add(auditLog);
                await _context.SaveChangesAsync();

                // Publish audit message
                var message = new EntityUpdatedMessage(userId, entityName, entityId, oldValues, newValues, changedProperties, description);
                _messagingService.Send(message);

                // Publish audit log created message
                var auditLogMessage = new AuditLogCreatedMessage(auditLog);
                _messagingService.Send(auditLogMessage);

                _logger.LogInformation("Entity updated audit log recorded: {EntityName} {EntityId} by user {UserId}, {PropertyCount} properties changed", entityName, entityId, userId, changedProperties.Count());

                return auditLog;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating audit log for entity update: {EntityName} {EntityId}", entityName, entityId);
                throw;
            }
        }

        public async Task<AuditLog> LogEntityDeletedAsync(string userId, string entityName, string entityId, object oldValues, string? description = null, string? ipAddress = null, string? userAgent = null)
        {
            try
            {
                var auditLog = new AuditLog
                {
                    UserId = userId,
                    Operation = "Delete",
                    EntityName = entityName,
                    EntityId = entityId,
                    OldValues = JsonConvert.SerializeObject(oldValues, Formatting.Indented),
                    Description = description,
                    IpAddress = ipAddress,
                    UserAgent = userAgent,
                    Timestamp = DateTime.UtcNow
                };

                _context.AuditLogs.Add(auditLog);
                await _context.SaveChangesAsync();

                // Publish audit message
                var message = new EntityDeletedMessage(userId, entityName, entityId, oldValues, description);
                _messagingService.Send(message);

                // Publish audit log created message
                var auditLogMessage = new AuditLogCreatedMessage(auditLog);
                _messagingService.Send(auditLogMessage);

                _logger.LogInformation("Entity deleted audit log recorded: {EntityName} {EntityId} by user {UserId}", entityName, entityId, userId);

                return auditLog;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating audit log for entity deletion: {EntityName} {EntityId}", entityName, entityId);
                throw;
            }
        }

        public async Task<AuditLog> LogReportViewedAsync(string userId, string reportType, Dictionary<string, object>? parameters = null, string? description = null, string? ipAddress = null, string? userAgent = null)
        {
            try
            {
                var auditLog = new AuditLog
                {
                    UserId = userId,
                    Operation = "View",
                    EntityName = "Report",
                    EntityId = reportType,
                    NewValues = parameters != null ? JsonConvert.SerializeObject(parameters, Formatting.Indented) : null,
                    Description = description ?? $"Viewed {reportType} report",
                    IpAddress = ipAddress,
                    UserAgent = userAgent,
                    Timestamp = DateTime.UtcNow
                };

                _context.AuditLogs.Add(auditLog);
                await _context.SaveChangesAsync();

                // Publish audit message
                var message = new ReportViewedMessage(userId, reportType, parameters, description);
                _messagingService.Send(message);

                // Publish audit log created message
                var auditLogMessage = new AuditLogCreatedMessage(auditLog);
                _messagingService.Send(auditLogMessage);

                _logger.LogInformation("Report viewed audit log recorded: {ReportType} by user {UserId}", reportType, userId);

                return auditLog;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating audit log for report view: {ReportType}", reportType);
                throw;
            }
        }

        public async Task<AuditLog> LogReportPrintedAsync(string userId, string reportType, Dictionary<string, object>? parameters = null, string? printerName = null, string? description = null, string? ipAddress = null, string? userAgent = null)
        {
            try
            {
                var reportData = new
                {
                    Parameters = parameters,
                    PrinterName = printerName
                };

                var auditLog = new AuditLog
                {
                    UserId = userId,
                    Operation = "Print",
                    EntityName = "Report",
                    EntityId = reportType,
                    NewValues = JsonConvert.SerializeObject(reportData, Formatting.Indented),
                    Description = description ?? $"Printed {reportType} report",
                    IpAddress = ipAddress,
                    UserAgent = userAgent,
                    Timestamp = DateTime.UtcNow
                };

                _context.AuditLogs.Add(auditLog);
                await _context.SaveChangesAsync();

                // Publish audit message
                var message = new ReportPrintedMessage(userId, reportType, parameters, printerName, description);
                _messagingService.Send(message);

                // Publish audit log created message
                var auditLogMessage = new AuditLogCreatedMessage(auditLog);
                _messagingService.Send(auditLogMessage);

                _logger.LogInformation("Report printed audit log recorded: {ReportType} by user {UserId}", reportType, userId);

                return auditLog;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating audit log for report print: {ReportType}", reportType);
                throw;
            }
        }

        public async Task<AuditLog> LogReportExportedAsync(string userId, string reportType, string exportFormat, string? filePath = null, Dictionary<string, object>? parameters = null, string? description = null, string? ipAddress = null, string? userAgent = null)
        {
            try
            {
                var exportData = new
                {
                    ExportFormat = exportFormat,
                    FilePath = filePath,
                    Parameters = parameters
                };

                var auditLog = new AuditLog
                {
                    UserId = userId,
                    Operation = "Export",
                    EntityName = "Report",
                    EntityId = reportType,
                    NewValues = JsonConvert.SerializeObject(exportData, Formatting.Indented),
                    Description = description ?? $"Exported {reportType} report as {exportFormat}",
                    IpAddress = ipAddress,
                    UserAgent = userAgent,
                    Timestamp = DateTime.UtcNow
                };

                _context.AuditLogs.Add(auditLog);
                await _context.SaveChangesAsync();

                // Publish audit message
                var message = new ReportExportedMessage(userId, reportType, exportFormat, filePath, parameters, description);
                _messagingService.Send(message);

                // Publish audit log created message
                var auditLogMessage = new AuditLogCreatedMessage(auditLog);
                _messagingService.Send(auditLogMessage);

                _logger.LogInformation("Report exported audit log recorded: {ReportType} as {ExportFormat} by user {UserId}", reportType, exportFormat, userId);

                return auditLog;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating audit log for report export: {ReportType}", reportType);
                throw;
            }
        }

        public async Task<IEnumerable<AuditLog>> GetAuditLogsForEntityAsync(string entityName, string entityId, int pageNumber = 1, int pageSize = 50)
        {
            try
            {
                return await _context.AuditLogs
                    .Include(al => al.AuditEntries)
                    .Where(al => al.EntityName == entityName && al.EntityId == entityId)
                    .OrderByDescending(al => al.Timestamp)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving audit logs for entity: {EntityName} {EntityId}", entityName, entityId);
                throw;
            }
        }

        public async Task<IEnumerable<AuditLog>> GetAuditLogsForUserAsync(string userId, int pageNumber = 1, int pageSize = 50)
        {
            try
            {
                return await _context.AuditLogs
                    .Include(al => al.AuditEntries)
                    .Where(al => al.UserId == userId)
                    .OrderByDescending(al => al.Timestamp)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving audit logs for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<AuditLog>> GetAuditLogsByDateRangeAsync(DateTime startDate, DateTime endDate, int pageNumber = 1, int pageSize = 50)
        {
            try
            {
                return await _context.AuditLogs
                    .Include(al => al.AuditEntries)
                    .Where(al => al.Timestamp >= startDate && al.Timestamp <= endDate)
                    .OrderByDescending(al => al.Timestamp)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving audit logs for date range: {StartDate} to {EndDate}", startDate, endDate);
                throw;
            }
        }

        public async Task<int> CleanupOldAuditLogsAsync(int retentionDays = 365)
        {
            try
            {
                var cutoffDate = DateTime.UtcNow.AddDays(-retentionDays);
                var oldAuditLogs = await _context.AuditLogs
                    .Where(al => al.Timestamp < cutoffDate)
                    .ToListAsync();

                if (oldAuditLogs.Any())
                {
                    _context.AuditLogs.RemoveRange(oldAuditLogs);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Cleaned up {Count} old audit logs older than {CutoffDate}", oldAuditLogs.Count, cutoffDate);
                }

                return oldAuditLogs.Count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up old audit logs");
                throw;
            }
        }

        /// <summary>
        /// Determines if a property is considered sensitive and should be handled with extra care
        /// </summary>
        private static bool IsSensitiveProperty(string entityName, string propertyName)
        {
            var sensitiveProperties = new Dictionary<string, string[]>
            {
                { "User", new[] { "Password", "PasswordHash", "Email", "PhoneNumber", "SSN", "TaxId" } },
                { "Account", new[] { "AccountNumber", "RoutingNumber", "PIN" } },
                { "Transaction", new[] { "AccountNumber", "CheckNumber" } },
                { "Debt", new[] { "AccountNumber", "SSN" } },
                { "Asset", new[] { "SerialNumber", "VIN" } }
            };

            return sensitiveProperties.ContainsKey(entityName) &&
                   sensitiveProperties[entityName].Contains(propertyName, StringComparer.OrdinalIgnoreCase);
        }
    }
}
