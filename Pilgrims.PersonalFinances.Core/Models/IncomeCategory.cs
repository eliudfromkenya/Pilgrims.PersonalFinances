using System.ComponentModel.DataAnnotations;

namespace Pilgrims.PersonalFinances.Core.Models
{
    public class IncomeCategory : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(50)]
        public string Color { get; set; } = "#4CAF50"; // Default green color

        [StringLength(50)]
        public string Icon { get; set; } = "💰"; // Default money icon

        public bool IsActive { get; set; } = true;

        // Navigation property
        public virtual ICollection<Income> Incomes { get; set; } = new List<Income>();
    }
}
