using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pilgrims.PersonalFinances.Core.Messaging.Interfaces;
using Pilgrims.PersonalFinances.Data;
using Pilgrims.PersonalFinances.Models;
using Pilgrims.PersonalFinances.Services.Interfaces;

namespace Pilgrims.PersonalFinances.Demo
{
    /// <summary>
    /// Demonstration of the audit trail functionality
    /// </summary>
    public class AuditTrailDemo
    {
        private readonly IAuditService _auditService;
        private readonly PersonalFinanceContext _context;
        private readonly ILogger<AuditTrailDemo> _logger;

        public AuditTrailDemo(IAuditService auditService, PersonalFinanceContext context, ILogger<AuditTrailDemo> logger)
        {
            _auditService = auditService;
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Demonstrates creating an account and viewing the audit trail
        /// </summary>
        public async Task DemonstrateAccountAuditAsync()
        {
            try
            {
                _logger.LogInformation("Starting audit trail demonstration...");

                // Create a new account (this will trigger the audit interceptor)
                var account = new Account
                {
                    Name = "Demo Checking Account",
                    AccountType = Models.Enums.AccountType.Checking,
                    Balance = 1000.00m,
                    Currency = "USD",
                    IsActive = true
                };

                _context.Accounts.Add(account);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created account: {AccountId}", account.Id);

                // Update the account (this will also trigger the audit interceptor)
                account.Balance = 1500.00m;
                account.Name = "Demo Checking Account - Updated";
                await _context.SaveChangesAsync();

                _logger.LogInformation("Updated account: {AccountId}", account.Id);

                // Manually log a report view operation
                await _auditService.LogReportViewedAsync(
                    userId: "demo-user-123",
                    reportType: "Account Balance Report",
                    parameters: new Dictionary<string, object>
                    {
                        { "AccountId", account.Id },
                        { "DateRange", "Last 30 days" }
                    },
                    description: "User viewed account balance report",
                    ipAddress: "192.168.1.100",
                    userAgent: "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36"
                );

                _logger.LogInformation("Logged report view operation");

                // Retrieve audit logs for the account
                var auditLogs = await _auditService.GetAuditLogsForEntityAsync("Account", account.Id);
                
                _logger.LogInformation("Retrieved {Count} audit log entries for account {AccountId}", 
                    auditLogs.Count(), account.Id);

                foreach (var log in auditLogs)
                {
                    _logger.LogInformation("Audit Log: {Operation} on {EntityName} at {Timestamp}", 
                        log.Operation, log.EntityName, log.Timestamp);
                }

                // Retrieve audit logs for the user
                var userAuditLogs = await _auditService.GetAuditLogsForUserAsync("demo-user-123");
                
                _logger.LogInformation("Retrieved {Count} audit log entries for user demo-user-123", 
                    userAuditLogs.Count());

                _logger.LogInformation("Audit trail demonstration completed successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during audit trail demonstration");
                throw;
            }
        }

        /// <summary>
        /// Demonstrates report audit operations
        /// </summary>
        public async Task DemonstrateReportAuditAsync()
        {
            try
            {
                _logger.LogInformation("Starting report audit demonstration...");

                var userId = "demo-user-456";

                // Log report printed operation
                await _auditService.LogReportPrintedAsync(
                    userId: userId,
                    reportType: "Monthly Budget Report",
                    parameters: new Dictionary<string, object>
                    {
                        { "Month", "December 2024" },
                        { "IncludeCharts", true }
                    },
                    printerName: "HP LaserJet Pro",
                    description: "User printed monthly budget report",
                    ipAddress: "192.168.1.101"
                );

                // Log report exported operation
                await _auditService.LogReportExportedAsync(
                    userId: userId,
                    reportType: "Transaction History Report",
                    exportFormat: "PDF",
                    filePath: "C:\\Reports\\TransactionHistory_2024.pdf",
                    parameters: new Dictionary<string, object>
                    {
                        { "StartDate", "2024-01-01" },
                        { "EndDate", "2024-12-31" },
                        { "Categories", new[] { "Food", "Transportation", "Entertainment" } }
                    },
                    description: "User exported annual transaction history"
                );

                _logger.LogInformation("Report audit operations completed successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during report audit demonstration");
                throw;
            }
        }
    }
}