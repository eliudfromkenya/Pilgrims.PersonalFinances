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

        public NotificationHandler(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
        }

        /// <summary>
        /// Handles toast notification messages
        /// </summary>
        public void Receive(ToastNotificationMessage message)
        {
            _ = Task.Run(async () =>
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
            });
        }

        /// <summary>
        /// Handles confirmation dialog messages
        /// </summary>
        public void Receive(ConfirmationDialogMessage message)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    var result = await _jsRuntime.InvokeAsync<bool>("confirm", 
                        $"{message.Value.Title}\n\n{message.Value.Message}");
                    
                    message.ResponseSource.SetResult(result);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error showing confirmation dialog: {ex.Message}");
                    message.ResponseSource.SetResult(false);
                }
            });
        }

        /// <summary>
        /// Handles alert dialog messages
        /// </summary>
        public void Receive(AlertDialogMessage message)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await _jsRuntime.InvokeVoidAsync("alert", 
                        $"{message.Value.Title}\n\n{message.Value.Message}");
                    
                    message.ResponseSource.SetResult(true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error showing alert dialog: {ex.Message}");
                    message.ResponseSource.SetResult(false);
                }
            });
        }

        /// <summary>
        /// Handles system notification messages
        /// </summary>
        public void Receive(SystemNotificationMessage message)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    // For now, we'll use toast notifications for system notifications
                    // This can be extended to use native system notifications in the future
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
            });
        }
    }
}