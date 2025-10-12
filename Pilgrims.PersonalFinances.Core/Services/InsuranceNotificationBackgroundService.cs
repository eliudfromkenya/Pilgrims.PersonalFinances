using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pilgrims.PersonalFinances.Core.Services.Extensions;
using Pilgrims.PersonalFinances.Core.Services.Interfaces;

namespace Pilgrims.PersonalFinances.Core.Services
{
    /// <summary>
    /// Background service for managing insurance-related notifications
    /// </summary>
    public class InsuranceNotificationBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<InsuranceNotificationBackgroundService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(6); // Check every 6 hours

        public InsuranceNotificationBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<InsuranceNotificationBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Insurance Notification Background Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessInsuranceNotifications();
                    await Task.Delay(_checkInterval, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Insurance Notification Background Service is stopping");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in Insurance Notification Background Service");
                    await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken); // Wait 30 minutes before retrying
                }
            }
        }

        private async Task ProcessInsuranceNotifications()
        {
            using var scope = _serviceProvider.CreateScope();
            var insuranceService = scope.ServiceProvider.GetRequiredService<IInsuranceService>();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

            try
            {
                _logger.LogInformation("Processing insurance notifications...");

                // Get all active policies
                var activePolicies = (await insuranceService.GetAllInsurancePoliciesAsync())
                    .Where(p => p.IsActive)
                    .ToList();

                // Process premium due notifications
                await ProcessPremiumDueNotifications(notificationService, activePolicies);

                // Process policy expiry notifications
                await ProcessPolicyExpiryNotifications(notificationService, activePolicies);

                // Process renewal reminder notifications
                await ProcessRenewalReminderNotifications(notificationService, activePolicies);

                // Process document expiry notifications
                await ProcessDocumentExpiryNotifications(notificationService, insuranceService, activePolicies);

                _logger.LogInformation($"Processed insurance notifications for {activePolicies.Count} active policies");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing insurance notifications");
            }
        }

        private async Task ProcessPremiumDueNotifications(INotificationService notificationService, List<Models.Insurance> policies)
        {
            try
            {
                var today = DateTime.Today;
                var notificationWindow = today.AddDays(7); // Notify 7 days in advance

                var policiesWithUpcomingPremiums = policies
                    .Where(p => p.NextPremiumDueDate.HasValue && 
                               p.NextPremiumDueDate.Value >= today && 
                               p.NextPremiumDueDate.Value <= notificationWindow)
                    .ToList();

                foreach (var policy in policiesWithUpcomingPremiums)
                {
                    var daysUntilDue = (policy.NextPremiumDueDate!.Value.Date - today).Days;
                    
                    // Send notifications at specific intervals: 7 days, 3 days, 1 day, and day of
                    if (daysUntilDue == 7 || daysUntilDue == 3 || daysUntilDue == 1 || daysUntilDue == 0)
                    {
                        await notificationService.CreatePremiumDueNotificationAsync(policy, policy.NextPremiumDueDate.Value);
                        _logger.LogInformation($"Created premium due notification for policy {policy.PolicyNumber} - {daysUntilDue} days until due");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing premium due notifications");
            }
        }

        private async Task ProcessPolicyExpiryNotifications(INotificationService notificationService, List<Models.Insurance> policies)
        {
            try
            {
                var today = DateTime.Today;
                var notificationWindow = today.AddDays(90); // Notify up to 90 days in advance

                var expiringPolicies = policies
                    .Where(p => p.PolicyEndDate >= today && p.PolicyEndDate <= notificationWindow)
                    .ToList();

                foreach (var policy in expiringPolicies)
                {
                    var daysUntilExpiry = (policy.PolicyEndDate?.Date - today)?.Days;
                    
                    // Send notifications at specific intervals: 90 days, 60 days, 30 days, 14 days, 7 days, 3 days, 1 day
                    if (daysUntilExpiry == 90 || daysUntilExpiry == 60 || daysUntilExpiry == 30 || 
                        daysUntilExpiry == 14 || daysUntilExpiry == 7 || daysUntilExpiry == 3 || daysUntilExpiry == 1)
                    {
                        await notificationService.CreatePolicyExpiryNotificationAsync(policy);
                        _logger.LogInformation($"Created policy expiry notification for policy {policy.PolicyNumber} - {daysUntilExpiry} days until expiry");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing policy expiry notifications");
            }
        }

        private async Task ProcessRenewalReminderNotifications(INotificationService notificationService, List<Models.Insurance> policies)
        {
            try
            {
                var today = DateTime.Today;
                var renewalWindow = today.AddDays(60); // Start renewal reminders 60 days before expiry

                var policiesForRenewal = policies
                    .Where(p => p.PolicyEndDate >= today && p.PolicyEndDate <= renewalWindow)
                    .ToList();

                foreach (var policy in policiesForRenewal)
                {
                    var daysUntilExpiry = (policy.PolicyEndDate?.Date - today)?.Days;
                    
                    // Send renewal reminders at specific intervals: 60 days, 45 days, 30 days, 15 days
                    if (daysUntilExpiry == 60 || daysUntilExpiry == 45 || daysUntilExpiry == 30 || daysUntilExpiry == 15)
                    {
                        await notificationService.CreatePolicyRenewalReminderAsync(policy);
                        _logger.LogInformation($"Created renewal reminder for policy {policy.PolicyNumber} - {daysUntilExpiry} days until expiry");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing renewal reminder notifications");
            }
        }

        private async Task ProcessDocumentExpiryNotifications(INotificationService notificationService, IInsuranceService insuranceService, List<Models.Insurance> policies)
        {
            try
            {
                var today = DateTime.Today;
                var documentWindow = today.AddDays(30); // Notify 30 days in advance for document expiry

                foreach (var policy in policies)
                {
                    var documents = await insuranceService.GetDocumentsByPolicyAsync(policy.Id);
                    
                    var expiringDocuments = documents
                        .Where(d => d.ExpiryDate.HasValue && 
                                   d.ExpiryDate.Value >= today && 
                                   d.ExpiryDate.Value <= documentWindow)
                        .ToList();

                    foreach (var document in expiringDocuments)
                    {
                        var daysUntilExpiry = (document.ExpiryDate!.Value.Date - today).Days;
                        
                        // Send document expiry notifications at specific intervals: 30 days, 14 days, 7 days, 3 days, 1 day
                        if (daysUntilExpiry == 30 || daysUntilExpiry == 14 || daysUntilExpiry == 7 || 
                            daysUntilExpiry == 3 || daysUntilExpiry == 1)
                        {
                            await notificationService.CreateDocumentExpiryNotificationAsync(policy, document);
                            _logger.LogInformation($"Created document expiry notification for {document.DocumentType} in policy {policy.PolicyNumber} - {daysUntilExpiry} days until expiry");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing document expiry notifications");
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Insurance Notification Background Service is stopping");
            await base.StopAsync(stoppingToken);
        }

        /// <summary>
        /// Manually trigger notification processing (for testing or immediate execution)
        /// </summary>
        public async Task TriggerNotificationProcessing()
        {
            _logger.LogInformation("Manually triggering insurance notification processing");
            await ProcessInsuranceNotifications();
        }
    }
}