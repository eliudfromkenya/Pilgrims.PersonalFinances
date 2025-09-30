using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pilgrims.PersonalFinances.Data;
using Pilgrims.PersonalFinances.Core.Models;
using Pilgrims.PersonalFinances.Core.Services.Interfaces;

namespace Pilgrims.PersonalFinances.Core.Services;

/// <summary>
/// Service implementation for managing application settings
/// </summary>
public class ApplicationSettingsService : IApplicationSettingsService
{
    private readonly PersonalFinanceContext _context;
    private readonly ILogger<ApplicationSettingsService> _logger;
    private ApplicationSettings? _cachedSettings;

    public ApplicationSettingsService(PersonalFinanceContext context, ILogger<ApplicationSettingsService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ApplicationSettings> GetSettingsAsync()
    {
        try
        {
            if (_cachedSettings != null)
                return _cachedSettings;

            _cachedSettings = await _context.ApplicationSettings
                .FirstOrDefaultAsync(s => s.IsActive);

            if (_cachedSettings == null)
            {
                // Create default settings if none exist
                _cachedSettings = new ApplicationSettings
                {
                    Id = "default-settings",
                    DefaultCurrency = "USD",
                    DateFormat = "MM/dd/yyyy",
                    NumberFormat = "en-US",
                    Theme = "Auto",
                    ShowCurrencyCode = false,
                    CurrencyDecimalPlaces = 2,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                _context.ApplicationSettings.Add(_cachedSettings);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Created default application settings");
            }

            return _cachedSettings;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving application settings");
            throw;
        }
    }

    public async Task<ApplicationSettings> UpdateSettingsAsync(ApplicationSettings settings)
    {
        try
        {
            var existingSettings = await GetSettingsAsync();
            
            // Update properties
            foreach (var property in typeof(ApplicationSettings).GetProperties())
            {
                if (property.CanWrite && property.Name != nameof(ApplicationSettings.Id) && 
                    property.Name != nameof(ApplicationSettings.CreatedAt) && 
                    property.Name != nameof(ApplicationSettings.UpdatedAt))
                {
                    var newValue = property.GetValue(settings);
                    property.SetValue(existingSettings, newValue);
                }
            }
            
            existingSettings.MarkAsDirty();

            await _context.SaveChangesAsync();
            
            // Clear cache to force reload
            _cachedSettings = null;
            
            _logger.LogInformation("Application settings updated successfully");
            return existingSettings;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating application settings");
            throw;
        }
    }

    public async Task<ApplicationSettings> UpdateDefaultCurrencyAsync(string currencyCode)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(currencyCode))
                throw new ArgumentException("Currency code cannot be null or empty", nameof(currencyCode));

            var settings = await GetSettingsAsync();
            settings.DefaultCurrency = currencyCode.ToUpper();
            settings.MarkAsDirty();

            await _context.SaveChangesAsync();
            
            // Clear cache to force reload
            _cachedSettings = null;
            
            _logger.LogInformation("Default currency updated to {CurrencyCode}", currencyCode);
            return settings;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating default currency to {CurrencyCode}", currencyCode);
            throw;
        }
    }

    public async Task<ApplicationSettings> ResetToDefaultsAsync()
    {
        try
        {
            var settings = await GetSettingsAsync();
            
            settings.DefaultCurrency = "USD";
            settings.DateFormat = "MM/dd/yyyy";
            settings.NumberFormat = "en-US";
            settings.Theme = "Auto";
            settings.ShowCurrencyCode = false;
            settings.CurrencyDecimalPlaces = 2;
            settings.MarkAsDirty();

            await _context.SaveChangesAsync();
            
            // Clear cache to force reload
            _cachedSettings = null;
            
            _logger.LogInformation("Application settings reset to defaults");
            return settings;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting application settings to defaults");
            throw;
        }
    }

    public async Task<bool> SettingsExistAsync()
    {
        try
        {
            return await _context.ApplicationSettings
                .AnyAsync(s => s.IsActive);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if settings exist");
            return false;
        }
    }
}