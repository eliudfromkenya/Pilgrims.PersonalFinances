using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pilgrims.PersonalFinances.Core.Messaging.Interfaces;
using Pilgrims.PersonalFinances.Core.Messaging.Messages;
using Pilgrims.PersonalFinances.Models;
using System.Collections.Concurrent;

namespace Pilgrims.PersonalFinances.Core.Interceptors
{
    /// <summary>
    /// Database interceptor that automatically captures CRUD operations for audit trail
    /// </summary>
    public class AuditInterceptor : SaveChangesInterceptor
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AuditInterceptor> _logger;
        private readonly ConcurrentDictionary<object, EntityAuditInfo> _auditInfoCache = new();

        public AuditInterceptor(IServiceProvider serviceProvider, ILogger<AuditInterceptor> logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            if (eventData.Context != null)
            {
                CaptureAuditInfo(eventData.Context);
            }
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            if (eventData.Context != null)
            {
                CaptureAuditInfo(eventData.Context);
            }
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
        {
            if (eventData.Context != null && result > 0)
            {
                _ = Task.Run(() => ProcessAuditInfo(eventData.Context));
            }
            return base.SavedChanges(eventData, result);
        }

        public override ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
        {
            if (eventData.Context != null && result > 0)
            {
                _ = Task.Run(() => ProcessAuditInfo(eventData.Context), cancellationToken);
            }
            return base.SavedChangesAsync(eventData, result, cancellationToken);
        }

        private void CaptureAuditInfo(DbContext context)
        {
            try
            {
                var entries = context.ChangeTracker.Entries()
                    .Where(e => e.Entity is BaseEntity && 
                               (e.State == EntityState.Added || 
                                e.State == EntityState.Modified || 
                                e.State == EntityState.Deleted))
                    .ToList();

                foreach (var entry in entries)
                {
                    var auditInfo = CreateAuditInfo(entry);
                    if (auditInfo != null)
                    {
                        _auditInfoCache[entry.Entity] = auditInfo;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error capturing audit information");
            }
        }

        private void ProcessAuditInfo(DbContext context)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var messagingService = scope.ServiceProvider.GetService<IMessagingService>();
                
                if (messagingService == null)
                {
                    _logger.LogWarning("MessagingService not available for audit processing");
                    return;
                }

                var auditInfos = _auditInfoCache.Values.ToList();
                _auditInfoCache.Clear();

                foreach (var auditInfo in auditInfos)
                {
                    PublishAuditMessage(messagingService, auditInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing audit information");
            }
        }

        private EntityAuditInfo? CreateAuditInfo(EntityEntry entry)
        {
            try
            {
                var entity = entry.Entity as BaseEntity;
                if (entity == null) return null;

                var entityType = entry.Entity.GetType();
                var entityName = entityType.Name;
                var entityId = entity.Id.ToString();

                // Get current user ID (this would typically come from a user context service)
                var userId = GetCurrentUserId();

                var auditInfo = new EntityAuditInfo
                {
                    UserId = userId,
                    EntityName = entityName,
                    EntityId = entityId,
                    Operation = entry.State,
                    Timestamp = DateTime.UtcNow
                };

                switch (entry.State)
                {
                    case EntityState.Added:
                        auditInfo.NewValues = GetEntityValues(entry, EntityState.Added);
                        break;

                    case EntityState.Modified:
                        auditInfo.OldValues = GetEntityValues(entry, EntityState.Modified, useOriginalValues: true);
                        auditInfo.NewValues = GetEntityValues(entry, EntityState.Modified, useOriginalValues: false);
                        auditInfo.ChangedProperties = GetChangedProperties(entry);
                        break;

                    case EntityState.Deleted:
                        auditInfo.OldValues = GetEntityValues(entry, EntityState.Deleted, useOriginalValues: true);
                        break;
                }

                return auditInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating audit info for entity: {EntityType}", entry.Entity.GetType().Name);
                return null;
            }
        }

        private void PublishAuditMessage(IMessagingService messagingService, EntityAuditInfo auditInfo)
        {
            try
            {
                switch (auditInfo.Operation)
                {
                    case EntityState.Added:
                        var createdMessage = new EntityCreatedMessage(
                            auditInfo.UserId,
                            auditInfo.EntityName,
                            auditInfo.EntityId,
                            auditInfo.NewValues,
                            $"Entity {auditInfo.EntityName} created via database interceptor"
                        );
                        messagingService.Send(createdMessage);
                        break;

                    case EntityState.Modified:
                        var updatedMessage = new EntityUpdatedMessage(
                            auditInfo.UserId,
                            auditInfo.EntityName,
                            auditInfo.EntityId,
                            auditInfo.OldValues,
                            auditInfo.NewValues,
                            auditInfo.ChangedProperties ?? new List<string>(),
                            $"Entity {auditInfo.EntityName} updated via database interceptor"
                        );
                        messagingService.Send(updatedMessage);
                        break;

                    case EntityState.Deleted:
                        var deletedMessage = new EntityDeletedMessage(
                            auditInfo.UserId,
                            auditInfo.EntityName,
                            auditInfo.EntityId,
                            auditInfo.OldValues,
                            $"Entity {auditInfo.EntityName} deleted via database interceptor"
                        );
                        messagingService.Send(deletedMessage);
                        break;
                }

                _logger.LogDebug("Published audit message for {Operation} on {EntityName} {EntityId}", 
                    auditInfo.Operation, auditInfo.EntityName, auditInfo.EntityId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing audit message for {EntityName} {EntityId}", 
                    auditInfo.EntityName, auditInfo.EntityId);
            }
        }

        private object GetEntityValues(EntityEntry entry, EntityState state, bool useOriginalValues = false)
        {
            var values = new Dictionary<string, object?>();

            foreach (var property in entry.Properties)
            {
                // Skip sensitive properties or system properties
                if (IsSensitiveProperty(entry.Entity.GetType().Name, property.Metadata.Name) ||
                    IsSystemProperty(property.Metadata.Name))
                {
                    continue;
                }

                object? value = null;
                try
                {
                    value = useOriginalValues && state == EntityState.Modified
                        ? property.OriginalValue
                        : property.CurrentValue;
                }
                catch (InvalidOperationException)
                {
                    // Some properties might not have original values
                    value = property.CurrentValue;
                }

                values[property.Metadata.Name] = value;
            }

            return values;
        }

        private List<string> GetChangedProperties(EntityEntry entry)
        {
            return entry.Properties
                .Where(p => p.IsModified && 
                           !IsSensitiveProperty(entry.Entity.GetType().Name, p.Metadata.Name) &&
                           !IsSystemProperty(p.Metadata.Name))
                .Select(p => p.Metadata.Name)
                .ToList();
        }

        private string GetCurrentUserId()
        {
            // This is a simplified implementation. In a real application, you would:
            // 1. Inject IHttpContextAccessor to get the current user from HTTP context
            // 2. Or inject a custom IUserContext service that provides the current user
            // 3. Handle cases where there's no current user (background services, etc.)
            
            try
            {
                //TODO
                using var scope = _serviceProvider.CreateScope();
                // Example: var userContext = scope.ServiceProvider.GetService<IUserContext>();
                // return userContext?.CurrentUserId ?? "System";
                
                // For now, return a default value
                return "System";
            }
            catch
            {
                return "System";
            }
        }

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

        private static bool IsSystemProperty(string propertyName)
        {
            var systemProperties = new[] { "CreatedAt", "UpdatedAt", "IsDirty" };
            return systemProperties.Contains(propertyName, StringComparer.OrdinalIgnoreCase);
        }
    }

    /// <summary>
    /// Internal class to hold audit information during the save process
    /// </summary>
    internal class EntityAuditInfo
    {
        public string UserId { get; set; } = string.Empty;
        public string EntityName { get; set; } = string.Empty;
        public string EntityId { get; set; } = string.Empty;
        public EntityState Operation { get; set; }
        public object? OldValues { get; set; }
        public object? NewValues { get; set; }
        public List<string>? ChangedProperties { get; set; }
        public DateTime Timestamp { get; set; }
    }
}