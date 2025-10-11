using System.Globalization;

namespace Pilgrims.PersonalFinances.Core.Localization.Interfaces
{
    /// <summary>
    /// Interface for managing culture settings and persistence
    /// </summary>
    public interface ICultureManager
    {
        /// <summary>
        /// Gets the saved culture preference
        /// </summary>
        /// <returns>The saved culture or default culture if none saved</returns>
        Task<CultureInfo> GetSavedCultureAsync();

        /// <summary>
        /// Saves the culture preference
        /// </summary>
        /// <param name="culture">The culture to save</param>
        Task SaveCultureAsync(CultureInfo culture);

        /// <summary>
        /// Gets the default culture for the application
        /// </summary>
        /// <returns>The default culture (English)</returns>
        CultureInfo GetDefaultCulture();

        /// <summary>
        /// Determines if the specified culture is supported
        /// </summary>
        /// <param name="culture">The culture to check</param>
        /// <returns>True if supported, false otherwise</returns>
        bool IsCultureSupported(CultureInfo culture);

        /// <summary>
        /// Determines if the specified culture code is supported
        /// </summary>
        /// <param name="cultureCode">The culture code to check</param>
        /// <returns>True if supported, false otherwise</returns>
        bool IsCultureSupported(string cultureCode);

        /// <summary>
        /// Gets all supported cultures
        /// </summary>
        /// <returns>List of supported cultures</returns>
        IEnumerable<CultureInfo> GetSupportedCultures();

        /// <summary>
        /// Gets the display name for a culture in the current UI language
        /// </summary>
        /// <param name="culture">The culture to get display name for</param>
        /// <returns>The localized display name</returns>
        string GetCultureDisplayName(CultureInfo culture);

        /// <summary>
        /// Gets the native display name for a culture
        /// </summary>
        /// <param name="culture">The culture to get native display name for</param>
        /// <returns>The native display name</returns>
        string GetCultureNativeName(CultureInfo culture);

        /// <summary>
        /// Initializes the culture manager and sets up the initial culture
        /// </summary>
        Task InitializeAsync();
    }
}