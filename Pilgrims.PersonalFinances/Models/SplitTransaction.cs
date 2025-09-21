using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pilgrims.PersonalFinances.Models
{
    /// <summary>
    /// Represents a split portion of a transaction across different categories
    /// </summary>
    public class SplitTransaction : BaseEntity
    {
        /// <summary>
        /// Reference to the parent transaction
        /// </summary>
        [Required]
        public string? TransactionId { get; set; }

        /// <summary>
        /// Amount for this split portion
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Split amount must be greater than 0")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Category for this split portion
        /// </summary>
        [Required]
        public string CategoryId { get; set; } = string.Empty;

        /// <summary>
        /// Optional description for this split portion
        /// </summary>
        [MaxLength(200)]
        public string? Description { get; set; }

        /// <summary>
        /// Optional memo for this split portion
        /// </summary>
        [MaxLength(500)]
        public string? Memo { get; set; }

        /// <summary>
        /// Tags associated with this split portion (comma-separated)
        /// </summary>
        [MaxLength(1000)]
        public string? Tags { get; set; }

        // Navigation Properties
        /// <summary>
        /// Navigation property to the parent transaction
        /// </summary>
        public virtual Pilgrims.PersonalFinances.Models.Transaction? Transaction { get; set; }

        /// <summary>
        /// Navigation property to the category
        /// </summary>
        public virtual Category? Category { get; set; }

        // Computed Properties
        /// <summary>
        /// Gets the percentage of the parent transaction this split represents
        /// </summary>
        [NotMapped]
        public decimal Percentage => Transaction?.Amount > 0 ? (Amount / Transaction.Amount) * 100 : 0;

        /// <summary>
        /// Gets the formatted amount as a string
        /// </summary>
        [NotMapped]
        public string FormattedAmount => Amount.ToString("C2");

        /// <summary>
        /// Gets the formatted percentage as a string
        /// </summary>
        [NotMapped]
        public string FormattedPercentage => $"{Percentage:F1}%";

        /// <summary>
        /// Gets the display name for this split
        /// </summary>
        [NotMapped]
        public string DisplayName => !string.IsNullOrEmpty(Description) ? Description : Category?.Name ?? "Unknown Category";

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

        // Methods
        /// <summary>
        /// Validates the split transaction
        /// </summary>
        /// <returns>List of validation errors</returns>
        public List<string> Validate()
        {
            var errors = new List<string>();

            if (Amount <= 0)
                errors.Add("Split amount must be greater than 0");

            if (string.IsNullOrEmpty(CategoryId))
                errors.Add("Category is required");

            if (!string.IsNullOrEmpty(Description) && Description.Length > 200)
                errors.Add("Description cannot exceed 200 characters");

            if (!string.IsNullOrEmpty(Memo) && Memo.Length > 500)
                errors.Add("Memo cannot exceed 500 characters");

            if (!string.IsNullOrEmpty(Tags) && Tags.Length > 1000)
                errors.Add("Tags cannot exceed 1000 characters");

            return errors;
        }

        /// <summary>
        /// Adds a tag to this split
        /// </summary>
        /// <param name="tag">Tag to add</param>
        public void AddTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag)) return;

            var currentTags = TagList;
            if (!currentTags.Contains(tag, StringComparer.OrdinalIgnoreCase))
            {
                currentTags.Add(tag.Trim());
                Tags = string.Join(", ", currentTags);
                MarkAsDirty();
            }
        }

        /// <summary>
        /// Removes a tag from this split
        /// </summary>
        /// <param name="tag">Tag to remove</param>
        public void RemoveTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag)) return;

            var currentTags = TagList;
            var removed = currentTags.RemoveAll(t => t.Equals(tag.Trim(), StringComparison.OrdinalIgnoreCase)) > 0;
            
            if (removed)
            {
                Tags = currentTags.Count > 0 ? string.Join(", ", currentTags) : null;
                MarkAsDirty();
            }
        }

        /// <summary>
        /// Creates a copy of this split transaction
        /// </summary>
        /// <returns>New split transaction with copied properties</returns>
        public SplitTransaction Clone()
        {
            return new SplitTransaction
            {
                Amount = Amount,
                CategoryId = CategoryId,
                Description = Description,
                Memo = Memo,
                Tags = Tags
            };
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