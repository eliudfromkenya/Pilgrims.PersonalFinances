using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pilgrims.PersonalFinances.Models.Enums;

namespace Pilgrims.PersonalFinances.Models
{
    /// <summary>
    /// Represents a transaction template for quick transaction creation
    /// </summary>
    public class TransactionTemplate : BaseEntity
    {
        /// <summary>
        /// Template name
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Template description
        /// </summary>
        [MaxLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// Transaction type
        /// </summary>
        [Required]
        public TransactionType Type { get; set; }

        /// <summary>
        /// Default amount for the template
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Default category ID
        /// </summary>
        public string? CategoryId { get; set; }

        /// <summary>
        /// Default payee/payer name
        /// </summary>
        [MaxLength(200)]
        public string? Payee { get; set; }

        /// <summary>
        /// Default tags (comma-separated)
        /// </summary>
        [MaxLength(1000)]
        public string? Tags { get; set; }

        /// <summary>
        /// Number of times this template has been used
        /// </summary>
        public int UsageCount { get; set; } = 0;

        /// <summary>
        /// Whether this template is active
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Sort order for display
        /// </summary>
        public int SortOrder { get; set; } = 0;

        // Navigation Properties
        /// <summary>
        /// Navigation property to the category
        /// </summary>
        public virtual Category? Category { get; set; }

        // Computed Properties
        /// <summary>
        /// Gets the tags as a list
        /// </summary>
        [NotMapped]
        public List<string> TagList => string.IsNullOrEmpty(Tags) 
            ? new List<string>() 
            : Tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                  .Select(t => t.Trim())
                  .Where(t => !string.IsNullOrEmpty(t))
                  .ToList();

        /// <summary>
        /// Gets or sets the formatted amount as a string (populated by currency service)
        /// </summary>
        [NotMapped]
        public string FormattedAmount { get; set; } = string.Empty;

        /// <summary>
        /// Gets the display name for this template
        /// </summary>
        [NotMapped]
        public string DisplayName => !string.IsNullOrEmpty(Name) ? Name : "Unnamed Template";

        /// <summary>
        /// Gets the type icon for display
        /// </summary>
        [NotMapped]
        public string TypeIcon => Type switch
        {
            TransactionType.Income => "💰",
            TransactionType.Expense => "💸",
            TransactionType.Transfer => "🔄",
            TransactionType.Adjustment => "⚖️",
            TransactionType.InitialBalance => "🏦",
            _ => "📝"
        };

        /// <summary>
        /// Gets the type color class for display
        /// </summary>
        [NotMapped]
        public string TypeColorClass => Type switch
        {
            TransactionType.Income => "bg-green-100 dark:bg-green-900/20 text-green-600 dark:text-green-400",
            TransactionType.Expense => "bg-red-100 dark:bg-red-900/20 text-red-600 dark:text-red-400",
            TransactionType.Transfer => "bg-blue-100 dark:bg-blue-900/20 text-blue-600 dark:text-blue-400",
            TransactionType.Adjustment => "bg-purple-100 dark:bg-purple-900/20 text-purple-600 dark:text-purple-400",
            TransactionType.InitialBalance => "bg-gray-100 dark:bg-gray-900/20 text-gray-600 dark:text-gray-400",
            _ => "bg-gray-100 dark:bg-gray-700 text-gray-600 dark:text-gray-400"
        };

        /// <summary>
        /// Creates a copy of this template
        /// </summary>
        public TransactionTemplate Clone()
        {
            return new TransactionTemplate
            {
                Name = Name,
                Description = Description,
                Type = Type,
                Amount = Amount,
                CategoryId = CategoryId,
                Payee = Payee,
                Tags = Tags,
                IsActive = IsActive,
                SortOrder = SortOrder
            };
        }

        /// <summary>
        /// Increments the usage count
        /// </summary>
        public void IncrementUsage()
        {
            UsageCount++;
            MarkAsDirty();
        }

        /// <summary>
        /// Updates the modified date
        /// </summary>
        public void Touch()
        {
            MarkAsDirty();
        }
    }
}