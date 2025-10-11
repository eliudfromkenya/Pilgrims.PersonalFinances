using Pilgrims.PersonalFinances.Core.Models;

namespace Pilgrims.PersonalFinances.Core.Services.Interfaces
{
    /// <summary>
    /// Interface for audit trail service operations
    /// </summary>
    public interface IAuditService
    {
        /// <summary>
        /// Creates an audit log entry for an entity creation operation
        /// </summary>
        /// <param name="userId">The ID of the user performing the operation</param>
        /// <param name="entityName">The name of the entity being created</param>
        /// <param name="entityId">The ID of the entity being created</param>
        /// <param name="newValues">The new values of the entity</param>
        /// <param name="description">Optional description of the operation</param>
        /// <param name="ipAddress">Optional IP address of the user</param>
        /// <param name="userAgent">Optional user agent information</param>
        /// <returns>The created audit log entry</returns>
        Task<AuditLog> LogEntityCreatedAsync(string userId, string entityName, string entityId, object newValues, string? description = null, string? ipAddress = null, string? userAgent = null);

        /// <summary>
        /// Creates an audit log entry for an entity update operation
        /// </summary>
        /// <param name="userId">The ID of the user performing the operation</param>
        /// <param name="entityName">The name of the entity being updated</param>
        /// <param name="entityId">The ID of the entity being updated</param>
        /// <param name="oldValues">The old values of the entity</param>
        /// <param name="newValues">The new values of the entity</param>
        /// <param name="changedProperties">The list of properties that were changed</param>
        /// <param name="description">Optional description of the operation</param>
        /// <param name="ipAddress">Optional IP address of the user</param>
        /// <param name="userAgent">Optional user agent information</param>
        /// <returns>The created audit log entry</returns>
        Task<AuditLog> LogEntityUpdatedAsync(string userId, string entityName, string entityId, object oldValues, object newValues, IEnumerable<string> changedProperties, string? description = null, string? ipAddress = null, string? userAgent = null);

        /// <summary>
        /// Creates an audit log entry for an entity deletion operation
        /// </summary>
        /// <param name="userId">The ID of the user performing the operation</param>
        /// <param name="entityName">The name of the entity being deleted</param>
        /// <param name="entityId">The ID of the entity being deleted</param>
        /// <param name="oldValues">The old values of the entity before deletion</param>
        /// <param name="description">Optional description of the operation</param>
        /// <param name="ipAddress">Optional IP address of the user</param>
        /// <param name="userAgent">Optional user agent information</param>
        /// <returns>The created audit log entry</returns>
        Task<AuditLog> LogEntityDeletedAsync(string userId, string entityName, string entityId, object oldValues, string? description = null, string? ipAddress = null, string? userAgent = null);

        /// <summary>
        /// Creates an audit log entry for a report view operation
        /// </summary>
        /// <param name="userId">The ID of the user viewing the report</param>
        /// <param name="reportType">The type of report being viewed</param>
        /// <param name="parameters">Optional parameters used for the report</param>
        /// <param name="description">Optional description of the operation</param>
        /// <param name="ipAddress">Optional IP address of the user</param>
        /// <param name="userAgent">Optional user agent information</param>
        /// <returns>The created audit log entry</returns>
        Task<AuditLog> LogReportViewedAsync(string userId, string reportType, Dictionary<string, object>? parameters = null, string? description = null, string? ipAddress = null, string? userAgent = null);

        /// <summary>
        /// Creates an audit log entry for a report print operation
        /// </summary>
        /// <param name="userId">The ID of the user printing the report</param>
        /// <param name="reportType">The type of report being printed</param>
        /// <param name="parameters">Optional parameters used for the report</param>
        /// <param name="printerName">Optional name of the printer used</param>
        /// <param name="description">Optional description of the operation</param>
        /// <param name="ipAddress">Optional IP address of the user</param>
        /// <param name="userAgent">Optional user agent information</param>
        /// <returns>The created audit log entry</returns>
        Task<AuditLog> LogReportPrintedAsync(string userId, string reportType, Dictionary<string, object>? parameters = null, string? printerName = null, string? description = null, string? ipAddress = null, string? userAgent = null);

        /// <summary>
        /// Creates an audit log entry for a report export operation
        /// </summary>
        /// <param name="userId">The ID of the user exporting the report</param>
        /// <param name="reportType">The type of report being exported</param>
        /// <param name="exportFormat">The format of the export (PDF, CSV, etc.)</param>
        /// <param name="filePath">Optional path where the file was saved</param>
        /// <param name="parameters">Optional parameters used for the report</param>
        /// <param name="description">Optional description of the operation</param>
        /// <param name="ipAddress">Optional IP address of the user</param>
        /// <param name="userAgent">Optional user agent information</param>
        /// <returns>The created audit log entry</returns>
        Task<AuditLog> LogReportExportedAsync(string userId, string reportType, string exportFormat, string? filePath = null, Dictionary<string, object>? parameters = null, string? description = null, string? ipAddress = null, string? userAgent = null);

        /// <summary>
        /// Retrieves audit logs for a specific entity
        /// </summary>
        /// <param name="entityName">The name of the entity</param>
        /// <param name="entityId">The ID of the entity</param>
        /// <param name="pageNumber">The page number for pagination</param>
        /// <param name="pageSize">The page size for pagination</param>
        /// <returns>A list of audit logs for the specified entity</returns>
        Task<IEnumerable<AuditLog>> GetAuditLogsForEntityAsync(string entityName, string entityId, int pageNumber = 1, int pageSize = 50);

        /// <summary>
        /// Retrieves audit logs for a specific user
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <param name="pageNumber">The page number for pagination</param>
        /// <param name="pageSize">The page size for pagination</param>
        /// <returns>A list of audit logs for the specified user</returns>
        Task<IEnumerable<AuditLog>> GetAuditLogsForUserAsync(string userId, int pageNumber = 1, int pageSize = 50);

        /// <summary>
        /// Retrieves audit logs within a specific date range
        /// </summary>
        /// <param name="startDate">The start date for the range</param>
        /// <param name="endDate">The end date for the range</param>
        /// <param name="pageNumber">The page number for pagination</param>
        /// <param name="pageSize">The page size for pagination</param>
        /// <returns>A list of audit logs within the specified date range</returns>
        Task<IEnumerable<AuditLog>> GetAuditLogsByDateRangeAsync(DateTime startDate, DateTime endDate, int pageNumber = 1, int pageSize = 50);

        /// <summary>
        /// Cleans up old audit logs based on retention policy
        /// </summary>
        /// <param name="retentionDays">Number of days to retain audit logs</param>
        /// <returns>The number of audit logs deleted</returns>
        Task<int> CleanupOldAuditLogsAsync(int retentionDays = 365);
    }
}
