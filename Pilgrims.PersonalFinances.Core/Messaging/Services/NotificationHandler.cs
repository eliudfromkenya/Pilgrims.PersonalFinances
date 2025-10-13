using CommunityToolkit.Mvvm.Messaging;
using Pilgrims.PersonalFinances.Core.Messaging.Messages;
using Pilgrims.PersonalFinances.Core.Messaging.Interfaces;

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
        private readonly IMessenger _messenger;
        private readonly IMessagingService _messagingService;

        public NotificationHandler(IMessenger messenger, IMessagingService messagingService)
        {
            _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
            _messagingService = messagingService ?? throw new ArgumentNullException(nameof(messagingService));

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
            var type = message.Value.Type switch
            {
                "success" => ToastType.Success,
                "error" => ToastType.Error,
                "warning" => ToastType.Warning,
                "info" => ToastType.Info,
                _ => ToastType.Info
            };
            _messagingService.ShowToast(message.Value.Message, type, message.Value.DurationMs);
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

                var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                _messenger.Send(new ShowConfirmationMessage(message.Value.Message, title, result => tcs.TrySetResult(result), confirmText, cancelText));
                var result = await tcs.Task.ConfigureAwait(false);
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
                var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                _messenger.Send(new ShowAlertMessage(message.Value.Message, title, result => tcs.TrySetResult(result), message.Value.Type));
                var result = await tcs.Task.ConfigureAwait(false);
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
            var toastType = message.Value.Type switch
            {
                "success" => ToastType.Success,
                "error" => ToastType.Error,
                "warning" => ToastType.Warning,
                "info" => ToastType.Info,
                _ => ToastType.Info
            };

            _messagingService.ShowToast($"{message.Value.Title}: {message.Value.Message}", toastType, message.Value.DurationMs);
        }
    }
}