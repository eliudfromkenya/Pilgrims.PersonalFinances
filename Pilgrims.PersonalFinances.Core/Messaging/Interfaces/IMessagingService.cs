using Pilgrims.PersonalFinances.Core.Messaging.Messages;

namespace Pilgrims.PersonalFinances.Core.Messaging.Interfaces
{
    /// <summary>
    /// Interface for the messaging service that handles publisher/subscriber communication
    /// </summary>
    public interface IMessagingService
    {
        /// <summary>
        /// Shows a toast notification
        /// </summary>
        /// <param name="message">The message to display</param>
        /// <param name="type">The type of toast notification</param>
        /// <param name="durationMs">Duration in milliseconds (default: 5000)</param>
        void ShowToast(string message, ToastType type = ToastType.Info, int durationMs = 5000);

        /// <summary>
        /// Shows a confirmation dialog and returns the user's response
        /// </summary>
        /// <param name="message">The confirmation message</param>
        /// <param name="title">The dialog title</param>
        /// <param name="confirmText">Text for the confirm button</param>
        /// <param name="cancelText">Text for the cancel button</param>
        /// <returns>True if confirmed, false if cancelled</returns>
        Task<bool> ShowConfirmationAsync(string message, string title = "Confirm", string confirmText = "Yes", string cancelText = "No");

        /// <summary>
        /// Shows an alert dialog
        /// </summary>
        /// <param name="message">The alert message</param>
        /// <param name="title">The dialog title</param>
        /// <param name="buttonText">Text for the OK button</param>
        Task ShowAlertAsync(string message, string title = "Alert", string buttonText = "OK");

        /// <summary>
        /// Shows a system notification
        /// </summary>
        /// <param name="message">The notification message</param>
        /// <param name="title">The notification title</param>
        /// <param name="type">The type of system notification</param>
        void ShowSystemNotification(string message, string title, SystemNotificationType type = SystemNotificationType.Info);

        /// <summary>
        /// Navigates to a specific route
        /// </summary>
        /// <param name="route">The route to navigate to</param>
        /// <param name="parameters">Optional navigation parameters</param>
        void Navigate(string route, object? parameters = null);

        /// <summary>
        /// Registers a recipient for a specific message type
        /// </summary>
        /// <typeparam name="TMessage">The message type to listen for</typeparam>
        /// <param name="recipient">The recipient object</param>
        /// <param name="handler">The message handler</param>
        void Register<TMessage>(object recipient, MessageHandler<object, TMessage> handler) where TMessage : class;

        /// <summary>
        /// Unregisters a recipient from all messages
        /// </summary>
        /// <param name="recipient">The recipient to unregister</param>
        void Unregister(object recipient);

        /// <summary>
        /// Unregisters a recipient from a specific message type
        /// </summary>
        /// <typeparam name="TMessage">The message type to unregister from</typeparam>
        /// <param name="recipient">The recipient to unregister</param>
        void Unregister<TMessage>(object recipient) where TMessage : class;

        /// <summary>
        /// Sends a custom message
        /// </summary>
        /// <typeparam name="TMessage">The message type</typeparam>
        /// <param name="message">The message to send</param>
        void Send<TMessage>(TMessage message) where TMessage : class;
    }

    /// <summary>
    /// Delegate for message handlers
    /// </summary>
    /// <typeparam name="TRecipient">The recipient type</typeparam>
    /// <typeparam name="TMessage">The message type</typeparam>
    /// <param name="recipient">The recipient</param>
    /// <param name="message">The message</param>
    public delegate void MessageHandler<in TRecipient, in TMessage>(TRecipient recipient, TMessage message);
}