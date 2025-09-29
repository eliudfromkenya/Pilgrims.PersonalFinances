using Pilgrims.PersonalFinances.Core.Messaging.Interfaces;
using Pilgrims.PersonalFinances.Core.Messaging.Messages;

namespace Pilgrims.PersonalFinances.Core.Messaging.Extensions
{
    /// <summary>
    /// Extension methods for IMessagingService to provide convenient toast notification methods
    /// </summary>
    public static class MessagingServiceExtensions
    {
        /// <summary>
        /// Shows a success toast notification
        /// </summary>
        /// <param name="messagingService">The messaging service</param>
        /// <param name="message">The message to display</param>
        /// <param name="durationMs">Duration in milliseconds (default: 5000)</param>
        public static void ShowSuccessToast(this IMessagingService messagingService, string message, int durationMs = 5000)
        {
            messagingService.ShowToast(message, ToastType.Success, durationMs);
        }

        /// <summary>
        /// Shows an error toast notification
        /// </summary>
        /// <param name="messagingService">The messaging service</param>
        /// <param name="message">The message to display</param>
        /// <param name="durationMs">Duration in milliseconds (default: 5000)</param>
        public static void ShowErrorToast(this IMessagingService messagingService, string message, int durationMs = 5000)
        {
            messagingService.ShowToast(message, ToastType.Error, durationMs);
        }

        /// <summary>
        /// Shows an info toast notification
        /// </summary>
        /// <param name="messagingService">The messaging service</param>
        /// <param name="message">The message to display</param>
        /// <param name="durationMs">Duration in milliseconds (default: 5000)</param>
        public static void ShowInfoToast(this IMessagingService messagingService, string message, int durationMs = 5000)
        {
            messagingService.ShowToast(message, ToastType.Info, durationMs);
        }

        /// <summary>
        /// Shows a warning toast notification
        /// </summary>
        /// <param name="messagingService">The messaging service</param>
        /// <param name="message">The message to display</param>
        /// <param name="durationMs">Duration in milliseconds (default: 5000)</param>
        public static void ShowWarningToast(this IMessagingService messagingService, string message, int durationMs = 5000)
        {
            messagingService.ShowToast(message, ToastType.Warning, durationMs);
        }

        /// <summary>
        /// Shows a success toast notification asynchronously
        /// </summary>
        /// <param name="messagingService">The messaging service</param>
        /// <param name="message">The message to display</param>
        /// <param name="durationMs">Duration in milliseconds (default: 5000)</param>
        public static Task ShowSuccessToastAsync(this IMessagingService messagingService, string message, int durationMs = 5000)
        {
            messagingService.ShowToast(message, ToastType.Success, durationMs);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Shows an error toast notification asynchronously
        /// </summary>
        /// <param name="messagingService">The messaging service</param>
        /// <param name="message">The message to display</param>
        /// <param name="durationMs">Duration in milliseconds (default: 5000)</param>
        public static Task ShowErrorToastAsync(this IMessagingService messagingService, string message, int durationMs = 5000)
        {
            messagingService.ShowToast(message, ToastType.Error, durationMs);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Shows an info toast notification asynchronously
        /// </summary>
        /// <param name="messagingService">The messaging service</param>
        /// <param name="message">The message to display</param>
        /// <param name="durationMs">Duration in milliseconds (default: 5000)</param>
        public static Task ShowInfoToastAsync(this IMessagingService messagingService, string message, int durationMs = 5000)
        {
            messagingService.ShowToast(message, ToastType.Info, durationMs);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Shows a warning toast notification asynchronously
        /// </summary>
        /// <param name="messagingService">The messaging service</param>
        /// <param name="message">The message to display</param>
        /// <param name="durationMs">Duration in milliseconds (default: 5000)</param>
        public static Task ShowWarningToastAsync(this IMessagingService messagingService, string message, int durationMs = 5000)
        {
            messagingService.ShowToast(message, ToastType.Warning, durationMs);
            return Task.CompletedTask;
        }
    }
}