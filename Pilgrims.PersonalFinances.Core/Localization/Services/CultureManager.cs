using Microsoft.Extensions.Logging;
using Pilgrims.PersonalFinances.Core.Localization.Interfaces;
using Pilgrims.PersonalFinances.Core.Services.Interfaces;
using System.Globalization;

namespace Pilgrims.PersonalFinances.Core.Localization.Services
{
    /// <summary>
    /// Implementation of culture manager for handling culture settings and persistence
    /// </summary>
    public class CultureManager : ICultureManager
    {
        private readonly ILogger<CultureManager> _logger;
        private readonly IPreferencesService _preferences;
        private const string CulturePreferenceKey = "SelectedCulture";

        /// <summary>
        /// Supported cultures for the application
        /// </summary>
        private static readonly CultureInfo[] SupportedCultures = new[]
        {
            new CultureInfo("en"),
            new CultureInfo("sw"),
            new CultureInfo("fr"),
            new CultureInfo("de"),
            new CultureInfo("es")
        };

        /// <summary>
        /// Culture display names mapping
        /// </summary>
        private static readonly Dictionary<string, string> CultureDisplayNames = new()
        {
            { "en", "English" },
            { "sw", "Kiswahili" },
            { "fr", "Français" },
            { "de", "Deutsch" },
            { "es", "Español" }
        };

        public CultureManager(ILogger<CultureManager> logger, IPreferencesService preferences)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _preferences = preferences ?? throw new ArgumentNullException(nameof(preferences));
        }

        public async Task<CultureInfo> GetSavedCultureAsync()
        {
            try
            {
                var savedCultureCode = await GetStoredCultureCodeAsync().ConfigureAwait(false);
                
                if (!string.IsNullOrEmpty(savedCultureCode) && IsCultureSupported(savedCultureCode))
                {
                    return new CultureInfo(savedCultureCode);
                }

                var systemCulture = CultureInfo.CurrentUICulture;
                if (IsCultureSupported(systemCulture))
                {
                    return systemCulture;
                }

                return GetDefaultCulture();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting saved culture, falling back to default");
                return GetDefaultCulture();
            }
        }

        public async Task SaveCultureAsync(CultureInfo culture)
        {
            try
            {
                if (!IsCultureSupported(culture))
                {
                    _logger.LogWarning("Attempted to save unsupported culture: {Culture}", culture.Name);
                    return;
                }

                await StoreSelectedCultureAsync(culture.Name).ConfigureAwait(false);
                _logger.LogInformation("Culture preference saved: {Culture}", culture.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving culture preference: {Culture}", culture.Name);
                throw;
            }
        }

        public CultureInfo GetDefaultCulture()
        {
            return new CultureInfo("en"); // English as default
        }

        public bool IsCultureSupported(CultureInfo culture)
        {
            return SupportedCultures.Any(c => c.Name.Equals(culture.Name, StringComparison.OrdinalIgnoreCase));
        }

        public bool IsCultureSupported(string cultureCode)
        {
            return SupportedCultures.Any(c => c.Name.Equals(cultureCode, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<CultureInfo> GetSupportedCultures()
        {
            return SupportedCultures;
        }

        public string GetCultureDisplayName(CultureInfo culture)
        {
            try
            {
                if (CultureDisplayNames.TryGetValue(culture.Name, out var displayName))
                {
                    return displayName;
                }

                return culture.DisplayName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting culture display name for: {Culture}", culture.Name);
                return culture.Name;
            }
        }

        public string GetCultureNativeName(CultureInfo culture)
        {
            try
            {
                // Return the native name of the culture
                return culture.NativeName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting culture native name for: {Culture}", culture.Name);
                return culture.Name;
            }
        }

        public async Task InitializeAsync()
        {
            try
            {
                var savedCulture = await GetSavedCultureAsync().ConfigureAwait(false);
                
                // Set the current culture
                CultureInfo.CurrentCulture = savedCulture;
                CultureInfo.CurrentUICulture = savedCulture;

                _logger.LogInformation("Culture manager initialized with culture: {Culture}", savedCulture.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing culture manager");
                
                // Fall back to default culture
                var defaultCulture = GetDefaultCulture();
                CultureInfo.CurrentCulture = defaultCulture;
                CultureInfo.CurrentUICulture = defaultCulture;
            }
        }

        /// <summary>
        /// Gets the stored culture code from preferences
        /// </summary>
        private async Task<string?> GetStoredCultureCodeAsync()
        {
            try
            {
                var storedValue = await _preferences.GetAsync(CulturePreferenceKey).ConfigureAwait(false);
                return storedValue;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving stored culture code");
                return null;
            }
        }

        /// <summary>
        /// Stores the selected culture code in preferences
        /// </summary>
        private async Task StoreSelectedCultureAsync(string cultureCode)
        {
            try
            {
                await _preferences.SetAsync(CulturePreferenceKey, cultureCode).ConfigureAwait(false);
                _logger.LogInformation("Culture code stored: {CultureCode}", cultureCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error storing culture code: {CultureCode}", cultureCode);
                throw;
            }
        }
    }
}