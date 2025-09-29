using Microsoft.Extensions.Logging;
using Serilog;
using System.Diagnostics;

namespace Pilgrims.PersonalFinances.Core.Logging
{
    public class LoggingService : ILoggingService
    {
        private readonly Serilog.ILogger _logger;

        public LoggingService(Serilog.ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void LogTrace(string message, params object[] args)
        {
            _logger.Verbose(message, args);
        }

        public void LogDebug(string message, params object[] args)
        {
            _logger.Debug(message, args);
        }

        public void LogInformation(string message, params object[] args)
        {
            _logger.Information(message, args);
        }

        public void LogWarning(string message, params object[] args)
        {
            _logger.Warning(message, args);
        }

        public void LogError(string message, params object[] args)
        {
            _logger.Error(message, args);
        }

        public void LogError(Exception exception, string message, params object[] args)
        {
            _logger.Error(exception, message, args);
        }

        public void LogCritical(string message, params object[] args)
        {
            _logger.Fatal(message, args);
        }

        public void LogCritical(Exception exception, string message, params object[] args)
        {
            _logger.Fatal(exception, message, args);
        }

        public void LogUserAction(string action, string details = "")
        {
            _logger.Information("User Action: {Action} - {Details}", action, details);
        }

        public void LogDatabaseOperation(string operation, string table, string details = "")
        {
            _logger.Debug("Database Operation: {Operation} on {Table} - {Details}", operation, table, details);
        }

        public void LogNotification(string type, string message, string recipient = "")
        {
            _logger.Information("Notification: Type={Type}, Message={Message}, Recipient={Recipient}", 
                type, message, recipient);
        }

        public void LogDialog(string dialogType, string action, string details = "")
        {
            _logger.Information("Dialog: Type={DialogType}, Action={Action}, Details={Details}", 
                dialogType, action, details);
        }

        public void LogFinancialTransaction(string transactionType, decimal amount, string accountId, string details = "")
        {
            _logger.Information("Financial Transaction: Type={TransactionType}, Amount={Amount}, Account={AccountId}, Details={Details}", 
                transactionType, amount, accountId, details);
        }

        public void LogSystemEvent(string eventType, string details = "")
        {
            _logger.Information("System Event: {EventType} - {Details}", eventType, details);
        }

        public void LogPerformance(string operation, TimeSpan duration, string details = "")
        {
            _logger.Information("Performance: {Operation} took {Duration}ms - {Details}", 
                operation, duration.TotalMilliseconds, details);
        }
    }
}