using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Pilgrims.PersonalFinances.Core.Localization.Interfaces;
using Pilgrims.PersonalFinances.Core.Localization.Resources;
using System.Globalization;

namespace Pilgrims.PersonalFinances.Core.Localization.Services
{
    /// <summary>
    /// Implementation of localization service providing string localization and culture management
    /// </summary>
    public class LocalizationService : ILocalizationService
    {
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly ILogger<LocalizationService> _logger;
        private CultureInfo _currentCulture;

        /// <summary>
        /// Supported cultures for the application
        /// </summary>
        private static readonly CultureInfo[] SupportedCultures = new[]
        {
            new CultureInfo("en"), // English (default)
            new CultureInfo("sw"), // Swahili
            new CultureInfo("fr"), // French
            new CultureInfo("de"), // German
            new CultureInfo("es")  // Spanish
        };

        public event EventHandler<CultureInfo>? CultureChanged;

        public LocalizationService(IStringLocalizer<SharedResources> localizer, ILogger<LocalizationService> logger)
        {
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            // Initialize with current culture, but this will be updated by CultureManager.InitializeAsync()
            _currentCulture = CultureInfo.CurrentCulture;
        }

        /// <summary>
        /// Initializes the localization service with the current culture
        /// This should be called after CultureManager.InitializeAsync() to sync the culture
        /// </summary>
        public void SyncWithCurrentCulture()
        {
            _currentCulture = CultureInfo.CurrentCulture;
            _logger.LogInformation("LocalizationService synced with current culture: {Culture}", _currentCulture.Name);
        }

        public string GetString(string key)
        {
            try
            {
                var localizedString = _localizer[key];
                if (localizedString.ResourceNotFound)
                {
                    _logger.LogWarning("Resource not found for key: {Key}", key);
                    return key; // Return the key itself if resource not found
                }
                return localizedString.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting localized string for key: {Key}", key);
                return key;
            }
        }

        public string GetString(string key, params object[] parameters)
        {
            try
            {
                var localizedString = _localizer[key, parameters];
                if (localizedString.ResourceNotFound)
                {
                    _logger.LogWarning("Resource not found for key: {Key}", key);
                    return string.Format(key, parameters); // Return formatted key if resource not found
                }
                return localizedString.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting localized string for key: {Key} with parameters", key);
                return string.Format(key, parameters);
            }
        }

        public IEnumerable<CultureInfo> GetAvailableCultures()
        {
            return SupportedCultures;
        }

        public CultureInfo GetCurrentCulture()
        {
            return _currentCulture;
        }

        public async Task SetCultureAsync(string culture)
        {
            try
            {
                var cultureInfo = new CultureInfo(culture);
                await SetCultureAsync(cultureInfo);
            }
            catch (CultureNotFoundException ex)
            {
                _logger.LogError(ex, "Invalid culture: {Culture}", culture);
                throw;
            }
        }

        public Task SetCultureAsync(CultureInfo cultureInfo)
        {
            try
            {
                // Validate that the culture is supported
                if (!SupportedCultures.Any(c => c.Name.Equals(cultureInfo.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    _logger.LogWarning("Unsupported culture: {Culture}. Falling back to English.", cultureInfo.Name);
                    cultureInfo = new CultureInfo("en");
                }

                var previousCulture = _currentCulture;
                _currentCulture = cultureInfo;

                // Set the culture for the current thread
                CultureInfo.CurrentCulture = cultureInfo;
                CultureInfo.CurrentUICulture = cultureInfo;

                _logger.LogInformation("Culture changed from {PreviousCulture} to {NewCulture}", 
                    previousCulture.Name, cultureInfo.Name);

                // Raise the culture changed event
                CultureChanged?.Invoke(this, cultureInfo);

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting culture to: {Culture}", cultureInfo.Name);
                throw;
            }
        }

        public string FormatNumber(decimal number)
        {
            try
            {
                return number.ToString("N", _currentCulture);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error formatting number: {Number}", number);
                return number.ToString();
            }
        }

        public string FormatCurrency(decimal amount, string? currencyCode = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(currencyCode))
                {
                    // Create a custom number format with the specified currency
                    var numberFormat = (NumberFormatInfo)_currentCulture.NumberFormat.Clone();
                    
                    // Map common currency codes to symbols
                    var currencySymbol = currencyCode.ToUpper() switch
                    {
                        "USD" => "$",
                        "EUR" => "€",
                        "GBP" => "£",
                        "KES" => "KSh", // Kenyan Shilling for Swahili
                        "XOF" => "CFA", // West African CFA franc for French-speaking Africa
                        _ => currencyCode
                    };
                    
                    numberFormat.CurrencySymbol = currencySymbol;
                    return amount.ToString("C", numberFormat);
                }

                return amount.ToString("C", _currentCulture);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error formatting currency: {Amount} with code: {CurrencyCode}", amount, currencyCode);
                return amount.ToString("C");
            }
        }

        public string FormatDate(DateTime date, string? format = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(format))
                {
                    return date.ToString(format, _currentCulture);
                }

                return date.ToString("d", _currentCulture); // Short date pattern
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error formatting date: {Date} with format: {Format}", date, format);
                return date.ToString("d");
            }
        }

        public string FormatDateTime(DateTime dateTime, string? format = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(format))
                {
                    return dateTime.ToString(format, _currentCulture);
                }

                return dateTime.ToString("g", _currentCulture); // General date/time pattern (short)
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error formatting datetime: {DateTime} with format: {Format}", dateTime, format);
                return dateTime.ToString("g");
            }
        }

        public string GetDecimalSeparator()
        {
            try
            {
                return _currentCulture.NumberFormat.NumberDecimalSeparator;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting decimal separator");
                return ".";
            }
        }

        public string GetThousandsSeparator()
        {
            try
            {
                return _currentCulture.NumberFormat.NumberGroupSeparator;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting thousands separator");
                return ",";
            }
        }
    }
}