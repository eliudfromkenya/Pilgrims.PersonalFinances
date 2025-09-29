//using System.ComponentModel.DataAnnotations;

//namespace Pilgrims.PersonalFinances.Models.Enums
//{
//    /// <summary>
//    /// Types of financial reports available in the system
//    /// </summary>
//    public enum ReportType
//    {
//        [Display(Name = "Income Statement")]
//        IncomeStatement,
        
//        [Display(Name = "Balance Sheet")]
//        BalanceSheet,
        
//        [Display(Name = "Cash Flow Statement")]
//        CashFlowStatement,
        
//        [Display(Name = "Budget Variance Report")]
//        BudgetVarianceReport,
        
//        [Display(Name = "Net Worth Trend")]
//        NetWorthTrend,
        
//        [Display(Name = "Category Analysis")]
//        CategoryAnalysis,
        
//        [Display(Name = "Account Summary")]
//        AccountSummary,
        
//        [Display(Name = "Transaction Summary")]
//        TransactionSummary,
        
//        [Display(Name = "Debt Analysis")]
//        DebtAnalysis,
        
//        [Display(Name = "Investment Performance")]
//        InvestmentPerformance,
        
//        [Display(Name = "Custom Report")]
//        CustomReport
//    }

//    /// <summary>
//    /// Chart types available for report visualization
//    /// </summary>
//    public enum ChartType
//    {
//        [Display(Name = "Bar Chart")]
//        BarChart,
        
//        [Display(Name = "Line Chart")]
//        LineChart,
        
//        [Display(Name = "Pie Chart")]
//        PieChart,
        
//        [Display(Name = "Area Chart")]
//        AreaChart,
        
//        [Display(Name = "Donut Chart")]
//        DonutChart,
        
//        [Display(Name = "Stacked Bar Chart")]
//        StackedBarChart,
        
//        [Display(Name = "Trend Line")]
//        TrendLine,
        
//        [Display(Name = "Scatter Plot")]
//        ScatterPlot,
        
//        [Display(Name = "Waterfall Chart")]
//        WaterfallChart,
        
//        [Display(Name = "Gauge Chart")]
//        GaugeChart,
        
//        [Display(Name = "Table")]
//        Table
//    }

//    /// <summary>
//    /// Export formats available for reports
//    /// </summary>
//    public enum ExportFormat
//    {
//        [Display(Name = "PDF")]
//        PDF,
        
//        [Display(Name = "CSV")]
//        CSV,
        
//        [Display(Name = "Excel")]
//        Excel,
        
//        [Display(Name = "PNG Image")]
//        PNG,
        
//        [Display(Name = "JPEG Image")]
//        JPEG,
        
//        [Display(Name = "SVG")]
//        SVG,
        
//        [Display(Name = "JSON")]
//        JSON,
        
//        [Display(Name = "XML")]
//        XML
//    }

//    /// <summary>
//    /// Report generation status
//    /// </summary>
//    public enum ReportStatus
//    {
//        [Display(Name = "Pending")]
//        Pending,
        
//        [Display(Name = "Processing")]
//        Processing,
        
//        [Display(Name = "Generating")]
//        Generating,
        
//        [Display(Name = "Completed")]
//        Completed,
        
//        [Display(Name = "Failed")]
//        Failed,
        
//        [Display(Name = "Cancelled")]
//        Cancelled
//    }

//    /// <summary>
//    /// Date range presets for reports
//    /// </summary>
//    public enum DateRangePreset
//    {
//        [Display(Name = "Today")]
//        Today,
        
//        [Display(Name = "Yesterday")]
//        Yesterday,
        
//        [Display(Name = "This Week")]
//        ThisWeek,
        
//        [Display(Name = "Last Week")]
//        LastWeek,
        
//        [Display(Name = "This Month")]
//        ThisMonth,
        
//        [Display(Name = "Last Month")]
//        LastMonth,
        
//        [Display(Name = "This Quarter")]
//        ThisQuarter,
        
//        [Display(Name = "Last Quarter")]
//        LastQuarter,
        
//        [Display(Name = "This Year")]
//        ThisYear,
        
//        [Display(Name = "Last Year")]
//        LastYear,
        
//        [Display(Name = "Year to Date")]
//        YearToDate,
        
//        [Display(Name = "Last 30 Days")]
//        Last30Days,
        
//        [Display(Name = "Last 90 Days")]
//        Last90Days,
        
//        [Display(Name = "Last 12 Months")]
//        Last12Months,
        
//        [Display(Name = "Custom Range")]
//        CustomRange
//    }

//    /// <summary>
//    /// Comparison period types for reports
//    /// </summary>
//    public enum ComparisonPeriod
//    {
//        [Display(Name = "None")]
//        None,
        
//        [Display(Name = "Previous Period")]
//        PreviousPeriod,
        
//        [Display(Name = "Same Period Last Year")]
//        SamePeriodLastYear,
        
//        [Display(Name = "Year over Year")]
//        YearOverYear,
        
//        [Display(Name = "Month over Month")]
//        MonthOverMonth,
        
//        [Display(Name = "Quarter over Quarter")]
//        QuarterOverQuarter
//    }

//    /// <summary>
//    /// Report parameter data types
//    /// </summary>
//    public enum ParameterType
//    {
//        [Display(Name = "Text")]
//        Text,
        
//        [Display(Name = "Number")]
//        Number,
        
//        [Display(Name = "Date")]
//        Date,
        
//        [Display(Name = "Date Range")]
//        DateRange,
        
//        [Display(Name = "Boolean")]
//        Boolean,
        
//        [Display(Name = "Dropdown")]
//        Dropdown,
        
//        [Display(Name = "Multi-Select")]
//        MultiSelect,
        
//        [Display(Name = "Account")]
//        Account,
        
//        [Display(Name = "Category")]
//        Category,
        
//        [Display(Name = "Budget")]
//        Budget
//    }

//    /// <summary>
//    /// Report frequency for scheduled reports
//    /// </summary>
//    public enum ReportFrequency
//    {
//        [Display(Name = "One Time")]
//        OneTime,
        
//        [Display(Name = "Daily")]
//        Daily,
        
//        [Display(Name = "Weekly")]
//        Weekly,
        
//        [Display(Name = "Monthly")]
//        Monthly,
        
//        [Display(Name = "Quarterly")]
//        Quarterly,
        
//        [Display(Name = "Yearly")]
//        Yearly
//    }
//}