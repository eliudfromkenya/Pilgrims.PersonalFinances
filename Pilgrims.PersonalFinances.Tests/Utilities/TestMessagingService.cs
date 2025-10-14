using Pilgrims.PersonalFinances.Core.Messaging.Interfaces;

namespace Pilgrims.PersonalFinances.Tests.Utilities
{
    // No-op messaging service for tests; satisfies GlobalExtensions.InitApp requirement
    public class TestMessagingService : IMessagingService
    {
        public void ShowToast(string message, Pilgrims.PersonalFinances.Core.Messaging.Messages.ToastType type = Pilgrims.PersonalFinances.Core.Messaging.Messages.ToastType.Info, int durationMs = 5000) { }

        public Task<bool> ShowConfirmationAsync(string message, string title = "Confirm", string confirmText = "Yes", string cancelText = "No")
            => Task.FromResult(true);

        public Task ShowAlertAsync(string message, string title = "Alert", string buttonText = "OK")
            => Task.CompletedTask;

        public void ShowSystemNotification(string message, string title, Pilgrims.PersonalFinances.Core.Messaging.Messages.SystemNotificationType type = Pilgrims.PersonalFinances.Core.Messaging.Messages.SystemNotificationType.Info) { }

        public void Navigate(string route, object? parameters = null) { }

        public void Register<TMessage>(object recipient, Pilgrims.PersonalFinances.Core.Messaging.Interfaces.MessageHandler<object, TMessage> handler) where TMessage : class { }

        public void Unregister(object recipient) { }

        public void Unregister<TMessage>(object recipient) where TMessage : class { }

        public void Send<TMessage>(TMessage message) where TMessage : class { }

        public Task ShowErrorAsync(Exception ex, string title = "Error") => Task.CompletedTask;
    }
}