using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pilgrims.PersonalFinances.Core.Messaging.Interfaces;
using Pilgrims.PersonalFinances.Core.Messaging.Messages;

namespace Pilgrims.PersonalFinances.Core.ViewModels
{
    /// <summary>
    /// Base ViewModel class providing common functionality for all ViewModels
    /// </summary>
    public abstract partial class BaseViewModel : ObservableObject
    {
        protected readonly IMessagingService _messagingService;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string title = string.Empty;

        protected BaseViewModel(IMessagingService messagingService)
        {
            _messagingService = messagingService ?? throw new ArgumentNullException(nameof(messagingService));
        }

        /// <summary>
        /// Execute an async operation with busy state management
        /// </summary>
        protected async Task ExecuteAsync(Func<Task> operation)
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                await operation();
            }
            catch (Exception ex)
            {
                ShowErrorToast($"An error occurred: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Execute an async operation with busy state management and return result
        /// </summary>
        protected async Task<T?> ExecuteAsync<T>(Func<Task<T>> operation)
        {
            if (IsBusy) return default;

            try
            {
                IsBusy = true;
                return await operation();
            }
            catch (Exception ex)
            {
                ShowErrorToast($"An error occurred: {ex.Message}");
                return default;
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Show a success toast notification
        /// </summary>
        protected void ShowSuccessToast(string message)
        {
            _messagingService.Send(new ToastNotificationMessage(message, ToastType.Success));
        }

        /// <summary>
        /// Show an error toast notification
        /// </summary>
        protected void ShowErrorToast(string message)
        {
            _messagingService.Send(new ToastNotificationMessage(message, ToastType.Error));
        }

        /// <summary>
        /// Show an info toast notification
        /// </summary>
        protected void ShowInfoToast(string message)
        {
            _messagingService.Send(new ToastNotificationMessage(message, ToastType.Info));
        }

        /// <summary>
        /// Show a warning toast notification
        /// </summary>
        protected void ShowWarningToast(string message)
        {
            _messagingService.Send(new ToastNotificationMessage(message, ToastType.Warning));
        }

        /// <summary>
        /// Show a toast notification with specified type
        /// </summary>
        protected void ShowToastNotification(string message, ToastType type)
        {
            _messagingService.Send(new ToastNotificationMessage(message, type));
        }

        /// <summary>
        /// Navigate to a specific route
        /// </summary>
        protected void Navigate(string route)
        {
            _messagingService.Send(new NavigationMessage(route));
        }

        /// <summary>
        /// Show an alert dialog
        /// </summary>
        protected void ShowAlert(string message, string title = "Alert")
        {
            _messagingService.Send(new ShowAlertMessage(message, title));
        }

        /// <summary>
        /// Show a confirmation dialog
        /// </summary>
        protected void ShowConfirmation(string message, string title = "Confirm", Action<bool>? onResult = null)
        {
            _messagingService.Send(new ShowConfirmationMessage(message, title, onResult));
        }

        /// <summary>
        /// Download a file
        /// </summary>
        protected void DownloadFile(string fileName, byte[] fileData, string contentType = "application/octet-stream")
        {
            _messagingService.Send(new DownloadFileMessage(fileName, fileData, contentType));
        }

        /// <summary>
        /// Print an element or the entire page
        /// </summary>
        protected void Print(string? elementId = null)
        {
            _messagingService.Send(new PrintMessage(elementId));
        }

        /// <summary>
        /// Initialize a JavaScript component
        /// </summary>
        protected void InitializeJavaScript(string componentType, string? elementId = null, object? data = null)
        {
            _messagingService.Send(new InitializeJavaScriptMessage(componentType, elementId, data));
        }

        /// <summary>
        /// Update a JavaScript component with new data
        /// </summary>
        protected void UpdateJavaScript(string componentType, string elementId, object data)
        {
            _messagingService.Send(new UpdateJavaScriptMessage(componentType, elementId, data));
        }

        /// <summary>
        /// Set a value in local storage
        /// </summary>
        protected void SetLocalStorage(string key, string value)
        {
            _messagingService.Send(new LocalStorageMessage(LocalStorageMessage.Operation.Set, key, value));
        }

        /// <summary>
        /// Get a value from local storage
        /// </summary>
        protected void GetLocalStorage(string key, Action<string?> onResult)
        {
            _messagingService.Send(new LocalStorageMessage(LocalStorageMessage.Operation.Get, key, onResult: onResult));
        }

        /// <summary>
        /// Remove a value from local storage
        /// </summary>
        protected void RemoveLocalStorage(string key)
        {
            _messagingService.Send(new LocalStorageMessage(LocalStorageMessage.Operation.Remove, key));
        }

        /// <summary>
        /// Toggle dark mode
        /// </summary>
        protected void ToggleDarkMode(bool isDarkMode)
        {
            _messagingService.Send(new ToggleDarkModeMessage(isDarkMode));
        }

        /// <summary>
        /// Log a message to console
        /// </summary>
        protected void LogToConsole(ConsoleLogMessage.LogLevel level, string message, params object[] args)
        {
            _messagingService.Send(new ConsoleLogMessage(level, message, args));
        }

        /// <summary>
        /// Log an error to console
        /// </summary>
        protected void LogError(string message, params object[] args)
        {
            LogToConsole(ConsoleLogMessage.LogLevel.Error, message, args);
        }
    }
}