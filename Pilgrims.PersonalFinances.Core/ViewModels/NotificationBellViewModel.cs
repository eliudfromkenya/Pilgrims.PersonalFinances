using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pilgrims.PersonalFinances.Core.Messaging.Interfaces;
using Pilgrims.PersonalFinances.Core.Services.Interfaces;
using Pilgrims.PersonalFinances.Core.Models;
using Pilgrims.PersonalFinances.Core.Models.Enums;
using System.Collections.ObjectModel;
using System.Timers;

namespace Pilgrims.PersonalFinances.Core.ViewModels
{
    /// <summary>
    /// ViewModel for the NotificationBell component using MVVM pattern with messaging
    /// </summary>
    public partial class NotificationBellViewModel : BaseViewModel, IDisposable
    {
        private readonly INotificationService _notificationService;
        private System.Timers.Timer? _refreshTimer;

        [ObservableProperty]
        private ObservableCollection<TransactionNotification> recentNotifications = new();

        [ObservableProperty]
        private int unreadCount;

        [ObservableProperty]
        private bool showDropdown = false;

        public NotificationBellViewModel(
            IMessagingService messagingService,
            INotificationService notificationService) : base(messagingService)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            Title = "Notifications";
        }

        /// <summary>
        /// Initialize the ViewModel and start refresh timer
        /// </summary>
        [RelayCommand]
        public async Task InitializeAsync()
        {
            await LoadNotificationsAsync();
            StartRefreshTimer();
        }

        /// <summary>
        /// Load recent notifications
        /// </summary>
        [RelayCommand]
        public async Task LoadNotificationsAsync()
        {
            try
            {
                var notifications = await _notificationService.GetRecentNotificationsAsync("10");
                var unreadCountValue = await _notificationService.GetUnreadCountAsync();

                RecentNotifications.Clear();
                foreach (var notification in notifications)
                {
                    RecentNotifications.Add(notification);
                }

                UnreadCount = unreadCountValue;
            }
            catch (Exception ex)
            {
                // Silently handle errors in background refresh to avoid disrupting UI
                Console.WriteLine($"Error loading notifications: {ex.Message}");
            }
        }

        /// <summary>
        /// Toggle dropdown visibility
        /// </summary>
        [RelayCommand]
        public void ToggleDropdown()
        {
            ShowDropdown = !ShowDropdown;
        }

        /// <summary>
        /// Close dropdown
        /// </summary>
        [RelayCommand]
        public void CloseDropdown()
        {
            ShowDropdown = false;
        }

        /// <summary>
        /// Handle notification click
        /// </summary>
        [RelayCommand]
        public async Task HandleNotificationClickAsync(TransactionNotification notification)
        {
            // Mark as read if not already read
            if (!notification.IsRead)
            {
                await ExecuteAsync(async () =>
                {
                    await _notificationService.MarkAsReadAsync(notification.Id);
                    notification.IsRead = true;
                    UnreadCount = Math.Max(0, UnreadCount - 1);
                });
            }

            // Navigate based on notification type
            if (notification.ScheduledTransaction != null)
            {
                Navigate($"/recurring-transactions/edit/{notification.ScheduledTransaction.Id}");
            }
            else
            {
                Navigate("/notifications");
            }

            CloseDropdown();
        }

        /// <summary>
        /// Mark all notifications as read
        /// </summary>
        [RelayCommand]
        public async Task MarkAllAsReadAsync()
        {
            await ExecuteAsync(async () =>
            {
                await _notificationService.MarkAllAsReadAsync();
                
                foreach (var notification in RecentNotifications)
                {
                    notification.IsRead = true;
                }
                
                UnreadCount = 0;
                ShowSuccessToast("All notifications marked as read");
            });
        }

        /// <summary>
        /// View all notifications
        /// </summary>
        [RelayCommand]
        public void ViewAllNotifications()
        {
            Navigate("/notifications");
            CloseDropdown();
        }

        /// <summary>
        /// Get notification icon based on type
        /// </summary>
        public string GetNotificationIcon(AppNotificationType type)
        {
            return type switch
            {
                AppNotificationType.ScheduledTransactionDue => "⏰",
                AppNotificationType.BudgetExceeded => "⚠️",
                AppNotificationType.LowBalance => "💰",
                AppNotificationType.RecurringTransactionFailed => "❌",
                AppNotificationType.SystemUpdate => "🔄",
                _ => "📢"
            };
        }

        /// <summary>
        /// Get notification type display name
        /// </summary>
        public string GetNotificationTypeDisplayName(AppNotificationType type)
        {
            return type switch
            {
                AppNotificationType.ScheduledTransactionDue => "Transaction Due",
                AppNotificationType.BudgetExceeded => "Budget Exceeded",
                AppNotificationType.LowBalance => "Low Balance",
                AppNotificationType.RecurringTransactionFailed => "Transaction Failed",
                AppNotificationType.SystemUpdate => "System Update",
                _ => type.ToString()
            };
        }

        /// <summary>
        /// Get relative time display
        /// </summary>
        public string GetRelativeTime(DateTime dateTime)
        {
            var timeSpan = DateTime.Now - dateTime;

            if (timeSpan.TotalMinutes < 1)
                return "Just now";
            if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes}m ago";
            if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours}h ago";
            if (timeSpan.TotalDays < 7)
                return $"{(int)timeSpan.TotalDays}d ago";
            
            return dateTime.ToString("MMM dd");
        }

        /// <summary>
        /// Start the refresh timer for periodic updates
        /// </summary>
        private void StartRefreshTimer()
        {
            _refreshTimer = new System.Timers.Timer(30000); // 30 seconds
            _refreshTimer.Elapsed += async (sender, e) => await LoadNotificationsAsync();
            _refreshTimer.AutoReset = true;
            _refreshTimer.Start();
        }

        /// <summary>
        /// Stop the refresh timer
        /// </summary>
        private void StopRefreshTimer()
        {
            _refreshTimer?.Stop();
            _refreshTimer?.Dispose();
            _refreshTimer = null;
        }

        /// <summary>
        /// Dispose resources
        /// </summary>
        public void Dispose()
        {
            StopRefreshTimer();
        }
    }
}