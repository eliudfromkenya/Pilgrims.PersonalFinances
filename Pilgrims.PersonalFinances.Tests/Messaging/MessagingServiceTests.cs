using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using FluentAssertions;
using Pilgrims.PersonalFinances.Core.Messaging.Messages;
using Pilgrims.PersonalFinances.Core.Messaging.Services;
using Pilgrims.PersonalFinances.Core.Messaging.Interfaces;
using Xunit;

namespace Pilgrims.PersonalFinances.Tests.Messaging;

public class MessagingServiceTests
{
    [Fact]
    public async Task ShowConfirmationAsync_ShouldReturnTrue_WhenResponseIsSetTrue()
    {
        // Arrange
        var messenger = new WeakReferenceMessenger();
        ConfirmationDialogMessage? capturedMessage = null;
        var recipient = new object();
        messenger.Register<ConfirmationDialogMessage>(recipient, (r, msg) => capturedMessage = msg);

        IMessagingService service = new MessagingService(messenger);

        // Act
        var task = service.ShowConfirmationAsync("Are you sure?", "Confirm", "Yes", "No");
        capturedMessage.Should().NotBeNull();
        capturedMessage!.ResponseSource.TrySetResult(true);
        var result = await task;

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ShowConfirmationAsync_ShouldReturnFalse_WhenResponseIsSetFalse()
    {
        // Arrange
        var messenger = new WeakReferenceMessenger();
        ConfirmationDialogMessage? capturedMessage = null;
        var recipient = new object();
        messenger.Register<ConfirmationDialogMessage>(recipient, (r, msg) => capturedMessage = msg);

        IMessagingService service = new MessagingService(messenger);

        // Act
        var task = service.ShowConfirmationAsync("Proceed?", "Confirm", "Yes", "No");
        capturedMessage.Should().NotBeNull();
        capturedMessage!.ResponseSource.TrySetResult(false);
        var result = await task;

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ShowAlertAsync_ShouldComplete_WhenResponseIsSet()
    {
        // Arrange
        var messenger = new WeakReferenceMessenger();
        AlertDialogMessage? capturedMessage = null;
        var recipient = new object();
        messenger.Register<AlertDialogMessage>(recipient, (r, msg) => capturedMessage = msg);

        IMessagingService service = new MessagingService(messenger);

        // Act
        var task = service.ShowAlertAsync("Alert message", "Alert", "OK");
        capturedMessage.Should().NotBeNull();
        capturedMessage!.ResponseSource.TrySetResult(true);
        await task;

        // Assert
        capturedMessage.Should().NotBeNull();
    }

    [Fact]
    public void ShowToast_ShouldSendToastNotificationMessage()
    {
        // Arrange
        var messenger = new WeakReferenceMessenger();
        ToastNotificationMessage? capturedMessage = null;
        var recipient = new object();
        messenger.Register<ToastNotificationMessage>(recipient, (r, msg) => capturedMessage = msg);

        IMessagingService service = new MessagingService(messenger);

        // Act
        service.ShowToast("Hello", ToastType.Warning, durationMs: 1234);

        // Assert
        capturedMessage.Should().NotBeNull();
        capturedMessage!.Value.Message.Should().Be("Hello");
        capturedMessage.Value.Type.Should().Be("warning");
        capturedMessage.Value.DurationMs.Should().Be(1234);
    }

    [Fact]
    public void ShowSystemNotification_ShouldSendSystemNotificationMessage()
    {
        // Arrange
        var messenger = new WeakReferenceMessenger();
        SystemNotificationMessage? capturedMessage = null;
        var recipient = new object();
        messenger.Register<SystemNotificationMessage>(recipient, (r, msg) => capturedMessage = msg);

        IMessagingService service = new MessagingService(messenger);

        // Act
        service.ShowSystemNotification("Body", "Title", SystemNotificationType.Error);

        // Assert
        capturedMessage.Should().NotBeNull();
        capturedMessage!.Value.Title.Should().Be("Title");
        capturedMessage.Value.Message.Should().Be("Body");
        capturedMessage.Value.Type.Should().Be("error");
    }
}