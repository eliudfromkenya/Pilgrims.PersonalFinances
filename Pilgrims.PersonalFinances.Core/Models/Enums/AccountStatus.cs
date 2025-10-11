using System.ComponentModel;

namespace Pilgrims.PersonalFinances.Models.Enums
{
    /// <summary>
    /// Defines the status states of financial accounts
    /// </summary>
    public enum AccountStatus
    {
        [Description("Active")]
        Active = 1,

        [Description("Inactive")]
        Inactive = 2,

        [Description("Closed")]
        Closed = 3
    }
}