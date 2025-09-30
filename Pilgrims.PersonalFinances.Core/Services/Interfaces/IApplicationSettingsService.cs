using Pilgrims.PersonalFinances.Core.Models;

namespace Pilgrims.PersonalFinances.Core.Services.Interfaces;

/// <summary>
/// Service interface for managing application settings
/// </summary>
public interface IApplicationSettingsService
{
    /// <summary>
    /// Get the current application settings
    /// </summary>
    /// <returns>Current application settings</returns>
    Task<ApplicationSettings> GetSettingsAsync();

    /// <summary>
    /// Update application settings
    /// </summary>
    /// <param name="settings">Settings to update</param>
    /// <returns>Updated settings</returns>
    Task<ApplicationSettings> UpdateSettingsAsync(ApplicationSettings settings);

    /// <summary>
    /// Update only the default currency
    /// </summary>
    /// <param name="currencyCode">New default currency code</param>
    /// <returns>Updated settings</returns>
    Task<ApplicationSettings> UpdateDefaultCurrencyAsync(string currencyCode);

    /// <summary>
    /// Reset settings to default values
    /// </summary>
    /// <returns>Reset settings</returns>
    Task<ApplicationSettings> ResetToDefaultsAsync();

    /// <summary>
    /// Check if settings exist in the database
    /// </summary>
    /// <returns>True if settings exist, false otherwise</returns>
    Task<bool> SettingsExistAsync();
}