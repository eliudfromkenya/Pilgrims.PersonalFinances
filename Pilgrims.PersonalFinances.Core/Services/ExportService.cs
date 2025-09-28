using System.Text;
using System.Text.Json;
using Microsoft.JSInterop;
using Pilgrims.PersonalFinances.Models.DTOs;

namespace Pilgrims.PersonalFinances.Services;

public class ExportService : IExportService
{
    private readonly IJSRuntime _jsRuntime;

    public ExportService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task ExportToPdfAsync(string reportType, object reportData, string fileName)
    {
        try
        {
            var htmlContent = await GenerateReportHtmlAsync(reportType, reportData);
            var fullFileName = $"{fileName}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            await _jsRuntime.InvokeVoidAsync("exportToPdf", htmlContent, fullFileName);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error exporting to PDF: {ex.Message}");
            throw;
        }
    }

    public async Task ExportToCsvAsync(string reportType, object reportData, string fileName)
    {
        try
        {
            var csvContent = ConvertToCsv(reportData);
            var fullFileName = $"{fileName}_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            await _jsRuntime.InvokeVoidAsync("downloadFile", csvContent, fullFileName, "text/csv");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error exporting to CSV: {ex.Message}");
            throw;
        }
    }

    public async Task ExportToImageAsync(string elementId, string fileName)
    {
        try
        {
            var fullFileName = $"{fileName}_{DateTime.Now:yyyyMMdd_HHmmss}.png";
            await _jsRuntime.InvokeVoidAsync("exportToImage", elementId, fullFileName);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error exporting to image: {ex.Message}");
            throw;
        }
    }

    public async Task<string> GenerateReportHtmlAsync(string reportType, object reportData)
    {
        return reportType switch
        {
            "IncomeStatement" => GenerateIncomeStatementHtml((IncomeExpenseReportDto)reportData),
            "BalanceSheet" => GenerateBalanceSheetHtml((BalanceSheetDto)reportData),
            "CashFlow" => GenerateCashFlowHtml((CashFlowReportDto)reportData),
            "BudgetVariance" => GenerateBudgetVarianceHtml((BudgetVarianceReportDto)reportData),
            "NetWorthTrend" => GenerateNetWorthTrendHtml(reportData as NetWorthTrendDto ?? new NetWorthTrendDto()),
            "CategoryAnalysis" => GenerateCategoryAnalysisHtml((CategoryAnalysisDto)reportData),
            "AccountSummary" => GenerateAccountSummaryHtml((AccountSummaryReportDto)reportData),
            _ => "<html><body><h1>Report not supported</h1></body></html>"
        };
    }

    private string ConvertToCsv(object data)
    {
        var csv = new StringBuilder();

        switch (data)
        {
            case IncomeExpenseReportDto incomeReport:
                csv.AppendLine("Income Statement Report");
                csv.AppendLine($"Period: {incomeReport.StartDate:yyyy-MM-dd} to {incomeReport.EndDate:yyyy-MM-dd}");
                csv.AppendLine();
                
                csv.AppendLine("Income Categories");
                csv.AppendLine("Category,Amount,Percentage");
                foreach (var category in incomeReport.IncomeByCategory)
                {
                    csv.AppendLine($"{category.Name},{category.Amount},{category.Percentage:P2}");
                }
                
                csv.AppendLine();
                csv.AppendLine("Expense Categories");
                csv.AppendLine("Category,Amount,Percentage");
                foreach (var category in incomeReport.ExpensesByCategory)
                {
                    csv.AppendLine($"{category.Name},{category.Amount},{category.Percentage:P2}");
                }
                
                csv.AppendLine();
                csv.AppendLine("Summary");
                csv.AppendLine($"Total Income,{incomeReport.TotalIncome}");
                csv.AppendLine($"Total Expenses,{incomeReport.TotalExpenses}");
                csv.AppendLine($"Net Income,{incomeReport.NetIncome}");
                break;

            case BalanceSheetDto balanceSheet:
                csv.AppendLine("Balance Sheet Report");
                csv.AppendLine($"As of: {balanceSheet.AsOfDate:yyyy-MM-dd}");
                csv.AppendLine();
                
                csv.AppendLine("Assets");
                csv.AppendLine("Account,Amount");
                foreach (var asset in balanceSheet.Assets)
                {
                    csv.AppendLine($"{asset.Name},{asset.Amount}");
                }
                
                csv.AppendLine();
                csv.AppendLine("Liabilities");
                csv.AppendLine("Account,Amount");
                foreach (var liability in balanceSheet.Liabilities)
                {
                    csv.AppendLine($"{liability.Name},{liability.Amount}");
                }
                
                csv.AppendLine();
                csv.AppendLine("Summary");
                csv.AppendLine($"Total Assets,{balanceSheet.TotalAssets}");
                csv.AppendLine($"Total Liabilities,{balanceSheet.TotalLiabilities}");
                csv.AppendLine($"Net Worth,{balanceSheet.NetWorth}");
                break;

            case CategoryAnalysisDto categoryAnalysis:
                csv.AppendLine("Category Analysis Report");
                csv.AppendLine($"Period: {categoryAnalysis.StartDate:yyyy-MM-dd} to {categoryAnalysis.EndDate:yyyy-MM-dd}");
                csv.AppendLine();
                
                csv.AppendLine("Categories");
                csv.AppendLine("Category,Amount,Percentage,Transaction Count");
                foreach (var category in categoryAnalysis.Categories)
                {
                    csv.AppendLine($"{category.Name},{category.Amount},{category.Percentage:P2},{category.TransactionCount}");
                }
                
                csv.AppendLine();
                csv.AppendLine($"Total Spending,{categoryAnalysis.TotalSpending}");
                break;

            case AccountSummaryReportDto accountSummary:
                csv.AppendLine("Account Summary Report");
                csv.AppendLine($"Period: {accountSummary.StartDate:yyyy-MM-dd} to {accountSummary.EndDate:yyyy-MM-dd}");
                csv.AppendLine();
                
                csv.AppendLine("Accounts");
                csv.AppendLine("Account Name,Account Type,Balance,Monthly Change,Transaction Count,Status");
                foreach (var account in accountSummary.Accounts)
                {
                    csv.AppendLine($"{account.Name},{account.AccountType},{account.Balance},{account.MonthlyChange},{account.TransactionCount},{(account.IsActive ? "Active" : "Inactive")}");
                }
                
                csv.AppendLine();
                csv.AppendLine($"Total Balance,{accountSummary.TotalBalance}");
                break;

            default:
                csv.AppendLine("Data export not supported for this report type");
                break;
        }

        return csv.ToString();
    }

    private string GenerateIncomeStatementHtml(IncomeExpenseReportDto data)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <title>Income Statement Report</title>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 20px; }}
        .header {{ text-align: center; margin-bottom: 30px; }}
        .period {{ color: #666; margin-bottom: 20px; }}
        table {{ width: 100%; border-collapse: collapse; margin-bottom: 20px; }}
        th, td {{ padding: 8px; text-align: left; border-bottom: 1px solid #ddd; }}
        th {{ background-color: #f5f5f5; font-weight: bold; }}
        .amount {{ text-align: right; }}
        .total {{ font-weight: bold; border-top: 2px solid #333; }}
        .section {{ margin-bottom: 30px; }}
    </style>
</head>
<body>
    <div class='header'>
        <h1>Income Statement</h1>
        <div class='period'>Period: {data.StartDate:MMMM dd, yyyy} - {data.EndDate:MMMM dd, yyyy}</div>
    </div>
    
    <div class='section'>
        <h2>Income</h2>
        <table>
            <thead>
                <tr><th>Category</th><th class='amount'>Amount</th><th class='amount'>Percentage</th></tr>
            </thead>
            <tbody>
                {string.Join("", data.IncomeByCategory.Select(c => $"<tr><td>{c.Name}</td><td class='amount'>{c.Amount:C}</td><td class='amount'>{c.Percentage:P1}</td></tr>"))}
                <tr class='total'><td>Total Income</td><td class='amount'>{data.TotalIncome:C}</td><td class='amount'>100.0%</td></tr>
            </tbody>
        </table>
    </div>
    
    <div class='section'>
        <h2>Expenses</h2>
        <table>
            <thead>
                <tr><th>Category</th><th class='amount'>Amount</th><th class='amount'>Percentage</th></tr>
            </thead>
            <tbody>
                {string.Join("", data.ExpensesByCategory.Select(c => $"<tr><td>{c.Name}</td><td class='amount'>{c.Amount:C}</td><td class='amount'>{c.Percentage:P1}</td></tr>"))}
                <tr class='total'><td>Total Expenses</td><td class='amount'>{data.TotalExpenses:C}</td><td class='amount'>100.0%</td></tr>
            </tbody>
        </table>
    </div>
    
    <div class='section'>
        <h2>Summary</h2>
        <table>
            <tr><td>Total Income</td><td class='amount'>{data.TotalIncome:C}</td></tr>
            <tr><td>Total Expenses</td><td class='amount'>{data.TotalExpenses:C}</td></tr>
            <tr class='total'><td>Net Income</td><td class='amount'>{data.NetIncome:C}</td></tr>
        </table>
    </div>
</body>
</html>";
    }

    private string GenerateBalanceSheetHtml(BalanceSheetDto data)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <title>Balance Sheet Report</title>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 20px; }}
        .header {{ text-align: center; margin-bottom: 30px; }}
        .date {{ color: #666; margin-bottom: 20px; }}
        table {{ width: 100%; border-collapse: collapse; margin-bottom: 20px; }}
        th, td {{ padding: 8px; text-align: left; border-bottom: 1px solid #ddd; }}
        th {{ background-color: #f5f5f5; font-weight: bold; }}
        .amount {{ text-align: right; }}
        .total {{ font-weight: bold; border-top: 2px solid #333; }}
        .section {{ margin-bottom: 30px; }}
    </style>
</head>
<body>
    <div class='header'>
        <h1>Balance Sheet</h1>
        <div class='date'>As of: {data.AsOfDate:MMMM dd, yyyy}</div>
    </div>
    
    <div class='section'>
        <h2>Assets</h2>
        <table>
            <thead>
                <tr><th>Account</th><th class='amount'>Amount</th></tr>
            </thead>
            <tbody>
                {string.Join("", data.Assets.Select(a => $"<tr><td>{a.Name}</td><td class='amount'>{a.Amount:C}</td></tr>"))}
                <tr class='total'><td>Total Assets</td><td class='amount'>{data.TotalAssets:C}</td></tr>
            </tbody>
        </table>
    </div>
    
    <div class='section'>
        <h2>Liabilities</h2>
        <table>
            <thead>
                <tr><th>Account</th><th class='amount'>Amount</th></tr>
            </thead>
            <tbody>
                {string.Join("", data.Liabilities.Select(l => $"<tr><td>{l.Name}</td><td class='amount'>{l.Amount:C}</td></tr>"))}
                <tr class='total'><td>Total Liabilities</td><td class='amount'>{data.TotalLiabilities:C}</td></tr>
            </tbody>
        </table>
    </div>
    
    <div class='section'>
        <h2>Net Worth</h2>
        <table>
            <tr><td>Total Assets</td><td class='amount'>{data.TotalAssets:C}</td></tr>
            <tr><td>Total Liabilities</td><td class='amount'>{data.TotalLiabilities:C}</td></tr>
            <tr class='total'><td>Net Worth</td><td class='amount'>{data.NetWorth:C}</td></tr>
        </table>
    </div>
</body>
</html>";
    }

    private string GenerateCashFlowHtml(CashFlowReportDto data)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <title>Cash Flow Statement</title>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 20px; }}
        .header {{ text-align: center; margin-bottom: 30px; }}
        .period {{ color: #666; margin-bottom: 20px; }}
        table {{ width: 100%; border-collapse: collapse; margin-bottom: 20px; }}
        th, td {{ padding: 8px; text-align: left; border-bottom: 1px solid #ddd; }}
        th {{ background-color: #f5f5f5; font-weight: bold; }}
        .amount {{ text-align: right; }}
        .total {{ font-weight: bold; border-top: 2px solid #333; }}
        .section {{ margin-bottom: 30px; }}
    </style>
</head>
<body>
    <div class='header'>
        <h1>Cash Flow Statement</h1>
        <div class='period'>Period: {data.StartDate:MMMM dd, yyyy} - {data.EndDate:MMMM dd, yyyy}</div>
    </div>
    
    <div class='section'>
        <h2>Summary</h2>
        <table>
            <tr><td>Operating Cash Flow</td><td class='amount'>{data.OperatingCashFlow:C}</td></tr>
            <tr><td>Investing Cash Flow</td><td class='amount'>{data.InvestingCashFlow:C}</td></tr>
            <tr><td>Financing Cash Flow</td><td class='amount'>{data.FinancingCashFlow:C}</td></tr>
            <tr class='total'><td>Net Cash Flow</td><td class='amount'>{data.NetCashFlow:C}</td></tr>
        </table>
    </div>
</body>
</html>";
    }

    private string GenerateBudgetVarianceHtml(BudgetVarianceReportDto data)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <title>Budget Variance Report</title>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 20px; }}
        .header {{ text-align: center; margin-bottom: 30px; }}
        .period {{ color: #666; margin-bottom: 20px; }}
        table {{ width: 100%; border-collapse: collapse; margin-bottom: 20px; }}
        th, td {{ padding: 8px; text-align: left; border-bottom: 1px solid #ddd; }}
        th {{ background-color: #f5f5f5; font-weight: bold; }}
        .amount {{ text-align: right; }}
        .total {{ font-weight: bold; border-top: 2px solid #333; }}
        .section {{ margin-bottom: 30px; }}
        .positive {{ color: green; }}
        .negative {{ color: red; }}
    </style>
</head>
<body>
    <div class='header'>
        <h1>Budget Variance Report</h1>
        <div class='period'>Period: {data.StartDate:MMMM dd, yyyy} - {data.EndDate:MMMM dd, yyyy}</div>
    </div>
    
    <div class='section'>
        <h2>Summary</h2>
        <table>
            <tr><td>Total Budgeted</td><td class='amount'>{data.TotalBudgetedAmount:C}</td></tr>
            <tr><td>Total Actual</td><td class='amount'>{data.TotalActualAmount:C}</td></tr>
            <tr class='total'><td>Total Variance</td><td class='amount {(data.TotalVariance >= 0 ? "positive" : "negative")}'>{data.TotalVariance:C}</td></tr>
        </table>
    </div>
</body>
</html>";
    }

    private string GenerateNetWorthTrendHtml(NetWorthTrendDto data)
    {
        // Ensure data is not null
        if (data == null)
        {
            data = new NetWorthTrendDto();
        }

        return $@"
<!DOCTYPE html>
<html>
<head>
    <title>Net Worth Trend Report</title>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 20px; }}
        .header {{ text-align: center; margin-bottom: 30px; }}
        .period {{ font-size: 14px; color: #666; }}
        .section {{ margin-bottom: 30px; }}
        table {{ width: 100%; border-collapse: collapse; }}
        th, td {{ padding: 8px; text-align: left; border-bottom: 1px solid #ddd; }}
        .amount {{ text-align: right; }}
        .total {{ font-weight: bold; }}
        .positive {{ color: green; }}
        .negative {{ color: red; }}
    </style>
</head>
<body>
    <div class='header'>
        <h1>Net Worth Trend</h1>
        <div class='period'>Period: {data.StartDate:MMMM dd, yyyy} - {data.EndDate:MMMM dd, yyyy}</div>
    </div>
    
    <div class='section'>
        <h2>Summary</h2>
        <table>
            <tr><td>Starting Net Worth</td><td class='amount'>{(data.DataPoints?.FirstOrDefault()?.NetWorth ?? 0):C}</td></tr>
            <tr><td>Ending Net Worth</td><td class='amount'>{(data.DataPoints?.LastOrDefault()?.NetWorth ?? 0):C}</td></tr>
            <tr class='total'><td>Net Change</td><td class='amount'>{data.NetWorthChange:C}</td></tr>
            <tr><td>Growth Rate</td><td class='amount'>{data.ChangePercentage:P2}</td></tr>
        </table>
    </div>
</body>
</html>";
    }

    private string GenerateCategoryAnalysisHtml(CategoryAnalysisDto data)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <title>Category Analysis Report</title>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 20px; }}
        .header {{ text-align: center; margin-bottom: 30px; }}
        .period {{ color: #666; margin-bottom: 20px; }}
        table {{ width: 100%; border-collapse: collapse; margin-bottom: 20px; }}
        th, td {{ padding: 8px; text-align: left; border-bottom: 1px solid #ddd; }}
        th {{ background-color: #f5f5f5; font-weight: bold; }}
        .amount {{ text-align: right; }}
        .total {{ font-weight: bold; border-top: 2px solid #333; }}
        .section {{ margin-bottom: 30px; }}
    </style>
</head>
<body>
    <div class='header'>
        <h1>Category Analysis</h1>
        <div class='period'>Period: {data.StartDate:MMMM dd, yyyy} - {data.EndDate:MMMM dd, yyyy}</div>
    </div>
    
    <div class='section'>
        <h2>Categories</h2>
        <table>
            <thead>
                <tr><th>Category</th><th class='amount'>Amount</th><th class='amount'>Percentage</th><th class='amount'>Transactions</th></tr>
            </thead>
            <tbody>
                {string.Join("", data.Categories.Select(c => $"<tr><td>{c.Name}</td><td class='amount'>{c.Amount:C}</td><td class='amount'>{c.Percentage:P1}</td><td class='amount'>{c.TransactionCount}</td></tr>"))}
                <tr class='total'><td>Total</td><td class='amount'>{data.TotalSpending:C}</td><td class='amount'>100.0%</td><td class='amount'>{data.Categories.Sum(c => c.TransactionCount)}</td></tr>
            </tbody>
        </table>
    </div>
</body>
</html>";
    }

    private string GenerateAccountSummaryHtml(AccountSummaryReportDto data)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <title>Account Summary Report</title>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 20px; }}
        .header {{ text-align: center; margin-bottom: 30px; }}
        .period {{ color: #666; margin-bottom: 20px; }}
        table {{ width: 100%; border-collapse: collapse; margin-bottom: 20px; }}
        th, td {{ padding: 8px; text-align: left; border-bottom: 1px solid #ddd; }}
        th {{ background-color: #f5f5f5; font-weight: bold; }}
        .amount {{ text-align: right; }}
        .total {{ font-weight: bold; border-top: 2px solid #333; }}
        .section {{ margin-bottom: 30px; }}
    </style>
</head>
<body>
    <div class='header'>
        <h1>Account Summary</h1>
        <div class='period'>Period: {data.StartDate:MMMM dd, yyyy} - {data.EndDate:MMMM dd, yyyy}</div>
    </div>
    
    <div class='section'>
        <h2>Accounts</h2>
        <table>
            <thead>
                <tr><th>Account</th><th>Type</th><th class='amount'>Balance</th><th class='amount'>Monthly Change</th><th class='amount'>Transactions</th><th>Status</th></tr>
            </thead>
            <tbody>
                {string.Join("", data.Accounts.Select(a => $"<tr><td>{a.Name}</td><td>{a.AccountType}</td><td class='amount'>{a.Balance:C}</td><td class='amount'>{a.MonthlyChange:C}</td><td class='amount'>{a.TransactionCount}</td><td>{(a.IsActive ? "Active" : "Inactive")}</td></tr>"))}
                <tr class='total'><td colspan='2'>Total</td><td class='amount'>{data.TotalBalance:C}</td><td class='amount'>{data.Accounts.Sum(a => a.MonthlyChange):C}</td><td class='amount'>{data.Accounts.Sum(a => a.TransactionCount)}</td><td></td></tr>
            </tbody>
        </table>
    </div>
</body>
</html>";
    }
}