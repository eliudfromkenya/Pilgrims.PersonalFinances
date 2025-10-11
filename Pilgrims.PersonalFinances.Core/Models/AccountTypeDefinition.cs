using System;
using Pilgrims.PersonalFinances.Core.Models;
using Pilgrims.PersonalFinances.Models;

namespace Pilgrims.PersonalFinances.Core.Models
{
    /// <summary>
    /// Database-driven account type definition with icon and display metadata.
    /// </summary>
    public class AccountTypeDefinition : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        /// <summary>
        /// Icon representation (emoji or glyph name).
        /// </summary>
        public string Icon { get; set; } = string.Empty;
        /// <summary>
        /// Maps to existing AccountType enum value for compatibility.
        /// </summary>
        public int EnumValue { get; set; }
        public bool IsActive { get; set; } = true;
    }
}