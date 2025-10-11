using Microsoft.Extensions.Logging;

namespace Pilgrims.PersonalFinances.Core.Logging
{
    public interface ILoggingService
    {
        void LogTrace(string message, params object[] args);
        void LogDebug(string message, params object[] args);
        void LogInformation(string message, params object[] args);
        void LogWarning(string message, params object[] args);
        void LogError(string message, params object[] args);
        void LogError(Exception exception, string message, params object[] args);
        void LogCritical(string message, params object[] args);
        void LogCritical(Exception exception, string message, params object[] args);

        // Specific logging methods for different application areas
        void LogUserAction(string action, string details = "");
        void LogDatabaseOperation(string operation, string table, string details = "");
        void LogNotification(string type, string message, string recipient = "");
        void LogDialog(string dialogType, string action, string details = "");
        void LogFinancialTransaction(string transactionType, decimal amount, string accountId, string details = "");
        void LogSystemEvent(string eventType, string details = "");
        void LogPerformance(string operation, TimeSpan duration, string details = "");
    }
}