using System;
using System.ComponentModel.DataAnnotations;

namespace SimpleGoalTest
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

    public class SimpleGoal
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        public GoalType GoalType { get; set; }
        
        [Range(0.01, double.MaxValue)]
        public decimal TargetAmount { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal CurrentAmount { get; set; }
        
        public DateTime StartDate { get; set; }
        
        public DateTime TargetDate { get; set; }
        
        [Range(1, 5)]
        public int Priority { get; set; } = 3; // 1 = Highest, 5 = Lowest
        
        public string Category { get; set; } = string.Empty;
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedDate { get; set; }
        
        public DateTime UpdatedDate { get; set; }

        // Calculated properties
        public decimal ProgressPercentage => TargetAmount > 0 ? (CurrentAmount / TargetAmount) * 100 : 0;
        
        public bool IsCompleted => CurrentAmount >= TargetAmount;
        
        public decimal RemainingAmount => Math.Max(0, TargetAmount - CurrentAmount);
        
        public int DaysRemaining => (TargetDate - DateTime.Now).Days;
        
        public decimal RequiredDailyAmount => DaysRemaining > 0 ? RemainingAmount / DaysRemaining : 0;

        // Methods
        public void UpdateProgress(decimal amount)
        {
            CurrentAmount = Math.Max(0, CurrentAmount + amount);
            UpdatedDate = DateTime.Now;
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
}