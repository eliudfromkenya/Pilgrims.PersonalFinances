using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pilgrims.PersonalFinances.Core.Interfaces;
using Pilgrims.PersonalFinances.Core.Logging;
using Pilgrims.PersonalFinances.Core.Messaging.Interfaces;
using Pilgrims.PersonalFinances.Core.Messaging.Services;
using Pilgrims.PersonalFinances.Core.Services;
using Pilgrims.PersonalFinances.Core.Services.Interfaces;
using Pilgrims.PersonalFinances.Core.ViewModels;
using Pilgrims.PersonalFinances.Core.Interceptors;
using Pilgrims.PersonalFinances.Data;
using Pilgrims.PersonalFinances.Services;
using Pilgrims.PersonalFinances.Services.Interfaces;
using Serilog;

namespace Pilgrims.PersonalFinances
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            // Configure Serilog
            var logger = LoggingConfiguration.CreateLogger();
            Log.Logger = logger;

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            // Configure logging
            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog(logger);

            builder.Services.AddMauiBlazorWebView();

            // Add Audit Services
            builder.Services.AddScoped<AuditInterceptor>();
            builder.Services.AddScoped<IAuditService, AuditService>();

            // Add Entity Framework with encrypted SQLite
            builder.Services.AddDbContext<PersonalFinanceContext>((serviceProvider, options) =>
            {
                var dbPath = Path.Combine(FileSystem.AppDataDirectory, "PersonalFinance.db");
                
                // For now, use standard SQLite connection
                // TODO: Integrate with DatabaseEncryptionService for SQLCipher support
                var connectionString = $"Data Source={dbPath};";
                options.UseSqlite(connectionString);
                
                // Add audit interceptor
                var auditInterceptor = serviceProvider.GetService<AuditInterceptor>();
                if (auditInterceptor != null)
                {
                    options.AddInterceptors(auditInterceptor);
                }
            });

            // Add Services
            builder.Services.AddSingleton<Serilog.ILogger>(logger);
            builder.Services.AddScoped<ILoggingService, LoggingService>();
            builder.Services.AddScoped<IDatabaseEncryptionService, DatabaseEncryptionService>();
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
            builder.Services.AddScoped<IThemeService, ThemeService>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<ITransactionService, TransactionService>();
            builder.Services.AddScoped<IBudgetService, BudgetService>();
            builder.Services.AddScoped<IWindowService, WindowService>();
            builder.Services.AddScoped<IScheduledTransactionService, ScheduledTransactionService>();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddScoped<IDebtService, DebtService>();
            builder.Services.AddScoped<IIncomeService, IncomeService>();
            builder.Services.AddScoped<IAssetService, AssetService>();
            builder.Services.AddScoped<IFileService, FileService>();
            builder.Services.AddScoped<IReconciliationService, ReconciliationService>();
            builder.Services.AddScoped<IInsuranceService, InsuranceService>();
            builder.Services.AddScoped<IReportService, ReportService>();
            builder.Services.AddScoped<IExportService, ExportService>();
            builder.Services.AddScoped<IComparisonService, ComparisonService>();
            builder.Services.AddScoped<IObligationService, ObligationService>();
            builder.Services.AddScoped<ICurrencyService, CurrencyService>();
            builder.Services.AddScoped<IApplicationSettingsService, ApplicationSettingsService>();
            builder.Services.AddSingleton<IScheduledTransactionBackgroundService, ScheduledTransactionBackgroundService>();
            builder.Services.AddHostedService<ScheduledTransactionBackgroundService>();
            builder.Services.AddHostedService<InsuranceNotificationBackgroundService>();

            // Security Services
            builder.Services.AddScoped<IBiometricAuthenticationService, BiometricAuthenticationService>();
            builder.Services.AddSingleton<IAutoLockService, AutoLockService>();
            builder.Services.AddScoped<IScreenshotProtectionService, ScreenshotProtectionService>();
            builder.Services.AddSingleton<IAppSwitcherPrivacyService, AppSwitcherPrivacyService>();

            // MVVM Messaging Services
        builder.Services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);
        builder.Services.AddScoped<IMessagingService, MessagingService>();
        builder.Services.AddScoped<NotificationHandler>();
        
        // ViewModels
        builder.Services.AddScoped<NotificationBellViewModel>();
        builder.Services.AddScoped<NotificationsViewModel>();
        builder.Services.AddScoped<ReportsViewModel>();

#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
