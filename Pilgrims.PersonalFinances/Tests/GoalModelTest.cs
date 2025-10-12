using System;
using System.Threading.Tasks;
using Pilgrims.PersonalFinances.Models;
using Pilgrims.PersonalFinances.Core.Models;
using Pilgrims.PersonalFinances.Core.Models.Enums;
using Pilgrims.PersonalFinances.Services;
using Pilgrims.PersonalFinances.Core.Services.Interfaces;

namespace Pilgrims.PersonalFinances.Tests
{
    /// <summary>
    /// Test class to verify Goal model functionality and notification integration
    /// </summary>
    public class GoalModelTest
    {
        public static void RunTests()
        {
            Console.WriteLine("=== Goal Model Integration Tests ===\n");

            // Test 1: Goal Creation and Validation
            TestGoalCreation();

            // Test 2: Goal Progress Tracking
            TestGoalProgressTracking();

            // Test 3: Goal Completion Detection
            TestGoalCompletion();

            // Test 4: Reminder Due Logic
            TestReminderLogic();

            // Test 5: Goal Validation
            TestGoalValidation();

            Console.WriteLine("\n=== All Tests Completed ===");
        }

        private static void TestGoalCreation()
        {
            Console.WriteLine("Test 1: Goal Creation and Validation");
            
            var goal = new Goal
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Emergency Fund",
                Description = "Build 6 months of expenses",
                GoalType = GoalType.EmergencyFund,
                TargetAmount = 30000m,
                CurrentAmount = 0m,
                StartDate = DateTime.Now,
                TargetDate = DateTime.Now.AddMonths(12),
                Priority = 1,
                Category = "Safety",
                Icon = "🚨",
                Color = "#FF6B6B",
                EnableReminders = true,
                ReminderFrequencyDays = 30,
                CreatedAt = DateTime.Now
            };

            Console.WriteLine($"✓ Goal created: {goal.Name}");
            Console.WriteLine($"  Target: {goal.FormattedTargetAmount}");
            Console.WriteLine($"  Progress: {goal.ProgressPercentage}%");
            Console.WriteLine($"  Days remaining: {goal.DaysRemaining}");
            Console.WriteLine($"  Monthly target: {goal.MonthlyTargetAmount:C}");
            Console.WriteLine();
        }

        private static void TestGoalProgressTracking()
        {
            Console.WriteLine("Test 2: Goal Progress Tracking");
            
            var goal = new Goal
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Vacation Fund",
                TargetAmount = 5000m,
                CurrentAmount = 1500m,
                StartDate = DateTime.Now.AddMonths(-3),
                TargetDate = DateTime.Now.AddMonths(6),
                CreatedAt = DateTime.Now.AddMonths(-3)
            };

            Console.WriteLine($"Initial progress: {goal.ProgressPercentage}%");
            Console.WriteLine($"Is on track: {goal.IsOnTrack}");

            // Add some progress
            goal.UpdateProgress(500m);
            Console.WriteLine($"After adding $500: {goal.ProgressPercentage}%");
            Console.WriteLine($"Current amount: {goal.FormattedCurrentAmount}");
            Console.WriteLine($"Remaining: {goal.FormattedRemainingAmount}");
            Console.WriteLine();
        }

        private static void TestGoalCompletion()
        {
            Console.WriteLine("Test 3: Goal Completion Detection");
            
            var goal = new Goal
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Car Down Payment",
                TargetAmount = 5000m,
                CurrentAmount = 4800m,
                StartDate = DateTime.Now.AddMonths(-6),
                TargetDate = DateTime.Now.AddMonths(1),
                CreatedAt = DateTime.Now.AddMonths(-6)
            };

            Console.WriteLine($"Before completion: {goal.IsCompleted}");
            Console.WriteLine($"Progress: {goal.ProgressPercentage}%");

            // Complete the goal
            goal.UpdateProgress(200m);
            Console.WriteLine($"After adding final $200:");
            Console.WriteLine($"  Is completed: {goal.IsCompleted}");
            Console.WriteLine($"  Completion date: {goal.CompletedDate}");
            Console.WriteLine($"  Progress: {goal.ProgressPercentage}%");
            Console.WriteLine();
        }

        private static void TestReminderLogic()
        {
            Console.WriteLine("Test 4: Reminder Due Logic");
            
            var goal = new Goal
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Investment Goal",
                EnableReminders = true,
                ReminderFrequencyDays = 30,
                LastReminderDate = null,
                IsCompleted = false,
                CreatedAt = DateTime.Now
            };

            Console.WriteLine($"First reminder due (no previous): {goal.IsReminderDue()}");

            goal.MarkReminderSent();
            Console.WriteLine($"After marking sent: {goal.IsReminderDue()}");

            // Simulate 31 days passing
            goal.LastReminderDate = DateTime.Now.AddDays(-31);
            Console.WriteLine($"After 31 days: {goal.IsReminderDue()}");

            // Test completed goal
            goal.IsCompleted = true;
            Console.WriteLine($"Completed goal reminder due: {goal.IsReminderDue()}");
            Console.WriteLine();
        }

        private static void TestGoalValidation()
        {
            Console.WriteLine("Test 5: Goal Validation");
            
            var invalidGoal = new Goal
            {
                Name = "", // Invalid: empty name
                TargetAmount = -100m, // Invalid: negative amount
                StartDate = DateTime.Now,
                TargetDate = DateTime.Now.AddDays(-1), // Invalid: past target date
                Priority = 10, // Invalid: out of range
                CreatedAt = DateTime.Now
            };

            var validation = invalidGoal.Validate();
            Console.WriteLine($"Validation passed: {validation.IsValid}");
            
            if (!validation.IsValid)
            {
                Console.WriteLine("Validation errors:");
                foreach (var error in validation.Errors)
                {
                    Console.WriteLine($"  - {error}");
                }
            }

            // Test valid goal
            var validGoal = new Goal
            {
                Name = "Valid Goal",
                TargetAmount = 1000m,
                StartDate = DateTime.Now,
                TargetDate = DateTime.Now.AddMonths(6),
                Priority = 3,
                CreatedAt = DateTime.Now
            };

            var validValidation = validGoal.Validate();
            Console.WriteLine($"Valid goal validation: {validValidation.IsValid}");
            Console.WriteLine();
        }

        /// <summary>
        /// Simulate notification creation for goals
        /// </summary>
        public static void TestNotificationIntegration()
        {
            Console.WriteLine("=== Goal Notification Integration Test ===\n");

            var goal = new Goal
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Emergency Fund",
                Description = "Build 6 months of expenses",
                TargetAmount = 30000m,
                CurrentAmount = 24000m, // 80% complete
                TargetDate = DateTime.Now.AddDays(30),
                CreatedAt = DateTime.Now.AddMonths(-10)
            };

            // Simulate different notification scenarios
            Console.WriteLine("Notification Scenarios:");
            Console.WriteLine($"1. Goal Reminder: '{goal.Name}' needs attention");
            Console.WriteLine($"2. Progress Update: '{goal.Name}' is {goal.ProgressPercentage}% complete");
            Console.WriteLine($"3. Deadline Warning: '{goal.Name}' due in {goal.DaysRemaining} days");
            
            if (goal.ProgressPercentage >= 100)
            {
                Console.WriteLine($"4. Achievement: '{goal.Name}' completed! 🎉");
            }
            else if (goal.ProgressPercentage >= 75)
            {
                Console.WriteLine($"4. Milestone: '{goal.Name}' is almost there! ({goal.ProgressPercentage}%)");
            }

            Console.WriteLine();
        }
    }
}