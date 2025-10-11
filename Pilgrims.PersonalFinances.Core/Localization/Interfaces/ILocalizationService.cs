using System.Globalization;

namespace Pilgrims.PersonalFinances.Core.Localization.Interfaces
{
    /// <summary>
    /// Interface for localization services providing string localization and culture management
    /// </summary>
    public interface ILocalizationService
    {
        /// <summary>
        /// Gets the localized string for the specified key
        /// </summary>
        /// <param name="key">The resource key</param>
        /// <returns>The localized string</returns>
        string GetString(string key);

        /// <summary>
        /// Gets the localized string for the specified key with parameters
        /// </summary>
        /// <param name="key">The resource key</param>
        /// <param name="parameters">Parameters to format into the string</param>
        /// <returns>The formatted localized string</returns>
        string GetString(string key, params object[] parameters);

        /// <summary>
        /// Gets all available cultures/languages
        /// </summary>
        /// <returns>List of supported cultures</returns>
        IEnumerable<CultureInfo> GetAvailableCultures();

        /// <summary>
        /// Gets the current culture
        /// </summary>
        /// <returns>The current culture</returns>
        CultureInfo GetCurrentCulture();

        /// <summary>
        /// Sets the current culture
        /// </summary>
        /// <param name="culture">The culture to set</param>
        Task SetCultureAsync(string culture);

        /// <summary>
        /// Sets the current culture
        /// </summary>
        /// <param name="cultureInfo">The culture info to set</param>
        Task SetCultureAsync(CultureInfo cultureInfo);

        /// <summary>
        /// Formats a number according to the current culture
        /// </summary>
        /// <param name="number">The number to format</param>
        /// <returns>The formatted number string</returns>
        string FormatNumber(decimal number);

        /// <summary>
        /// Formats a currency amount according to the current culture
        /// </summary>
        /// <param name="amount">The amount to format</param>
        /// <param name="currencyCode">Optional currency code (defaults to current culture's currency)</param>
        /// <returns>The formatted currency string</returns>
        string FormatCurrency(decimal amount, string? currencyCode = null);

        /// <summary>
        /// Formats a date according to the current culture
        /// </summary>
        /// <param name="date">The date to format</param>
        /// <param name="format">Optional format string</param>
        /// <returns>The formatted date string</returns>
        string FormatDate(DateTime date, string? format = null);

        /// <summary>
        /// Formats a date and time according to the current culture
        /// </summary>
        /// <param name="dateTime">The date and time to format</param>
        /// <param name="format">Optional format string</param>
        /// <returns>The formatted date and time string</returns>
        string FormatDateTime(DateTime dateTime, string? format = null);

        /// <summary>
        /// Gets the culture-specific decimal separator
        /// </summary>
        /// <returns>The decimal separator for the current culture</returns>
        string GetDecimalSeparator();

        /// <summary>
        /// Gets the culture-specific thousands separator
        /// </summary>
        /// <returns>The thousands separator for the current culture</returns>
        string GetThousandsSeparator();

        /// <summary>
        /// Synchronizes the localization service with the current culture
        /// This should be called after CultureManager.InitializeAsync() to ensure consistency
        /// </summary>
        void SyncWithCurrentCulture();

        /// <summary>
        /// Event raised when the culture changes
        /// </summary>
        event EventHandler<CultureInfo>? CultureChanged;
    }
}