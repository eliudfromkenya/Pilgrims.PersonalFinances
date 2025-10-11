using Pilgrims.PersonalFinances.Models.Enums;
using Pilgrims.PersonalFinances.Services.Interfaces;

namespace Pilgrims.PersonalFinances.Services.Extensions
{
    public static class NotificationServiceExtensions
    {
        public static async Task ShowSuccessAsync(this INotificationService notificationService, string message)
        {
            await notificationService.CreateNotificationAsync("Success", message, AppNotificationType.SystemAlert, NotificationPriority.Low);
        }

        public static async Task ShowErrorAsync(this INotificationService notificationService, string title, string message)
        {
            await notificationService.CreateNotificationAsync(title, message, AppNotificationType.SystemAlert, NotificationPriority.High);
        }

        public static async Task<bool> ShowConfirmAsync(this INotificationService notificationService, string title, string message)
        {
            // For now, we'll just return true to simulate confirmation
            // In a real implementation, this would show a modal dialog
            await notificationService.CreateNotificationAsync(title, message, AppNotificationType.SystemAlert, NotificationPriority.Medium);
            return true;
        }
    }
}