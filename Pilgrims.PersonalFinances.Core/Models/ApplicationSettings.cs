using System;
using System.ComponentModel.DataAnnotations;
using Pilgrims.PersonalFinances.Core.Models;

namespace Pilgrims.PersonalFinances.Core.Models;

/// <summary>
/// Application-wide settings stored in the database
/// </summary>
public class ApplicationSettings : BaseEntity
{
    /// <summary>
    /// Default currency code for the application (ISO 4217)
    /// </summary>
    [Required]
    [StringLength(3, MinimumLength = 3)]
    public string DefaultCurrency { get; set; } = "USD";

    /// <summary>
    /// Date format preference
    /// </summary>
    [StringLength(20)]
    public string DateFormat { get; set; } = "MM/dd/yyyy";

    /// <summary>
    /// Number format preference
    /// </summary>
    [StringLength(20)]
    public string NumberFormat { get; set; } = "en-US";

    /// <summary>
    /// Theme preference (Light, Dark, Auto)
    /// </summary>
    [StringLength(10)]
    public string Theme { get; set; } = "Auto";

    /// <summary>
    /// Whether to show currency codes alongside symbols
    /// </summary>
    public bool ShowCurrencyCode { get; set; } = false;

    /// <summary>
    /// Decimal places for currency display
    /// </summary>
    public int CurrencyDecimalPlaces { get; set; } = 2;

    /// <summary>
    /// Application version when settings were last updated
    /// </summary>
    [StringLength(20)]
    public string? LastUpdatedVersion { get; set; }

    /// <summary>
    /// User ID who last modified these settings (for multi-user scenarios)
    /// </summary>
    public int? LastModifiedByUserId { get; set; }

    /// <summary>
    /// Whether this is the active settings record
    /// </summary>
    public bool IsActive { get; set; } = true;
}