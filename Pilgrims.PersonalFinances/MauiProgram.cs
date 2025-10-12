using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Pilgrims.PersonalFinances.Data;
using Pilgrims.PersonalFinances.Core.Services.Interfaces;
using Pilgrims.PersonalFinances.Core.Localization.Interfaces;
using Pilgrims.PersonalFinances.Core.Localization.Services;
using CommunityToolkit.Mvvm.Messaging;
using Pilgrims.PersonalFinances.Core.Messaging.Interfaces;
using Pilgrims.PersonalFinances.Core.Messaging.Services;
using Pilgrims.PersonalFinances.Core.ViewModels;
using Pilgrims.PersonalFinances.Core.Logging;
using Serilog;
using Pilgrims.PersonalFinances.Core.Interfaces;
using Pilgrims.PersonalFinances.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Pilgrims.PersonalFinances.Services;

namespace Pilgrims.PersonalFinances
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();
            // Ensure localization services are available for IStringLocalizer<T>
            builder.Services.AddLocalization();

            // Add Entity Framework with encrypted SQLite
            builder.Services.AddDbContext<PersonalFinanceContext>(options =>
            {
                var dbPath = Path.Combine(FileSystem.AppDataDirectory, "PersonalFinance.db");
                var connectionString = $"Data Source={dbPath};";
                options.UseSqlite(connectionString, b => b.MigrationsAssembly("Pilgrims.PersonalFinances.Core"));
            });

            // Messaging & Logging
            builder.Services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);
            builder.Services.AddSingleton<IMessagingService, MessagingService>();
            builder.Services.AddSingleton<NotificationHandler>();
            builder.Services.AddSingleton<Serilog.ILogger>(sp => new LoggerConfiguration().CreateLogger());
            builder.Services.AddSingleton<ILoggingService, LoggingService>();

            // Add Services
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
            builder.Services.AddScoped<IThemeService, ThemeService>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<ICurrencyService, CurrencyService>();
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
            builder.Services.AddScoped<ILocalizationService, LocalizationService>();
            builder.Services.AddScoped<IAccountTypeService, AccountTypeService>();
            // ViewModels
            builder.Services.AddScoped<NotificationBellViewModel>();

            builder.Services.AddSingleton<IScheduledTransactionBackgroundService, ScheduledTransactionBackgroundService>();
            builder.Services.AddHostedService<ScheduledTransactionBackgroundService>();
            builder.Services.AddHostedService<InsuranceNotificationBackgroundService>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            // Build app and apply migrations at startup
            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<PersonalFinanceContext>();
                db.Database.Migrate();
            }

            return app;
        }
    }
}
