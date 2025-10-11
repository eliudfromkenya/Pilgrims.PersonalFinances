using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pilgrims.PersonalFinances.Core.Messaging.Interfaces;
using Pilgrims.PersonalFinances.Core.Services.Interfaces;
using Pilgrims.PersonalFinances.Core.Models;
using Pilgrims.PersonalFinances.Core.Models.Enums;
using System.Collections.ObjectModel;
using Pilgrims.PersonalFinances.Core.Models;

namespace Pilgrims.PersonalFinances.Core.ViewModels
{
    /// <summary>
    /// ViewModel for the Notifications page using MVVM pattern with messaging
    /// </summary>
    public partial class NotificationsViewModel : BaseViewModel
    {
        private readonly INotificationService _notificationService;

        [ObservableProperty]
        private ObservableCollection<TransactionNotification> notifications = new();

        [ObservableProperty]
        private ObservableCollection<TransactionNotification> filteredNotifications = new();

        [ObservableProperty]
        private string searchTerm = string.Empty;

        [ObservableProperty]
        private AppNotificationType? selectedType;

        [ObservableProperty]
        private bool showOnlyUnread = false;

        [ObservableProperty]
        private string sortBy = "Date";

        [ObservableProperty]
        private bool sortDescending = true;

        // Statistics
        [ObservableProperty]
        private int totalCount;

        [ObservableProperty]
        private int unreadCount;

        [ObservableProperty]
        private int todayCount;

        [ObservableProperty]
        private int weekCount;

        public NotificationsViewModel(
            IMessagingService messagingService,
            INotificationService notificationService) : base(messagingService)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            Title = "Notifications";
        }

        /// <summary>
        /// Initialize the ViewModel
        /// </summary>
        [RelayCommand]
        public async Task InitializeAsync()
        {
            await LoadNotificationsAsync();
        }

        /// <summary>
        /// Load all notifications
        /// </summary>
        [RelayCommand]
        public async Task LoadNotificationsAsync()
        {
            await ExecuteAsync(async () =>
            {
                var notificationList = await _notificationService.GetAllNotificationsAsync();
                
                Notifications.Clear();
                foreach (var notification in notificationList)
                {
                    Notifications.Add(notification);
                }

                CalculateStatistics();
                ApplyFiltersAndSort();
            });
        }

        /// <summary>
        /// Refresh notifications
        /// </summary>
        [RelayCommand]
        public async Task RefreshNotificationsAsync()
        {
            await LoadNotificationsAsync();
            ShowSuccessToast("Notifications refreshed");
        }

        /// <summary>
        /// Mark a notification as read
        /// </summary>
        [RelayCommand]
        public async Task MarkAsReadAsync(string? notificationId)
        {
            if (string.IsNullOrEmpty(notificationId))
                return;

            await ExecuteAsync(async () =>
            {
                await _notificationService.MarkAsReadAsync(notificationId);
                
                var notification = Notifications.FirstOrDefault(n => n.Id == notificationId);
                if (notification != null)
                {
                    notification.IsRead = true;
                    CalculateStatistics();
                    ApplyFiltersAndSort();
                }
            });
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
                
                foreach (var notification in Notifications)
                {
                    notification.IsRead = true;
                }
                
                CalculateStatistics();
                ApplyFiltersAndSort();
                ShowSuccessToast("All notifications marked as read");
            });
        }

        /// <summary>
        /// Dismiss a notification
        /// </summary>
        [RelayCommand]
        public async Task DismissNotificationAsync(string? notificationId)
        {
            if (string.IsNullOrEmpty(notificationId))
                return;

            await ExecuteAsync(async () =>
            {
                await _notificationService.DismissNotificationAsync(notificationId);
                
                var notification = Notifications.FirstOrDefault(n => n.Id == notificationId);
                if (notification != null)
                {
                    notification.IsDismissed = true;
                    CalculateStatistics();
                    ApplyFiltersAndSort();
                }
            });
        }

        /// <summary>
        /// Delete a notification
        /// </summary>
        [RelayCommand]
        public async Task DeleteNotificationAsync(string? notificationId)
        {
            if (string.IsNullOrEmpty(notificationId))
                return;

            var confirmed = await _messagingService.ShowConfirmationAsync("Are you sure you want to delete this notification?");
            if (!confirmed)
                return;

            await ExecuteAsync(async () =>
            {
                await _notificationService.DeleteNotificationAsync(notificationId);
                
                var notification = Notifications.FirstOrDefault(n => n.Id == notificationId);
                if (notification != null)
                {
                    Notifications.Remove(notification);
                    CalculateStatistics();
                    ApplyFiltersAndSort();
                    ShowSuccessToast("Notification deleted");
                }
            });
        }

        /// <summary>
        /// Navigate to related transaction
        /// </summary>
        [RelayCommand]
        public void NavigateToTransaction(string? scheduledTransactionId)
        {
            if (!string.IsNullOrEmpty(scheduledTransactionId))
            {
                Navigate($"/recurring-transactions/edit/{scheduledTransactionId}");
            }
        }

        /// <summary>
        /// Apply search filter
        /// </summary>
        [RelayCommand]
        public void ApplySearch()
        {
            ApplyFiltersAndSort();
        }

        /// <summary>
        /// Clear search filter
        /// </summary>
        [RelayCommand]
        public void ClearSearch()
        {
            SearchTerm = string.Empty;
            ApplyFiltersAndSort();
        }

        /// <summary>
        /// Apply type filter
        /// </summary>
        [RelayCommand]
        public void ApplyTypeFilter(AppNotificationType? type)
        {
            SelectedType = type;
            ApplyFiltersAndSort();
        }

        /// <summary>
        /// Toggle unread filter
        /// </summary>
        [RelayCommand]
        public void ToggleUnreadFilter()
        {
            ShowOnlyUnread = !ShowOnlyUnread;
            ApplyFiltersAndSort();
        }

        /// <summary>
        /// Apply sort
        /// </summary>
        [RelayCommand]
        public void ApplySort(string sortField)
        {
            if (SortBy == sortField)
            {
                SortDescending = !SortDescending;
            }
            else
            {
                SortBy = sortField;
                SortDescending = true;
            }
            
            ApplyFiltersAndSort();
        }

        /// <summary>
        /// Calculate notification statistics
        /// </summary>
        private void CalculateStatistics()
        {
            TotalCount = Notifications.Count;
            UnreadCount = Notifications.Count(n => !n.IsRead);
            
            var today = DateTime.Today;
            TodayCount = Notifications.Count(n => n.CreatedAt.Date == today);
            
            var weekStart = today.AddDays(-(int)today.DayOfWeek);
            WeekCount = Notifications.Count(n => n.CreatedAt.Date >= weekStart);
        }

        /// <summary>
        /// Apply filters and sorting to notifications
        /// </summary>
        private void ApplyFiltersAndSort()
        {
            var filtered = Notifications.AsEnumerable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                filtered = filtered.Where(n => 
                    n.Message.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    n.Title.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));
            }

            // Apply type filter
            if (SelectedType.HasValue)
            {
                filtered = filtered.Where(n => n.NotificationType == SelectedType.Value);
            }

            // Apply unread filter
            if (ShowOnlyUnread)
            {
                filtered = filtered.Where(n => !n.IsRead);
            }

            // Apply sorting
            filtered = SortBy switch
            {
                "Date" => SortDescending 
                    ? filtered.OrderByDescending(n => n.CreatedAt)
                    : filtered.OrderBy(n => n.CreatedAt),
                "Type" => SortDescending
                    ? filtered.OrderByDescending(n => n.NotificationType)
                    : filtered.OrderBy(n => n.NotificationType),
                "Status" => SortDescending
                    ? filtered.OrderByDescending(n => n.IsRead)
                    : filtered.OrderBy(n => n.IsRead),
                _ => filtered.OrderByDescending(n => n.CreatedAt)
            };

            FilteredNotifications.Clear();
            foreach (var notification in filtered)
            {
                FilteredNotifications.Add(notification);
            }
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
        /// Get notification icon
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
    }
}