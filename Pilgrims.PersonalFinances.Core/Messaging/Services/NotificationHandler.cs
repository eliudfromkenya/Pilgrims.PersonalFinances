using CommunityToolkit.Mvvm.Messaging;
using Microsoft.JSInterop;
using Pilgrims.PersonalFinances.Core.Messaging.Messages;

namespace Pilgrims.PersonalFinances.Core.Messaging.Services
{
    /// <summary>
    /// Handles notification messages and executes the appropriate UI actions
    /// </summary>
    public class NotificationHandler : IRecipient<ToastNotificationMessage>, 
                                      IRecipient<ConfirmationDialogMessage>, 
                                      IRecipient<AlertDialogMessage>,
                                      IRecipient<SystemNotificationMessage>
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly IMessenger _messenger;

        public NotificationHandler(IJSRuntime jsRuntime, IMessenger messenger)
        {
            _jsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
            _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));

            // Register this handler as a recipient for all supported message types
            _messenger.Register<ToastNotificationMessage>(this);
            _messenger.Register<ConfirmationDialogMessage>(this);
            _messenger.Register<AlertDialogMessage>(this);
            _messenger.Register<SystemNotificationMessage>(this);
        }

        /// <summary>
        /// Handles toast notification messages
        /// </summary>
        public void Receive(ToastNotificationMessage message)
        {
            _ = ShowToastAsync(message);
        }

        private async Task ShowToastAsync(ToastNotificationMessage message)
        {
            try
            {
                var toastTypeString = message.Value.Type switch
                {
                    "success" => "success",
                    "error" => "error", 
                    "warning" => "warning",
                    "info" => "info",
                    _ => "info"
                };

                await _jsRuntime.InvokeVoidAsync("showToast", message.Value.Message, toastTypeString);
            }
            catch (Exception ex)
            {
                // Log error but don't throw to prevent breaking the application
                Console.WriteLine($"Error showing toast notification: {ex.Message}");
            }
        }

        /// <summary>
        /// Handles confirmation dialog messages
        /// </summary>
        public void Receive(ConfirmationDialogMessage message)
        {
            _ = HandleConfirmationAsync(message);
        }

        private async Task HandleConfirmationAsync(ConfirmationDialogMessage message)
        {
            try
            {
                var title = message.Value.Title ?? "Confirm";
                var confirmText = message.Value.ConfirmText ?? "Yes";
                var cancelText = message.Value.CancelText ?? "No";

                var result = await _jsRuntime.InvokeAsync<bool>("showConfirmationToast",
                    title,
                    message.Value.Message,
                    confirmText,
                    cancelText);

                message.ResponseSource.TrySetResult(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error showing confirmation dialog: {ex.Message}");
                message.ResponseSource.TrySetResult(false);
            }
        }

        /// <summary>
        /// Handles alert dialog messages
        /// </summary>
        public void Receive(AlertDialogMessage message)
        {
            _ = HandleAlertAsync(message);
        }

        private async Task HandleAlertAsync(AlertDialogMessage message)
        {
            try
            {
                var title = message.Value.Title ?? "Alert";
                var buttonText = message.Value.ConfirmText ?? "OK";

                var result = await _jsRuntime.InvokeAsync<bool>("showAlertToast",
                    title,
                    message.Value.Message,
                    buttonText);

                message.ResponseSource.TrySetResult(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error showing alert dialog: {ex.Message}");
                message.ResponseSource.TrySetResult(false);
            }
        }

        /// <summary>
        /// Handles system notification messages
        /// </summary>
        public void Receive(SystemNotificationMessage message)
        {
            _ = HandleSystemNotificationAsync(message);
        }

        private async Task HandleSystemNotificationAsync(SystemNotificationMessage message)
        {
            try
            {
                var toastType = message.Value.Type switch
                {
                    "success" => "success",
                    "error" => "error",
                    "warning" => "warning",
                    "info" => "info",
                    _ => "info"
                };

                await _jsRuntime.InvokeVoidAsync("showToast", 
                    $"{message.Value.Title}: {message.Value.Message}", toastType);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error showing system notification: {ex.Message}");
            }
        }
    }
}