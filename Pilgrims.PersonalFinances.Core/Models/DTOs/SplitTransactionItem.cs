using System.ComponentModel.DataAnnotations;

namespace Pilgrims.PersonalFinances.Core.Models.DTOs
{
    /// <summary>
    /// DTO for split transaction items used in UI components
    /// </summary>
    public class SplitTransactionItem
    {
        /// <summary>
        /// Unique identifier for this split item
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Amount for this split portion
        /// </summary>
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Split amount must be greater than 0")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Category ID for this split portion
        /// </summary>
        [Required]
        public string CategoryId { get; set; } = string.Empty;

        /// <summary>
        /// Optional description for this split portion
        /// </summary>
        [MaxLength(200)]
        public string Description { get; set; } = string.Empty;

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

        /// <summary>
        /// Gets the percentage of the total amount this split represents
        /// </summary>
        public decimal GetPercentage(decimal totalAmount)
        {
            return totalAmount > 0 ? (Amount / totalAmount) * 100 : 0;
        }

        /// <summary>
        /// Gets or sets the formatted amount as a string (populated by currency service)
        /// </summary>
        public string FormattedAmount { get; set; } = string.Empty;

        /// <summary>
        /// Gets the tags as a list
        /// </summary>
        public List<string> TagList => string.IsNullOrEmpty(Tags) 
            ? new List<string>() 
            : Tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                  .Select(t => t.Trim())
                  .Where(t => !string.IsNullOrEmpty(t))
                  .ToList();

        /// <summary>
        /// Creates a copy of this split transaction item
        /// </summary>
        public SplitTransactionItem Clone()
        {
            return new SplitTransactionItem
            {
                Amount = Amount,
                CategoryId = CategoryId,
                Description = Description,
                Memo = Memo,
                Tags = Tags
            };
        }
    }
}
