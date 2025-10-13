using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Pilgrims.PersonalFinances.Core.Messaging.Messages
{
    /// <summary>
    /// Base message for all notification-related messages in the MVVM messaging system
    /// </summary>
    public abstract class NotificationMessage : ValueChangedMessage<NotificationMessageData>
    {
        protected NotificationMessage(NotificationMessageData value) : base(value)
        {
        }
    }

    /// <summary>
    /// Message for displaying toast notifications
    /// </summary>
    public class ToastNotificationMessage : NotificationMessage
    {
        public ToastNotificationMessage(string message, ToastType type = ToastType.Info, int durationMs = 5000)
            : base(new NotificationMessageData
            {
                Message = message,
                Type = type.ToString().ToLower(),
                DurationMs = durationMs,
                Timestamp = DateTime.UtcNow
            })
        {
        }
    }

    /// <summary>
    /// Message for displaying confirmation dialogs
    /// </summary>
    public class ConfirmationDialogMessage : NotificationMessage
    {
        public TaskCompletionSource<bool> ResponseSource { get; }

        public ConfirmationDialogMessage(string message, string title = "Confirm", string confirmText = "Yes", string cancelText = "No")
            : base(new NotificationMessageData
            {
                Message = message,
                Title = title,
                ConfirmText = confirmText,
                CancelText = cancelText,
                Type = "confirmation",
                Timestamp = DateTime.UtcNow
            })
        {
            ResponseSource = new TaskCompletionSource<bool>();
        }
    }

    /// <summary>
    /// Message for displaying alert dialogs
    /// </summary>
    public class AlertDialogMessage : NotificationMessage
    {
        public TaskCompletionSource<bool> ResponseSource { get; }

        public AlertDialogMessage(string message, string title = "Alert", string buttonText = "OK")
            : base(new NotificationMessageData
            {
                Message = message,
                Title = title,
                ConfirmText = buttonText,
                Type = "alert",
                Timestamp = DateTime.UtcNow
            })
        {
            ResponseSource = new TaskCompletionSource<bool>();
        }

        public AlertDialogMessage(Exception ex, string title = "Error Occured")
           : base(new NotificationMessageData
           {
               Message = (ex?.InnerException ?? ex)?.Message ?? "An error occured",
               Title = title,
               ConfirmText = "OK",
               Type = "error",
               Timestamp = DateTime.UtcNow
           })
        {
            ResponseSource = new TaskCompletionSource<bool>();
        }
    }

    /// <summary>
    /// Message for displaying system notifications
    /// </summary>
    public class SystemNotificationMessage : NotificationMessage
    {
        public SystemNotificationMessage(string message, string title, SystemNotificationType notificationType = SystemNotificationType.Info)
            : base(new NotificationMessageData
            {
                Message = message,
                Title = title,
                Type = notificationType.ToString().ToLower(),
                Timestamp = DateTime.UtcNow
            })
        {
        }
    }

    /// <summary>
    /// Message for navigation requests
    /// </summary>
    public class NavigationMessage : ValueChangedMessage<NavigationData>
    {
        public NavigationMessage(string route, object? parameters = null)
            : base(new NavigationData
            {
                Route = route,
                Parameters = parameters,
                Timestamp = DateTime.UtcNow
            })
        {
        }
    }

    /// <summary>
    /// Data structure for notification messages
    /// </summary>
    public class NotificationMessageData
    {
        public string Message { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string Type { get; set; } = "info";
        public string? ConfirmText { get; set; }
        public string? CancelText { get; set; }
        public int DurationMs { get; set; } = 5000;
        public DateTime Timestamp { get; set; }
        public Dictionary<string, object>? Metadata { get; set; }
    }

    /// <summary>
    /// Data structure for navigation messages
    /// </summary>
    public class NavigationData
    {
        public string Route { get; set; } = string.Empty;
        public object? Parameters { get; set; }
        public DateTime Timestamp { get; set; }
    }

    /// <summary>
    /// Toast notification types
    /// </summary>
    public enum ToastType
    {
        Success,
        Error,
        Warning,
        Info
    }

    /// <summary>
    /// System notification types
    /// </summary>
    public enum SystemNotificationType
    {
        Info,
        Warning,
        Error,
        Success
    }
}