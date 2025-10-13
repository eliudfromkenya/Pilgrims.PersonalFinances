using CommunityToolkit.Mvvm.Messaging;
using Pilgrims.PersonalFinances.Core.Messaging.Interfaces;
using Pilgrims.PersonalFinances.Core.Messaging.Messages;

namespace Pilgrims.PersonalFinances.Core.Messaging.Services
{
    /// <summary>
    /// Implementation of the messaging service using CommunityToolkit.Mvvm's WeakReferenceMessenger
    /// </summary>
    public class MessagingService : IMessagingService
    {
        private readonly IMessenger _messenger;
        private readonly NotificationHandler  _messageHandler;

        public MessagingService(IMessenger messenger)
        {
            _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
            _messageHandler = new NotificationHandler(messenger, this); // Ensure the handler is instantiated to receive messages
        }

        /// <summary>
        /// Shows a toast notification
        /// </summary>
        public void ShowToast(string message, ToastType type = ToastType.Info, int durationMs = 5000)
        {
            var toastMessage = new ToastNotificationMessage(message, type, durationMs);
            _messenger.Send(toastMessage);
        }

        /// <summary>
        /// Shows a confirmation dialog and returns the user's response
        /// </summary>
        public async Task<bool> ShowConfirmationAsync(string message, string title = "Confirm", string confirmText = "Yes", string cancelText = "No")
        {
            var confirmationMessage = new ConfirmationDialogMessage(message, title, confirmText, cancelText);
            _messenger.Send(confirmationMessage);
            
            // Wait for the response from the UI handler
            return await confirmationMessage.ResponseSource.Task.ConfigureAwait(false);
        }

        /// <summary>
        /// Shows an alert dialog
        /// </summary>
        public async Task ShowAlertAsync(string message, string title = "Alert", string buttonText = "OK")
        {
            var alertMessage = new AlertDialogMessage(message, title, buttonText);
            _messenger.Send(alertMessage);
            
            // Wait for the response from the UI handler
            await alertMessage.ResponseSource.Task.ConfigureAwait(false);
        }

        /// <summary>
        /// Shows a system notification
        /// </summary>
        public void ShowSystemNotification(string message, string title, SystemNotificationType type = SystemNotificationType.Info)
        {
            var systemMessage = new SystemNotificationMessage(message, title, type);
            _messenger.Send(systemMessage);
        }

        /// <summary>
        /// Navigates to a specific route
        /// </summary>
        public void Navigate(string route, object? parameters = null)
        {
            var navigationMessage = new NavigationMessage(route, parameters);
            _messenger.Send(navigationMessage);
        }

        /// <summary>
        /// Registers a recipient for a specific message type
        /// </summary>
        public void Register<TMessage>(object recipient, Interfaces.MessageHandler<object, TMessage> handler) where TMessage : class
        {
            _messenger.Register<TMessage>(recipient, (r, m) => handler(r, m));
        }

        /// <summary>
        /// Unregisters a recipient from all messages
        /// </summary>
        public void Unregister(object recipient)
        {
            _messenger.UnregisterAll(recipient);
        }

        /// <summary>
        /// Unregisters a recipient from a specific message type
        /// </summary>
        public void Unregister<TMessage>(object recipient) where TMessage : class
        {
            _messenger.Unregister<TMessage>(recipient);
        }

        /// <summary>
        /// Sends a custom message
        /// </summary>
        public void Send<TMessage>(TMessage message) where TMessage : class
        {
            _messenger.Send(message);
        }

        public async Task ShowErrorAsync(Exception ex, string title = "Error")
        {
            var alertMessage = new AlertDialogMessage(ex, title);
            _messenger.Send(alertMessage);

            // Wait for the response from the UI handler
            await alertMessage.ResponseSource.Task.ConfigureAwait(false);
        }
    }
}