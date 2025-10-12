using System.ComponentModel.DataAnnotations;
using Pilgrims.PersonalFinances.Core.Models;

namespace Pilgrims.PersonalFinances.Core.Models
{
    public class Currency : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Country { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string CurrencyName { get; set; } = string.Empty;

        [Required]
        [MaxLength(3)]
        public string ISOCode { get; set; } = string.Empty;

        [MaxLength(10)]
        public string SymbolOrSign { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}