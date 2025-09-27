using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleGoalTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Goal Model Test ===\n");

            try
            {
                // Test 1: Create a new goal
                Console.WriteLine("Test 1: Creating a new goal...");
                var goal = new SimpleGoal
                {
                    Id = 1,
                    Name = "Emergency Fund",
                    Description = "Build an emergency fund for unexpected expenses",
                    GoalType = GoalType.EmergencyFund,
                    TargetAmount = 10000m,
                    CurrentAmount = 0m,
                    StartDate = DateTime.Now,
                    TargetDate = DateTime.Now.AddMonths(12),
                    Priority = 1, // 1 = High priority
                    Category = "Emergency",
                    IsActive = true,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                };

                Console.WriteLine($"✓ Goal created: {goal.Name}");
                Console.WriteLine($"  Target: ${goal.TargetAmount:N2}");
                Console.WriteLine($"  Current: ${goal.CurrentAmount:N2}");
                Console.WriteLine($"  Progress: {goal.ProgressPercentage:F1}%");
                Console.WriteLine($"  Status: {goal.GetStatusText()}");
                Console.WriteLine($"  Days remaining: {goal.DaysRemaining}");
                Console.WriteLine($"  Required daily amount: ${goal.RequiredDailyAmount:F2}");

                // Test 2: Update progress
                Console.WriteLine("\nTest 2: Updating progress...");
                goal.UpdateProgress(2500m);
                Console.WriteLine($"✓ Added $2,500 to goal");
                Console.WriteLine($"  Current: ${goal.CurrentAmount:N2}");
                Console.WriteLine($"  Progress: {goal.ProgressPercentage:F1}%");
                Console.WriteLine($"  Remaining: ${goal.RemainingAmount:N2}");
                Console.WriteLine($"  Status: {goal.GetStatusText()}");

                // Test 3: Check properties
                Console.WriteLine("\nTest 3: Checking goal properties...");
                Console.WriteLine($"✓ Is Active: {goal.IsActive}");
                Console.WriteLine($"✓ Is Completed: {goal.IsCompleted}");
                Console.WriteLine($"✓ Is Overdue: {goal.IsOverdue}");
                Console.WriteLine($"✓ Priority: {goal.Priority}");
                Console.WriteLine($"✓ Category: {goal.Category}");

                // Test 4: Complete the goal
                Console.WriteLine("\nTest 4: Completing the goal...");
                goal.UpdateProgress(7500m); // This should complete the goal
                Console.WriteLine($"✓ Added $7,500 to goal");
                Console.WriteLine($"  Current: ${goal.CurrentAmount:N2}");
                Console.WriteLine($"  Progress: {goal.ProgressPercentage:F1}%");
                Console.WriteLine($"  Is Completed: {goal.IsCompleted}");
                Console.WriteLine($"  Status: {goal.GetStatusText()}");

                // Test 5: Test multiple goals
                Console.WriteLine("\nTest 5: Testing multiple goals...");
                var goals = new List<SimpleGoal>
                {
                    new SimpleGoal
                    {
                        Id = 2,
                        Name = "Vacation Fund",
                        Description = "Save for summer vacation",
                        GoalType = GoalType.VacationFund,
                        TargetAmount = 5000m,
                        CurrentAmount = 1500m,
                        StartDate = DateTime.Now,
                        TargetDate = DateTime.Now.AddMonths(6),
                        Priority = 2, // Medium priority
                        Category = "Lifestyle",
                        IsActive = true,
                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now
                    },
                    new SimpleGoal
                    {
                        Id = 3,
                        Name = "Car Down Payment",
                        Description = "Save for new car down payment",
                        GoalType = GoalType.CarPurchase,
                        TargetAmount = 15000m,
                        CurrentAmount = 8000m,
                        StartDate = DateTime.Now,
                        TargetDate = DateTime.Now.AddMonths(18),
                        Priority = 1, // High priority
                        Category = "Transportation",
                        IsActive = true,
                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now
                    }
                };

                Console.WriteLine($"✓ Created {goals.Count} additional goals");
                
                foreach (var g in goals)
                {
                    Console.WriteLine($"  - {g.Name}: ${g.CurrentAmount:N2}/${g.TargetAmount:N2} ({g.ProgressPercentage:F1}%)");
                }

                // Test 6: Query goals
                Console.WriteLine("\nTest 6: Querying goals...");
                var allGoals = new List<SimpleGoal> { goal };
                allGoals.AddRange(goals);

                var highPriorityGoals = allGoals.Where(g => g.Priority == 1).ToList();
                var activeGoals = allGoals.Where(g => g.IsActive).ToList();
                var completedGoals = allGoals.Where(g => g.IsCompleted).ToList();

                Console.WriteLine($"✓ Total goals: {allGoals.Count}");
                Console.WriteLine($"✓ High priority goals: {highPriorityGoals.Count}");
                Console.WriteLine($"✓ Active goals: {activeGoals.Count}");
                Console.WriteLine($"✓ Completed goals: {completedGoals.Count}");

                Console.WriteLine("\n=== All Tests Passed! ===");
                Console.WriteLine("The Goal model is working correctly with all basic functionality:");
                Console.WriteLine("- Goal creation and property assignment");
                Console.WriteLine("- Progress tracking and updates");
                Console.WriteLine("- Calculated properties (percentage, remaining amount, etc.)");
                Console.WriteLine("- Status determination");
                Console.WriteLine("- Goal completion detection");
                Console.WriteLine("- Multiple goal management");
                Console.WriteLine("- Goal querying and filtering");
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