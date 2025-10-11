using System;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Pilgrims.PersonalFinances.Data;
using Pilgrims.PersonalFinances.Core.Interfaces;
using Pilgrims.PersonalFinances.Core.Models;

namespace Pilgrims.PersonalFinances.Core.Services;

/// <summary>
/// Service for centralized currency management and formatting
/// </summary>
public class CurrencyService : ICurrencyService
{
    private readonly PersonalFinanceContext _context;
    private ApplicationSettings? _cachedSettings;
    private readonly Dictionary<string, (string Name, string Symbol, CultureInfo Culture)> _supportedCurrencies;

    public CurrencyService(PersonalFinanceContext context)
    {
        _context = context;
        _supportedCurrencies = InitializeSupportedCurrencies();
    }

    public async Task<string> GetCurrentCurrencyAsync()
    {
        var settings = await GetApplicationSettingsAsync();
        return settings.DefaultCurrency;
    }

    public async Task SetCurrentCurrencyAsync(string currencyCode)
    {
        if (string.IsNullOrWhiteSpace(currencyCode) || currencyCode.Length != 3)
            throw new ArgumentException("Currency code must be a valid 3-character ISO 4217 code", nameof(currencyCode));

        if (!_supportedCurrencies.ContainsKey(currencyCode.ToUpper()))
            throw new ArgumentException($"Currency code '{currencyCode}' is not supported", nameof(currencyCode));

        var settings = await GetApplicationSettingsAsync();
        settings.DefaultCurrency = currencyCode.ToUpper();
        settings.TouchUpdatedAt();

        await _context.SaveChangesAsync();
        _cachedSettings = null; // Clear cache
    }

    public async Task<string> GetCurrencySymbolAsync()
    {
        var currencyCode = await GetCurrentCurrencyAsync();
        return GetCurrencySymbol(currencyCode);
    }

    public string GetCurrencySymbol(string currencyCode)
    {
        if (_supportedCurrencies.TryGetValue(currencyCode.ToUpper(), out var currency))
        {
            return currency.Symbol;
        }

        // Fallback to culture-based symbol lookup
        try
        {
            var culture = GetCurrencyCulture(currencyCode);
            return culture.NumberFormat.CurrencySymbol;
        }
        catch
        {
            return "$"; // Default fallback
        }
    }

    public async Task<string> FormatAmountAsync(decimal amount, bool includeCurrencyCode = false)
    {
        var currencyCode = await GetCurrentCurrencyAsync();
        return FormatAmount(amount, currencyCode, includeCurrencyCode);
    }

    public string FormatAmount(decimal amount, string currencyCode, bool includeCurrencyCode = false)
    {
        var culture = GetCurrencyCulture(currencyCode);
        var formatted = amount.ToString("C", culture);

        if (includeCurrencyCode)
        {
            formatted += $" {currencyCode.ToUpper()}";
        }

        return formatted;
    }

    public async Task<CultureInfo> GetCurrencyCultureAsync()
    {
        var currencyCode = await GetCurrentCurrencyAsync();
        return GetCurrencyCulture(currencyCode);
    }

    public CultureInfo GetCurrencyCulture(string currencyCode)
    {
        if (_supportedCurrencies.TryGetValue(currencyCode.ToUpper(), out var currency))
        {
            return currency.Culture;
        }

        // Fallback mapping for common currencies
        return currencyCode.ToUpper() switch
        {
            "USD" => new CultureInfo("en-US"),
            "EUR" => new CultureInfo("de-DE"),
            "GBP" => new CultureInfo("en-GB"),
            "JPY" => new CultureInfo("ja-JP"),
            "CAD" => new CultureInfo("en-CA"),
            "AUD" => new CultureInfo("en-AU"),
            "CHF" => new CultureInfo("de-CH"),
            "CNY" => new CultureInfo("zh-CN"),
            "INR" => new CultureInfo("hi-IN"),
            _ => new CultureInfo("en-US") // Default to US culture
        };
    }

    public List<(string Code, string Name, string Symbol)> GetSupportedCurrencies()
    {
        // Load currencies from database if available, otherwise fall back to hardcoded list
        try
        {
            var dbCurrencies = _context.Currencies.ToList();
            if (dbCurrencies.Any())
            {
                return dbCurrencies.Select(c => (
                    Code: c.ISOCode,
                    Name: c.CurrencyName,
                    Symbol: c.SymbolOrSign
                )).OrderBy(c => c.Code).ToList();
            }
        }
        catch
        {
            // If database is not available or not seeded, fall back to hardcoded list
        }

        // Fallback to hardcoded currencies
        return _supportedCurrencies.Select(kvp => (
            Code: kvp.Key,
            Name: kvp.Value.Name,
            Symbol: kvp.Value.Symbol
        )).OrderBy(c => c.Code).ToList();
    }

    private async Task<ApplicationSettings> GetApplicationSettingsAsync()
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
                DefaultCurrency = "USD",
                IsActive = true
            };

            _context.ApplicationSettings.Add(_cachedSettings);
            await _context.SaveChangesAsync();
        }

        return _cachedSettings;
    }

    private static Dictionary<string, (string Name, string Symbol, CultureInfo Culture)> InitializeSupportedCurrencies()
    {
        return new Dictionary<string, (string Name, string Symbol, CultureInfo Culture)>
        {
            { "USD", ("US Dollar", "$", new CultureInfo("en-US")) },
            { "EUR", ("Euro", "€", new CultureInfo("de-DE")) },
            { "GBP", ("British Pound", "£", new CultureInfo("en-GB")) },
            { "JPY", ("Japanese Yen", "¥", new CultureInfo("ja-JP")) },
            { "CAD", ("Canadian Dollar", "C$", new CultureInfo("en-CA")) },
            { "AUD", ("Australian Dollar", "A$", new CultureInfo("en-AU")) },
            { "CHF", ("Swiss Franc", "CHF", new CultureInfo("de-CH")) },
            { "CNY", ("Chinese Yuan", "¥", new CultureInfo("zh-CN")) },
            { "INR", ("Indian Rupee", "₹", new CultureInfo("hi-IN")) },
            { "KRW", ("South Korean Won", "₩", new CultureInfo("ko-KR")) },
            { "SGD", ("Singapore Dollar", "S$", new CultureInfo("en-SG")) },
            { "HKD", ("Hong Kong Dollar", "HK$", new CultureInfo("zh-HK")) },
            { "NOK", ("Norwegian Krone", "kr", new CultureInfo("nb-NO")) },
            { "SEK", ("Swedish Krona", "kr", new CultureInfo("sv-SE")) },
            { "DKK", ("Danish Krone", "kr", new CultureInfo("da-DK")) },
            { "PLN", ("Polish Zloty", "zł", new CultureInfo("pl-PL")) },
            { "CZK", ("Czech Koruna", "Kč", new CultureInfo("cs-CZ")) },
            { "HUF", ("Hungarian Forint", "Ft", new CultureInfo("hu-HU")) },
            { "RUB", ("Russian Ruble", "₽", new CultureInfo("ru-RU")) },
            { "BRL", ("Brazilian Real", "R$", new CultureInfo("pt-BR")) },
            { "MXN", ("Mexican Peso", "$", new CultureInfo("es-MX")) },
            { "ZAR", ("South African Rand", "R", new CultureInfo("en-ZA")) },
            { "TRY", ("Turkish Lira", "₺", new CultureInfo("tr-TR")) },
            { "NZD", ("New Zealand Dollar", "NZ$", new CultureInfo("en-NZ")) },
            { "THB", ("Thai Baht", "฿", new CultureInfo("th-TH")) }
        };
    }
}