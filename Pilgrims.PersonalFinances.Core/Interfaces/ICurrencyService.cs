using System.Globalization;

namespace Pilgrims.PersonalFinances.Core.Interfaces;

/// <summary>
/// Service for centralized currency management and formatting
/// </summary>
public interface ICurrencyService
{
    /// <summary>
    /// Gets the current system-wide currency code (e.g., "USD", "EUR", "GBP")
    /// </summary>
    Task<string> GetCurrentCurrencyAsync();

    /// <summary>
    /// Sets the system-wide currency code
    /// </summary>
    /// <param name="currencyCode">ISO 4217 currency code</param>
    Task SetCurrentCurrencyAsync(string currencyCode);

    /// <summary>
    /// Gets the currency symbol for the current currency (e.g., "$", "€", "£")
    /// </summary>
    Task<string> GetCurrencySymbolAsync();

    /// <summary>
    /// Gets the currency symbol for a specific currency code
    /// </summary>
    /// <param name="currencyCode">ISO 4217 currency code</param>
    string GetCurrencySymbol(string currencyCode);

    /// <summary>
    /// Formats an amount using the current system currency
    /// </summary>
    /// <param name="amount">Amount to format</param>
    /// <param name="includeCurrencyCode">Whether to include currency code (e.g., "USD")</param>
    Task<string> FormatAmountAsync(decimal amount, bool includeCurrencyCode = false);

    /// <summary>
    /// Formats an amount using a specific currency
    /// </summary>
    /// <param name="amount">Amount to format</param>
    /// <param name="currencyCode">ISO 4217 currency code</param>
    /// <param name="includeCurrencyCode">Whether to include currency code</param>
    string FormatAmount(decimal amount, string currencyCode, bool includeCurrencyCode = false);

    /// <summary>
    /// Gets the culture info for the current currency
    /// </summary>
    Task<CultureInfo> GetCurrencyCultureAsync();

    /// <summary>
    /// Gets the culture info for a specific currency
    /// </summary>
    /// <param name="currencyCode">ISO 4217 currency code</param>
    CultureInfo GetCurrencyCulture(string currencyCode);

    /// <summary>
    /// Gets a list of supported currencies
    /// </summary>
    List<(string Code, string Name, string Symbol)> GetSupportedCurrencies();
}