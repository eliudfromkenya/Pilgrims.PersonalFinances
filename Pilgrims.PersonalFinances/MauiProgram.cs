using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Pilgrims.PersonalFinances.Data;
using Pilgrims.PersonalFinances.Services;
using Pilgrims.PersonalFinances.Services.Interfaces;

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

            // Add Entity Framework with encrypted SQLite
            builder.Services.AddDbContext<PersonalFinanceContext>(options =>
            {
                var dbPath = Path.Combine(FileSystem.AppDataDirectory, "PersonalFinance.db");
                var connectionString = $"Data Source={dbPath};";
                options.UseSqlite(connectionString);
            });

            // Add Services
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
            builder.Services.AddSingleton<IScheduledTransactionBackgroundService, ScheduledTransactionBackgroundService>();
            builder.Services.AddHostedService<ScheduledTransactionBackgroundService>();
            builder.Services.AddHostedService<InsuranceNotificationBackgroundService>();

#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
