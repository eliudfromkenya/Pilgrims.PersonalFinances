using Pilgrims.PersonalFinances.Models;
using Pilgrims.PersonalFinances.Models.Enums;
using Pilgrims.PersonalFinances.Services.Interfaces;

namespace Pilgrims.PersonalFinances.Services.Extensions
{
    public static class InsuranceNotificationExtensions
    {
        /// <summary>
        /// Creates a premium due notification
        /// </summary>
        public static async Task CreatePremiumDueNotificationAsync(this INotificationService notificationService, Insurance insurance, DateTime dueDate)
        {
            var daysUntilDue = (dueDate.Date - DateTime.Today).Days;
            var priority = daysUntilDue switch
            {
                0 => NotificationPriority.Critical,
                1 => NotificationPriority.High,
                <= 3 => NotificationPriority.Medium,
                _ => NotificationPriority.Low
            };

            var timeDescription = daysUntilDue switch
            {
                0 => "today",
                1 => "tomorrow",
                _ => $"in {daysUntilDue} days"
            };

            var title = $"Premium Due: {insurance.PolicyName}";
            var message = $"Your {insurance.PolicyType} insurance premium of {insurance.PremiumAmount:C} is due {timeDescription}. " +
                         $"Policy: {insurance.PolicyNumber} with {insurance.InsuranceCompany}.";

            await notificationService.CreateNotificationAsync(title, message, AppNotificationType.BillReminder, priority);
        }

        /// <summary>
        /// Creates a policy expiry notification
        /// </summary>
        public static async Task CreatePolicyExpiryNotificationAsync(this INotificationService notificationService, Insurance insurance)
        {
            var daysUntilExpiry = (insurance.PolicyEndDate?.Date - DateTime.Today)?.Days ?? 0;
            var priority = daysUntilExpiry switch
            {
                <= 7 => NotificationPriority.Critical,
                <= 30 => NotificationPriority.High,
                <= 60 => NotificationPriority.Medium,
                _ => NotificationPriority.Low
            };

            var timeDescription = daysUntilExpiry switch
            {
                0 => "today",
                1 => "tomorrow",
                <= 7 => $"in {daysUntilExpiry} days",
                _ => $"in {daysUntilExpiry} days"
            };

            var title = $"Policy Expiring: {insurance.PolicyName}";
            var message = $"Your {insurance.PolicyType} insurance policy expires {timeDescription}. " +
                         $"Policy: {insurance.PolicyNumber} with {insurance.InsuranceCompany}. " +
                         $"Coverage: {insurance.CoverageAmount:C}. Please renew to maintain coverage.";

            await notificationService.CreateNotificationAsync(title, message, AppNotificationType.SystemAlert, priority);
        }

        /// <summary>
        /// Creates a claim status update notification
        /// </summary>
        public static async Task CreateClaimStatusUpdateNotificationAsync(this INotificationService notificationService, InsuranceClaim claim, Insurance insurance)
        {
            var priority = claim.Status switch
            {
                ClaimStatus.Approved => NotificationPriority.High,
                ClaimStatus.Rejected => NotificationPriority.High,
                ClaimStatus.Settled => NotificationPriority.Medium,
                _ => NotificationPriority.Low
            };

            var statusMessage = claim.Status switch
            {
                ClaimStatus.Approved => "has been approved",
                ClaimStatus.Rejected => "has been rejected",
                ClaimStatus.Settled => "has been settled",
                ClaimStatus.UnderReview => "is now under review",
                _ => "status has been updated"
            };

            var title = $"Claim Update: {claim.ClaimNumber}";
            var message = $"Your insurance claim {statusMessage}. " +
                         $"Claim: {claim.ClaimNumber} for {insurance.PolicyName}. " +
                         $"Amount: {(claim.ClaimAmount):C}";

            if (claim.ApprovedAmount.HasValue && claim.Status == ClaimStatus.Approved)
            {
                message += $". Approved amount: {claim.ApprovedAmount.Value:C}";
            }

            await notificationService.CreateNotificationAsync(title, message, AppNotificationType.SystemAlert, priority);
        }

        /// <summary>
        /// Creates a policy renewal reminder notification
        /// </summary>
        public static async Task CreatePolicyRenewalReminderAsync(this INotificationService notificationService, Insurance insurance)
        {
            var daysUntilRenewal = (insurance.PolicyEndDate?.Date - DateTime.Today)?.Days;
            var priority = daysUntilRenewal switch
            {
                <= 30 => NotificationPriority.High,
                <= 60 => NotificationPriority.Medium,
                _ => NotificationPriority.Low
            };

            var title = $"Renewal Reminder: {insurance.PolicyName}";
            var message = $"Your {insurance.PolicyType} insurance policy is up for renewal in {daysUntilRenewal} days. " +
                         $"Policy: {insurance.PolicyNumber} with {insurance.InsuranceCompany}. " +
                         $"Current premium: {insurance.PremiumAmount:C}. Review your coverage and renew on time.";

            await notificationService.CreateNotificationAsync(title, message, AppNotificationType.SystemAlert, priority);
        }

        /// <summary>
        /// Creates a coverage gap alert notification
        /// </summary>
        public static async Task CreateCoverageGapAlertAsync(this INotificationService notificationService, string policyType, decimal recommendedCoverage, decimal currentCoverage)
        {
            var gapAmount = recommendedCoverage - currentCoverage;
            var gapPercentage = currentCoverage > 0 ? (gapAmount / recommendedCoverage) * 100 : 100;

            var title = $"Coverage Gap Alert: {policyType} Insurance";
            var message = $"You may have insufficient {policyType.ToLower()} insurance coverage. " +
                         $"Current coverage: {currentCoverage:C}, Recommended: {recommendedCoverage:C}. " +
                         $"Gap: {gapAmount:C} ({gapPercentage:F1}%). Consider increasing your coverage.";

            await notificationService.CreateNotificationAsync(title, message, AppNotificationType.SystemAlert, NotificationPriority.Medium);
        }

        /// <summary>
        /// Creates a premium payment confirmation notification
        /// </summary>
        public static async Task CreatePremiumPaymentConfirmationAsync(this INotificationService notificationService, Insurance insurance, InsurancePremiumPayment payment)
        {
            var title = $"Premium Paid: {insurance.PolicyName}";
            var message = $"Premium payment of {payment.Amount:C} has been processed for your {insurance.PolicyType} insurance. " +
                         $"Policy: {insurance.PolicyNumber}. Payment date: {payment.PaymentDate:MMM dd, yyyy}. " +
                         $"Next payment due: {payment.NextDueDate:MMM dd, yyyy}.";

            await notificationService.CreateNotificationAsync(title, message, AppNotificationType.SystemAlert, NotificationPriority.Low);
        }

        /// <summary>
        /// Creates a document expiry notification
        /// </summary>
        public static async Task CreateDocumentExpiryNotificationAsync(this INotificationService notificationService, Insurance insurance, InsuranceDocument document)
        {
            if (!document.ExpiryDate.HasValue) return;

            var daysUntilExpiry = (document.ExpiryDate - DateTime.Today)?.Days;
            var priority = daysUntilExpiry switch
            {
                <= 7 => NotificationPriority.High,
                <= 30 => NotificationPriority.Medium,
                _ => NotificationPriority.Low
            };

            var timeDescription = daysUntilExpiry switch
            {
                0 => "today",
                1 => "tomorrow",
                _ => $"in {daysUntilExpiry} days"
            };

            var title = $"Document Expiring: {document.DocumentType}";
            var message = $"Your {document.DocumentType} document for {insurance.PolicyName} expires {timeDescription}. " +
                         $"Please update your documents to maintain policy compliance.";

            await notificationService.CreateNotificationAsync(title, message, AppNotificationType.SystemAlert, priority);
        }

        /// <summary>
        /// Creates multiple premium due notifications for upcoming payments
        /// </summary>
        public static async Task CreateUpcomingPremiumNotificationsAsync(this INotificationService notificationService, IEnumerable<Insurance> policies)
        {
            var today = DateTime.Today;
            var notificationWindow = today.AddDays(30); // Look ahead 30 days

            foreach (var policy in policies)
            {
                if (policy.IsActive && policy.NextPremiumDueDate.HasValue)
                {
                    var dueDate = policy.NextPremiumDueDate.Value;
                    if (dueDate >= today && dueDate <= notificationWindow)
                    {
                        await CreatePremiumDueNotificationAsync(notificationService, policy, dueDate);
                    }
                }
            }
        }

        /// <summary>
        /// Creates multiple policy expiry notifications
        /// </summary>
        public static async Task CreateUpcomingExpiryNotificationsAsync(this INotificationService notificationService, IEnumerable<Insurance> policies)
        {
            var today = DateTime.Today;
            var notificationWindow = today.AddDays(90); // Look ahead 90 days

            foreach (var policy in policies)
            {
                if (policy.IsActive && policy.PolicyEndDate >= today && policy.PolicyEndDate <= notificationWindow)
                {
                    await CreatePolicyExpiryNotificationAsync(notificationService, policy);
                }
            }
        }

        /// <summary>
        /// Creates batch notifications for policy renewals
        /// </summary>
        public static async Task CreateUpcomingRenewalNotificationsAsync(this INotificationService notificationService, IEnumerable<Insurance> policies)
        {
            var today = DateTime.Today;
            var notificationWindow = today.AddDays(60); // Look ahead 60 days

            foreach (var policy in policies)
            {
                if (policy.IsActive && policy.PolicyEndDate >= today && policy.PolicyEndDate <= notificationWindow)
                {
                    await CreatePolicyRenewalReminderAsync(notificationService, policy);
                }
            }
        }

        /// <summary>
        /// Creates success notification for insurance operations
        /// </summary>
        public static async Task ShowInsuranceSuccessAsync(this INotificationService notificationService, string message)
        {
            await notificationService.CreateNotificationAsync("Insurance Success", message, AppNotificationType.SystemAlert, NotificationPriority.Low);
        }

        /// <summary>
        /// Creates error notification for insurance operations
        /// </summary>
        public static async Task ShowInsuranceErrorAsync(this INotificationService notificationService, string title, string message)
        {
            await notificationService.CreateNotificationAsync($"Insurance Error: {title}", message, AppNotificationType.SystemAlert, NotificationPriority.High);
        }
    }
}