using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pilgrims.PersonalFinances.Core.Messaging.Interfaces;
using Pilgrims.PersonalFinances.Core.Messaging.Messages;
using Pilgrims.PersonalFinances.Services;
using Pilgrims.PersonalFinances.Services.Interfaces;
using Pilgrims.PersonalFinances.Models.DTOs;
using Pilgrims.PersonalFinances.Models.Enums;

namespace Pilgrims.PersonalFinances.Core.ViewModels
{
    /// <summary>
    /// ViewModel for the Reports page using MVVM pattern with messaging
    /// </summary>
    public partial class ReportsViewModel : BaseViewModel
    {
        private readonly IReportService _reportService;
        private readonly IExportService _exportService;
        private readonly IComparisonService _comparisonService;
        private readonly IAuthenticationService _authService;

        [ObservableProperty]
        private ReportType activeReportType = ReportType.IncomeStatement;

        [ObservableProperty]
        private bool showExportMenu = false;

        [ObservableProperty]
        private bool showComparison = false;

        [ObservableProperty]
        private string? currentUserId;

        [ObservableProperty]
        private DateTime startDate = DateTime.Now.AddMonths(-1);

        [ObservableProperty]
        private DateTime endDate = DateTime.Now;

        // Report Data Properties
        [ObservableProperty]
        private IncomeExpenseReportDto? incomeStatement;

        [ObservableProperty]
        private BalanceSheetDto? balanceSheet;

        [ObservableProperty]
        private BalanceSheetDto? previousBalanceSheet;

        [ObservableProperty]
        private CashFlowReportDto? cashFlowStatement;

        [ObservableProperty]
        private BudgetVarianceReportDto? budgetVariance;

        [ObservableProperty]
        private List<Pilgrims.PersonalFinances.Models.DTOs.NetWorthTrendDto>? netWorthTrend;

        [ObservableProperty]
        private Pilgrims.PersonalFinances.Models.DTOs.CategoryAnalysisDto? categoryAnalysis;

        [ObservableProperty]
        private Pilgrims.PersonalFinances.Models.DTOs.AccountSummaryReportDto? accountSummary;

        [ObservableProperty]
        private string selectedPeriod = "last12months";

        [ObservableProperty]
        private ComparisonResultDto? comparisonData;

        public ReportsViewModel(
            IMessagingService messagingService,
            IReportService reportService,
            IExportService exportService,
            IComparisonService comparisonService,
            IAuthenticationService authService) : base(messagingService)
        {
            _reportService = reportService ?? throw new ArgumentNullException(nameof(reportService));
            _exportService = exportService ?? throw new ArgumentNullException(nameof(exportService));
            _comparisonService = comparisonService ?? throw new ArgumentNullException(nameof(comparisonService));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));

            Title = "Reports";
        }

        /// <summary>
        /// Initialize the ViewModel
        /// </summary>
        [RelayCommand]
        public async Task InitializeAsync()
        {
            await ExecuteAsync(async () =>
            {
                await GetCurrentUserIdAsync();
                await LoadReportDataAsync();
            });
        }

        /// <summary>
        /// Get the current user ID from session
        /// </summary>
        private async Task GetCurrentUserIdAsync()
        {
            try
            {
                // TODO: This needs proper session token management
                // For now, we'll use a placeholder approach
                var sessionToken = "current_session"; // This should come from session management
                var user = await _authService.GetUserBySessionTokenAsync(sessionToken);
                CurrentUserId = user?.Id;
            }
            catch (Exception ex)
            {
                ShowErrorToast($"Failed to get user information: {ex.Message}");
            }
        }

        /// <summary>
        /// Load report data based on active report type
        /// </summary>
        [RelayCommand]
        public async Task LoadReportDataAsync()
        {
            if (string.IsNullOrEmpty(CurrentUserId))
            {
                ShowErrorToast("User not authenticated");
                return;
            }

            await ExecuteAsync(async () =>
            {
                switch (ActiveReportType)
                {
                    case ReportType.IncomeStatement:
                        IncomeStatement = await _reportService.GenerateIncomeStatementAsync(StartDate, EndDate);
                        break;
                    case ReportType.BalanceSheet:
                        BalanceSheet = await _reportService.GenerateBalanceSheetAsync(EndDate);
                        break;
                    case ReportType.CashFlowStatement:
                        CashFlowStatement = await _reportService.GenerateCashFlowStatementAsync(StartDate, EndDate);
                        break;
                    case ReportType.BudgetVarianceReport:
                        BudgetVariance = await _reportService.GenerateBudgetVarianceReportAsync(StartDate, EndDate);
                        break;
                    case ReportType.NetWorthTrend:
                        NetWorthTrend = await _reportService.GenerateNetWorthTrendAsync(StartDate, EndDate);
                        break;
                    case ReportType.CategoryAnalysis:
                        CategoryAnalysis = await _reportService.GenerateCategoryAnalysisAsync(StartDate, EndDate);
                        break;
                    case ReportType.AccountSummary:
                        AccountSummary = await _reportService.GenerateAccountSummaryReportAsync(StartDate, EndDate);
                        break;
                }
            });
        }

        /// <summary>
        /// Change the active report type
        /// </summary>
        [RelayCommand]
        public async Task ChangeReportTypeAsync(ReportType reportType)
        {
            ActiveReportType = reportType;
            await LoadReportDataAsync();
        }

        /// <summary>
        /// Set the active report type (synchronous version for UI binding)
        /// </summary>
        [RelayCommand]
        public async Task SetActiveReport(ReportType reportType)
        {
            await ChangeReportTypeAsync(reportType);
        }

        /// <summary>
        /// Export report in specified format
        /// </summary>
        [RelayCommand]
        public async Task ExportReportAsync(string format)
        {
            if (string.IsNullOrEmpty(CurrentUserId))
            {
                ShowErrorToast("User not authenticated");
                return;
            }

            await ExecuteAsync(async () =>
            {
                var reportData = GetCurrentReportData();
                if (reportData == null)
                {
                    ShowErrorToast("No report data to export");
                    return;
                }

                await _exportService.ExportToPdfAsync(ActiveReportType.ToString(), reportData, $"Report_{ActiveReportType}_{DateTime.Now:yyyyMMdd}");
                ShowSuccessToast($"Report exported as {format} successfully!");
                ShowExportMenu = false;
            });
        }

        /// <summary>
        /// Generate year-over-year comparison
        /// </summary>
        [RelayCommand]
        public async Task GenerateYearOverYearComparisonAsync()
        {
            if (string.IsNullOrEmpty(CurrentUserId))
            {
                ShowErrorToast("User not authenticated");
                return;
            }

            await ExecuteAsync(async () =>
            {
                var currentYear = EndDate.Year;
                var previousYear = currentYear - 1;
                
                ComparisonData = await _comparisonService.GetYearOverYearComparisonAsync(CurrentUserId, currentYear, previousYear);
                ShowComparison = true;
            });
        }

        /// <summary>
        /// Generate month-over-month comparison
        /// </summary>
        [RelayCommand]
        public async Task GenerateMonthOverMonthComparisonAsync()
        {
            if (string.IsNullOrEmpty(CurrentUserId))
            {
                ShowErrorToast("User not authenticated");
                return;
            }

            await ExecuteAsync(async () =>
            {
                var currentMonth = EndDate;
                var previousMonth = currentMonth.AddMonths(-1);
                
                ComparisonData = await _comparisonService.GetMonthOverMonthComparisonAsync(CurrentUserId, currentMonth, previousMonth);
                ShowComparison = true;
            });
        }

        /// <summary>
        /// Handle category drill-down
        /// </summary>
        [RelayCommand]
        public async Task CategoryClickAsync(string categoryId)
        {
            ShowInfoToast($"Drilling down into category: {categoryId}");
            // Navigate to detailed transaction view for the category
            Navigate($"/transactions?categoryId={categoryId}");
        }

        /// <summary>
        /// Handle account drill-down
        /// </summary>
        [RelayCommand]
        public async Task AccountClickAsync(string accountId)
        {
            ShowInfoToast($"Drilling down into account: {accountId}");
            // Navigate to detailed account view
            Navigate($"/accounts/{accountId}");
        }

        /// <summary>
        /// Toggle export menu visibility
        /// </summary>
        [RelayCommand]
        public void ToggleExportMenu()
        {
            ShowExportMenu = !ShowExportMenu;
        }

        /// <summary>
        /// Close comparison view
        /// </summary>
        [RelayCommand]
        public void CloseComparison()
        {
            ShowComparison = false;
            ComparisonData = null;
        }

        /// <summary>
        /// Get current report data based on active report type
        /// </summary>
        private object? GetCurrentReportData()
        {
            return ActiveReportType switch
            {
                ReportType.IncomeStatement => IncomeStatement,
                ReportType.BalanceSheet => BalanceSheet,
                ReportType.CashFlowStatement => CashFlowStatement,
                ReportType.BudgetVarianceReport => BudgetVariance,
                ReportType.NetWorthTrend => NetWorthTrend,
                ReportType.CategoryAnalysis => CategoryAnalysis,
                ReportType.AccountSummary => AccountSummary,
                _ => null
            };
        }

        [RelayCommand]
        private async Task UpdateComparisonMetric(string metric)
        {
            if (ComparisonData != null)
            {
                ShowToastNotification($"Metric changed to: {metric}", ToastType.Info);
                // Reload comparison data with new metric
                // Implementation would depend on the comparison service
            }
        }

        [RelayCommand]
        private async Task UpdateComparisonPeriod(string period)
        {
            if (ComparisonData != null)
            {
                ShowToastNotification($"Period changed to: {period}", ToastType.Info);
                // Reload comparison data with new period
                // Implementation would depend on the comparison service
            }
        }
    }
}