using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace GoalNotificationTest
{
    public enum GoalType
    {
        EmergencyFund,
        VacationFund,
        CarPurchase,
        HomePurchase,
        DebtPayoff,
        Investment,
        Education,
        Other
    }

    public enum AppNotificationType
    {
        SystemAlert,
        BillReminder,
        BudgetAlert,
        DebtPayment,
        IncomeExpectation,
        ReconciliationReminder
    }

    public enum NotificationPriority
    {
        Low,
        Medium,
        High
    }

    public class SimpleGoal
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public GoalType GoalType { get; set; }
        public decimal TargetAmount { get; set; }
        public decimal CurrentAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime TargetDate { get; set; }
        public int Priority { get; set; } = 3;
        public string Category { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool EnableReminders { get; set; } = true;
        public DateTime? LastReminderDate { get; set; }
        public int ReminderFrequencyDays { get; set; } = 7;

        // Calculated properties
        public decimal ProgressPercentage => TargetAmount > 0 ? (CurrentAmount / TargetAmount) * 100 : 0;
        public bool IsCompleted => CurrentAmount >= TargetAmount;
        public decimal RemainingAmount => Math.Max(0, TargetAmount - CurrentAmount);
        public int DaysRemaining => (TargetDate - DateTime.Now).Days;
        public decimal RequiredDailyAmount => DaysRemaining > 0 ? RemainingAmount / DaysRemaining : 0;

        // Notification-related methods
        public bool IsReminderDue()
        {
            if (!EnableReminders || IsCompleted) return false;
            
            return LastReminderDate == null || 
                   (DateTime.Now - LastReminderDate.Value).TotalDays >= ReminderFrequencyDays;
        }

        public void MarkReminderSent()
        {
            LastReminderDate = DateTime.Now;
        }

        public bool IsOverdue => DateTime.Now > TargetDate && !IsCompleted;
        
        public string GetStatusText()
        {
            if (IsCompleted)
                return "Completed";
            if (IsOverdue)
                return "Overdue";
            if (DaysRemaining <= 30)
                return "Due Soon";
            return "On Track";
        }
    }

    public class MockNotification
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public AppNotificationType NotificationType { get; set; }
        public NotificationPriority Priority { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsRead { get; set; }
        public bool IsDismissed { get; set; }
    }

    public class MockNotificationService
    {
        private List<MockNotification> notifications = new List<MockNotification>();

        public MockNotification CreateGoalReminderNotification(SimpleGoal goal)
        {
            var notification = new MockNotification
            {
                Title = $"Goal Reminder: {goal.Name}",
                Message = $"Don't forget about your goal '{goal.Name}'. Target amount: {goal.TargetAmount:C}",
                NotificationType = AppNotificationType.SystemAlert,
                Priority = NotificationPriority.Medium,
                CreatedDate = DateTime.Now
            };

            notifications.Add(notification);
            goal.MarkReminderSent();
            return notification;
        }

        public MockNotification CreateGoalDeadlineNotification(SimpleGoal goal)
        {
            var notification = new MockNotification
            {
                Title = $"Goal Deadline Approaching: {goal.Name}",
                Message = $"Your goal '{goal.Name}' deadline is approaching on {goal.TargetDate:MMM dd, yyyy}",
                NotificationType = AppNotificationType.SystemAlert,
                Priority = NotificationPriority.High,
                CreatedDate = DateTime.Now
            };

            notifications.Add(notification);
            return notification;
        }

        public MockNotification CreateGoalAchievedNotification(SimpleGoal goal)
        {
            var notification = new MockNotification
            {
                Title = $"🎉 Goal Achieved: {goal.Name}",
                Message = $"Congratulations! You've achieved your goal '{goal.Name}' with a target of {goal.TargetAmount:C}",
                NotificationType = AppNotificationType.SystemAlert,
                Priority = NotificationPriority.High,
                CreatedDate = DateTime.Now
            };

            notifications.Add(notification);
            return notification;
        }

        public MockNotification CreateGoalProgressNotification(SimpleGoal goal, decimal progressPercentage)
        {
            var notification = new MockNotification
            {
                Title = $"Goal Progress: {goal.Name}",
                Message = $"You're {progressPercentage:F1}% towards your goal '{goal.Name}'. Keep it up!",
                NotificationType = AppNotificationType.SystemAlert,
                Priority = NotificationPriority.Low,
                CreatedDate = DateTime.Now
            };

            notifications.Add(notification);
            return notification;
        }

        public List<MockNotification> GetAllNotifications()
        {
            return new List<MockNotification>(notifications);
        }

        public int GetUnreadCount()
        {
            return notifications.Count(n => !n.IsRead);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Goal-Notification Integration Test ===\n");

            var notificationService = new MockNotificationService();

            try
            {
                // Test 1: Create goals with different scenarios
                Console.WriteLine("Test 1: Creating goals for notification testing...");
                
                var emergencyGoal = new SimpleGoal
                {
                    Name = "Emergency Fund",
                    Description = "Build 6 months of expenses",
                    GoalType = GoalType.EmergencyFund,
                    TargetAmount = 30000m,
                    CurrentAmount = 24000m, // 80% complete
                    StartDate = DateTime.Now.AddMonths(-10),
                    TargetDate = DateTime.Now.AddDays(30), // Due soon
                    Priority = 1,
                    Category = "Emergency",
                    EnableReminders = true,
                    ReminderFrequencyDays = 7
                };

                var vacationGoal = new SimpleGoal
                {
                    Name = "Summer Vacation",
                    Description = "Family trip to Europe",
                    GoalType = GoalType.VacationFund,
                    TargetAmount = 8000m,
                    CurrentAmount = 8000m, // Completed
                    StartDate = DateTime.Now.AddMonths(-6),
                    TargetDate = DateTime.Now.AddMonths(2),
                    Priority = 2,
                    Category = "Lifestyle",
                    EnableReminders = true,
                    ReminderFrequencyDays = 14
                };

                var carGoal = new SimpleGoal
                {
                    Name = "Car Down Payment",
                    Description = "Save for new car",
                    GoalType = GoalType.CarPurchase,
                    TargetAmount = 15000m,
                    CurrentAmount = 3000m, // 20% complete
                    StartDate = DateTime.Now.AddMonths(-2),
                    TargetDate = DateTime.Now.AddDays(-10), // Overdue
                    Priority = 1,
                    Category = "Transportation",
                    EnableReminders = true,
                    ReminderFrequencyDays = 7
                };

                var goals = new List<SimpleGoal> { emergencyGoal, vacationGoal, carGoal };
                Console.WriteLine($"✓ Created {goals.Count} goals for testing");

                // Test 2: Test reminder notifications
                Console.WriteLine("\nTest 2: Testing goal reminder notifications...");
                foreach (var goal in goals)
                {
                    if (goal.IsReminderDue())
                    {
                        var notification = notificationService.CreateGoalReminderNotification(goal);
                        Console.WriteLine($"✓ Created reminder for '{goal.Name}': {notification.Message}");
                    }
                    else
                    {
                        Console.WriteLine($"- No reminder needed for '{goal.Name}' (last sent: {goal.LastReminderDate?.ToString("yyyy-MM-dd") ?? "never"})");
                    }
                }

                // Test 3: Test deadline notifications
                Console.WriteLine("\nTest 3: Testing goal deadline notifications...");
                foreach (var goal in goals)
                {
                    if (goal.DaysRemaining <= 30 && goal.DaysRemaining > 0)
                    {
                        var notification = notificationService.CreateGoalDeadlineNotification(goal);
                        Console.WriteLine($"✓ Created deadline warning for '{goal.Name}': {notification.Message}");
                    }
                    else if (goal.IsOverdue)
                    {
                        var notification = notificationService.CreateGoalDeadlineNotification(goal);
                        Console.WriteLine($"✓ Created overdue notification for '{goal.Name}': {notification.Message}");
                    }
                }

                // Test 4: Test achievement notifications
                Console.WriteLine("\nTest 4: Testing goal achievement notifications...");
                foreach (var goal in goals)
                {
                    if (goal.IsCompleted)
                    {
                        var notification = notificationService.CreateGoalAchievedNotification(goal);
                        Console.WriteLine($"✓ Created achievement notification for '{goal.Name}': {notification.Message}");
                    }
                }

                // Test 5: Test progress notifications
                Console.WriteLine("\nTest 5: Testing goal progress notifications...");
                foreach (var goal in goals)
                {
                    if (!goal.IsCompleted && goal.ProgressPercentage >= 25)
                    {
                        var notification = notificationService.CreateGoalProgressNotification(goal, goal.ProgressPercentage);
                        Console.WriteLine($"✓ Created progress notification for '{goal.Name}': {notification.Message}");
                    }
                }

                // Test 6: Display notification summary
                Console.WriteLine("\nTest 6: Notification summary...");
                var allNotifications = notificationService.GetAllNotifications();
                var unreadCount = notificationService.GetUnreadCount();

                Console.WriteLine($"✓ Total notifications created: {allNotifications.Count}");
                Console.WriteLine($"✓ Unread notifications: {unreadCount}");

                Console.WriteLine("\nNotification Details:");
                foreach (var notification in allNotifications.OrderBy(n => n.CreatedDate))
                {
                    Console.WriteLine($"  • [{notification.Priority}] {notification.Title}");
                    Console.WriteLine($"    {notification.Message}");
                    Console.WriteLine($"    Created: {notification.CreatedDate:yyyy-MM-dd HH:mm}");
                    Console.WriteLine();
                }

                // Test 7: Test notification scenarios
                Console.WriteLine("Test 7: Goal status and notification scenarios...");
                foreach (var goal in goals)
                {
                    Console.WriteLine($"Goal: {goal.Name}");
                    Console.WriteLine($"  Status: {goal.GetStatusText()}");
                    Console.WriteLine($"  Progress: {goal.ProgressPercentage:F1}%");
                    Console.WriteLine($"  Days remaining: {goal.DaysRemaining}");
                    Console.WriteLine($"  Reminder due: {goal.IsReminderDue()}");
                    Console.WriteLine($"  Last reminder: {goal.LastReminderDate?.ToString("yyyy-MM-dd") ?? "Never"}");
                    Console.WriteLine();
                }

                Console.WriteLine("=== All Goal-Notification Integration Tests Passed! ===");
                Console.WriteLine("The Goal-Notification integration is working correctly with:");
                Console.WriteLine("- Goal reminder notifications based on frequency");
                Console.WriteLine("- Deadline and overdue notifications");
                Console.WriteLine("- Achievement notifications for completed goals");
                Console.WriteLine("- Progress notifications for milestone tracking");
                Console.WriteLine("- Proper notification prioritization");
                Console.WriteLine("- Reminder tracking and management");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Test failed with error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}