using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pilgrims.PersonalFinances.Models
{
    public class BudgetCategory : BaseEntity
    {
        /// <summary>
        /// Budget ID this category belongs to
        /// </summary>
        [Required]
        public string BudgetId { get; set; } = string.Empty;

        /// <summary>
        /// Category ID for category-based budgets
        /// </summary>
        public string? CategoryId { get; set; }

        /// <summary>
        /// Account ID for account-based budgets
        /// </summary>
        public string? AccountId { get; set; }

        /// <summary>
        /// Tag name for tag-based budgets
        /// </summary>
        [MaxLength(100)]
        public string? TagName { get; set; }

        /// <summary>
        /// Allocated amount for this category within the budget
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal AllocatedAmount { get; set; }

        /// <summary>
        /// Amount spent in this category
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal SpentAmount { get; set; }

        // Navigation properties
        /// <summary>
        /// Budget this category belongs to
        /// </summary>
        public virtual Budget Budget { get; set; } = null!;

        /// <summary>
        /// Category for category-based budgets
        /// </summary>
        public virtual Category? Category { get; set; }

        /// <summary>
        /// Account for account-based budgets
        /// </summary>
        public virtual Account? Account { get; set; }

        // Computed properties
        /// <summary>
        /// Remaining amount in this category
        /// </summary>
        [NotMapped]
        public decimal RemainingAmount => AllocatedAmount - SpentAmount;

        /// <summary>
        /// Percentage of allocated amount used
        /// </summary>
        [NotMapped]
        public decimal UsedPercentage => AllocatedAmount > 0 ? (SpentAmount / AllocatedAmount) * 100 : 0;

        /// <summary>
        /// Whether this category is over budget
        /// </summary>
        [NotMapped]
        public bool IsOverBudget => SpentAmount > AllocatedAmount;

        /// <summary>
        /// Formatted allocated amount
        /// </summary>
        [NotMapped]
        public string FormattedAllocatedAmount => AllocatedAmount.ToString("C2");

        /// <summary>
        /// Formatted spent amount
        /// </summary>
        [NotMapped]
        public string FormattedSpentAmount => SpentAmount.ToString("C2");

        /// <summary>
        /// Formatted remaining amount
        /// </summary>
        [NotMapped]
        public string FormattedRemainingAmount => RemainingAmount.ToString("C2");

        /// <summary>
        /// Display name for this budget category
        /// </summary>
        [NotMapped]
        public string DisplayName
        {
            get
            {
                if (Category != null)
                    return Category.Name;
                if (Account != null)
                    return Account.Name;
                if (!string.IsNullOrEmpty(TagName))
                    return $"Tag: {TagName}";
                return "Unknown";
            }
        }

        /// <summary>
        /// Budget category type display
        /// </summary>
        [NotMapped]
        public string TypeDisplay
        {
            get
            {
                if (!string.IsNullOrEmpty(CategoryId))
                    return "Category";
                if (!string.IsNullOrEmpty(AccountId))
                    return "Account";
                if (!string.IsNullOrEmpty(TagName))
                    return "Tag";
                return "Unknown";
            }
        }

        // Methods
        /// <summary>
        /// Updates the spent amount for this category
        /// </summary>
        /// <param name="amount">Amount to add to spent total</param>
        public void AddSpentAmount(decimal amount)
        {
            SpentAmount += amount;
            MarkAsDirty();
        }

        /// <summary>
        /// Resets the spent amount for this category
        /// </summary>
        public void ResetSpentAmount()
        {
            SpentAmount = 0;
            MarkAsDirty();
        }

        /// <summary>
        /// Validates the budget category
        /// </summary>
        /// <returns>List of validation errors</returns>
        public List<string> Validate()
        {
            var errors = new List<string>();

            // Must have exactly one target (category, account, or tag)
            var targetCount = 0;
            if (!string.IsNullOrEmpty(CategoryId)) targetCount++;
            if (!string.IsNullOrEmpty(AccountId)) targetCount++;
            if (!string.IsNullOrEmpty(TagName)) targetCount++;

            if (targetCount == 0)
                errors.Add("Budget category must target a category, account, or tag");
            else if (targetCount > 1)
                errors.Add("Budget category can only target one of: category, account, or tag");

            // Allocated amount must be positive
            if (AllocatedAmount <= 0)
                errors.Add("Allocated amount must be greater than zero");

            // Tag name validation
            if (!string.IsNullOrEmpty(TagName) && TagName.Length > 100)
                errors.Add("Tag name cannot exceed 100 characters");

            return errors;
        }

        /// <summary>
        /// Creates a budget category for a specific category
        /// </summary>
        /// <param name="budgetId">Budget ID</param>
        /// <param name="categoryId">Category ID</param>
        /// <param name="allocatedAmount">Allocated amount</param>
        /// <returns>Budget category instance</returns>
        public static BudgetCategory ForCategory(string budgetId, string categoryId, decimal allocatedAmount)
        {
            return new BudgetCategory
            {
                BudgetId = budgetId,
                CategoryId = categoryId,
                AllocatedAmount = allocatedAmount
            };
        }

        /// <summary>
        /// Creates a budget category for a specific account
        /// </summary>
        /// <param name="budgetId">Budget ID</param>
        /// <param name="accountId">Account ID</param>
        /// <param name="allocatedAmount">Allocated amount</param>
        /// <returns>Budget category instance</returns>
        public static BudgetCategory ForAccount(string budgetId, string accountId, decimal allocatedAmount)
        {
            return new BudgetCategory
            {
                BudgetId = budgetId,
                AccountId = accountId,
                AllocatedAmount = allocatedAmount
            };
        }

        /// <summary>
        /// Creates a budget category for a specific tag
        /// </summary>
        /// <param name="budgetId">Budget ID</param>
        /// <param name="tagName">Tag name</param>
        /// <param name="allocatedAmount">Allocated amount</param>
        /// <returns>Budget category instance</returns>
        public static BudgetCategory ForTag(string budgetId, string tagName, decimal allocatedAmount)
        {
            return new BudgetCategory
            {
                BudgetId = budgetId,
                TagName = tagName,
                AllocatedAmount = allocatedAmount
            };
        }
    }
}