using Serilog;
using Serilog.Events;
using System.IO;

namespace Pilgrims.PersonalFinances.Core.Logging
{
    public static class LoggingConfiguration
    {
        private static readonly string LogsDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "PilgrimsPersonalFinances",
            "Logs"
        );

        private static readonly string LogDatabasePath = Path.Combine(LogsDirectory, "application.db");

        public static ILogger CreateLogger()
        {
            // Ensure logs directory exists
            Directory.CreateDirectory(LogsDirectory);

            var config = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("System", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "Pilgrims Personal Finances")
                .Enrich.WithProperty("Version", GetApplicationVersion());

#if ANDROID
            // On Android, avoid SQLite sink due to native interop packaging issues; use rolling file sink instead
            var logger = config
                .WriteTo.File(
                    path: Path.Combine(LogsDirectory, "log-.txt"),
                    rollingInterval: RollingInterval.Day,
                    restrictedToMinimumLevel: LogEventLevel.Verbose)
                .CreateLogger();
#else
            // On non-Android platforms, use SQLite sink for structured log storage
            var logger = config
                .WriteTo.SQLite(
                    sqliteDbPath: LogDatabasePath,
                    tableName: "Logs",
                    restrictedToMinimumLevel: LogEventLevel.Verbose,
                    formatProvider: null,
                    storeTimestampInUtc: true)
                .CreateLogger();
#endif

            return logger;
        }

        public static string GetLogDatabasePath() => LogDatabasePath;

        public static string GetLogsDirectory() => LogsDirectory;

        private static string GetApplicationVersion()
        {
            try
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var version = assembly.GetName().Version;
                return version?.ToString() ?? "Unknown";
            }
            catch
            {
                return "Unknown";
            }
        }
    }
}