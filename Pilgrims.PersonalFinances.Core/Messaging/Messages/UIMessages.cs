using Pilgrims.PersonalFinances.Core.Messaging.Interfaces;

namespace Pilgrims.PersonalFinances.Core.Messaging.Messages
{
    /// <summary>
    /// Message to show an alert dialog
    /// </summary>
    public class ShowAlertMessage : IMessage
    {
        public string Message { get; }
        public string Title { get; }
        public Action<bool>? OnResult { get; }

        public ShowAlertMessage(string message, string title = "Alert", Action<bool>? onResult = null)
        {
            Message = message;
            Title = title;
            OnResult = onResult;
        }
    }

    /// <summary>
    /// Message to show a confirmation dialog
    /// </summary>
    public class ShowConfirmationMessage : IMessage
    {
        public string Message { get; }
        public string Title { get; }
        public string ConfirmText { get; }
        public string CancelText { get; }
        public Action<bool>? OnResult { get; }

        public ShowConfirmationMessage(string message, string title = "Confirm", Action<bool>? onResult = null, string confirmText = "Yes", string cancelText = "No")
        {
            Message = message;
            Title = title;
            OnResult = onResult;
            ConfirmText = confirmText;
            CancelText = cancelText;
        }
    }

    /// <summary>
    /// Message to download a file
    /// </summary>
    public class DownloadFileMessage : IMessage
    {
        public string FileName { get; }
        public byte[] FileData { get; }
        public string ContentType { get; }

        public DownloadFileMessage(string fileName, byte[] fileData, string contentType = "application/octet-stream")
        {
            FileName = fileName;
            FileData = fileData;
            ContentType = contentType;
        }
    }

    /// <summary>
    /// Message to print an element or the entire page
    /// </summary>
    public class PrintMessage : IMessage
    {
        public string? ElementId { get; }
        public bool PrintEntirePage { get; }

        public PrintMessage(string? elementId = null)
        {
            ElementId = elementId;
            PrintEntirePage = string.IsNullOrEmpty(elementId);
        }
    }

    /// <summary>
    /// Message to initialize JavaScript components (charts, particles, etc.)
    /// </summary>
    public class InitializeJavaScriptMessage : IMessage
    {
        public string ComponentType { get; }
        public string? ElementId { get; }
        public object? Data { get; }

        public InitializeJavaScriptMessage(string componentType, string? elementId = null, object? data = null)
        {
            ComponentType = componentType;
            ElementId = elementId;
            Data = data;
        }
    }

    /// <summary>
    /// Message to update JavaScript components with new data
    /// </summary>
    public class UpdateJavaScriptMessage : IMessage
    {
        public string ComponentType { get; }
        public string ElementId { get; }
        public object Data { get; }

        public UpdateJavaScriptMessage(string componentType, string elementId, object data)
        {
            ComponentType = componentType;
            ElementId = elementId;
            Data = data;
        }
    }

    /// <summary>
    /// Message to manage local storage operations
    /// </summary>
    public class LocalStorageMessage : IMessage
    {
        public enum Operation
        {
            Set,
            Get,
            Remove,
            Clear
        }

        public Operation Action { get; }
        public string Key { get; }
        public string? Value { get; }
        public Action<string?>? OnResult { get; }

        public LocalStorageMessage(Operation action, string key, string? value = null, Action<string?>? onResult = null)
        {
            Action = action;
            Key = key;
            Value = value;
            OnResult = onResult;
        }
    }

    /// <summary>
    /// Message to toggle dark mode
    /// </summary>
    public class ToggleDarkModeMessage : IMessage
    {
        public bool IsDarkMode { get; }

        public ToggleDarkModeMessage(bool isDarkMode)
        {
            IsDarkMode = isDarkMode;
        }
    }

    /// <summary>
    /// Message to log errors to console
    /// </summary>
    public class ConsoleLogMessage : IMessage
    {
        public enum LogLevel
        {
            Log,
            Info,
            Warn,
            Error
        }

        public LogLevel Level { get; }
        public string Message { get; }
        public object[]? Args { get; }

        public ConsoleLogMessage(LogLevel level, string message, params object[] args)
        {
            Level = level;
            Message = message;
            Args = args;
        }
    }
}