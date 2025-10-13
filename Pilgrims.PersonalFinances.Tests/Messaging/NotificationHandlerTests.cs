using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using FluentAssertions;
using Moq;
using Pilgrims.PersonalFinances.Core.Messaging.Interfaces;
using Pilgrims.PersonalFinances.Core.Messaging.Messages;
using Pilgrims.PersonalFinances.Core.Messaging.Services;
using Xunit;

namespace Pilgrims.PersonalFinances.Tests.Messaging;

public class NotificationHandlerTests
{
    [Fact]
    public async Task AlertDialog_ShouldSetResultTrue_WhenUiHandlerReturnsTrue()
    {
        // Arrange
        var messenger = new WeakReferenceMessenger();
        var messagingServiceMock = new Mock<IMessagingService>();
        messenger.Register<ShowAlertMessage>(this, (r, msg) => msg.OnResult?.Invoke(true));

        var handler = new NotificationHandler(messenger, messagingServiceMock.Object);
        var message = new AlertDialogMessage("Test alert message", "Alert Title", "OK");

        // Act
        handler.Receive(message);
        var result = await message.ResponseSource.Task;

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ConfirmationDialog_ShouldSetResultFalse_WhenUiHandlerReturnsFalse()
    {
        // Arrange
        var messenger = new WeakReferenceMessenger();
        var messagingServiceMock = new Mock<IMessagingService>();
        messenger.Register<ShowConfirmationMessage>(this, (r, msg) => msg.OnResult?.Invoke(false));

        var handler = new NotificationHandler(messenger, messagingServiceMock.Object);
        var message = new ConfirmationDialogMessage("Confirm message", "Confirm Title", "Yes", "No");

        // Act
        handler.Receive(message);
        var result = await message.ResponseSource.Task;

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ToastNotification_ShouldDelegateToMessagingService_WithCorrectTypeAndDuration()
    {
        // Arrange
        var messenger = new WeakReferenceMessenger();
        var messagingServiceMock = new Mock<IMessagingService>();
        var handler = new NotificationHandler(messenger, messagingServiceMock.Object);
        var message = new ToastNotificationMessage("Toast message", ToastType.Success, durationMs: 3000);

        // Act
        handler.Receive(message);

        // Assert
        messagingServiceMock.Verify(s => s.ShowToast("Toast message", ToastType.Success, 3000), Times.Once);
    }

    [Fact]
    public void SystemNotification_ShouldShowToast_WithFormattedTitleAndMessage()
    {
        // Arrange
        var messenger = new WeakReferenceMessenger();
        var messagingServiceMock = new Mock<IMessagingService>();
        var handler = new NotificationHandler(messenger, messagingServiceMock.Object);
        var message = new SystemNotificationMessage("System message", "System Title", SystemNotificationType.Warning);

        // Act
        handler.Receive(message);

        // Assert
        messagingServiceMock.Verify(s => s.ShowToast("System Title: System message", ToastType.Warning, It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public async Task AlertDialog_ShouldSetFalse_OnUiHandlerException()
    {
        // Arrange
        var messenger = new WeakReferenceMessenger();
        var messagingServiceMock = new Mock<IMessagingService>();
        messenger.Register<ShowAlertMessage>(this, (r, msg) => throw new InvalidOperationException("UI error"));

        var handler = new NotificationHandler(messenger, messagingServiceMock.Object);
        var message = new AlertDialogMessage("Error alert", "Alert Title", "OK");

        // Act
        handler.Receive(message);
        var result = await message.ResponseSource.Task;

        // Assert
        result.Should().BeFalse();
    }
}