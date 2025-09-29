using Microsoft.Extensions.Logging;
using Pilgrims.PersonalFinances.Services.Interfaces;

namespace Pilgrims.PersonalFinances.Services;

/// <summary>
/// Service for managing screenshot protection on sensitive screens
/// </summary>
public class ScreenshotProtectionService : IScreenshotProtectionService
{
    private const string ProtectedPagesKey = "ScreenshotProtectedPages";
    private readonly ILogger<ScreenshotProtectionService> _logger;
    private bool _isProtectionEnabled = false;

    // Default pages that should be protected
    private readonly List<string> _defaultProtectedPages = new()
    {
        "Accounts",
        "Transactions", 
        "Budget",
        "Debts",
        "Assets",
        "Income",
        "Reports",
        "Settings",
        "Goals",
        "Insurance"
    };

    public ScreenshotProtectionService(ILogger<ScreenshotProtectionService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Check if screenshot protection is currently enabled
    /// </summary>
    public bool IsProtectionEnabled => _isProtectionEnabled;

    /// <summary>
    /// Check if screenshot protection is supported on the current platform
    /// </summary>
    public bool IsSupported
    {
        get
        {
#if ANDROID || IOS
            return true;
#elif WINDOWS
            return true; // Windows supports window flags for screenshot protection
#elif MACCATALYST
            return true; // macOS supports screenshot protection
#else
            return false;
#endif
        }
    }

    /// <summary>
    /// Enable screenshot protection for the current screen
    /// </summary>
    public async Task<bool> EnableProtectionAsync()
    {
        try
        {
            if (!IsSupported)
            {
                _logger.LogWarning("Screenshot protection is not supported on this platform");
                return false;
            }

#if ANDROID
            return await EnableProtectionOnAndroidAsync();
#elif IOS
            return await EnableProtectionOnIOSAsync();
#elif WINDOWS
            return await EnableProtectionOnWindowsAsync();
#elif MACCATALYST
            return await EnableProtectionOnMacCatalystAsync();
#else
            _logger.LogWarning("Screenshot protection not implemented for this platform");
            return false;
#endif
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enabling screenshot protection");
            return false;
        }
    }

    /// <summary>
    /// Disable screenshot protection for the current screen
    /// </summary>
    public async Task<bool> DisableProtectionAsync()
    {
        try
        {
            if (!IsSupported)
            {
                return true; // Nothing to disable
            }

#if ANDROID
            return await DisableProtectionOnAndroidAsync();
#elif IOS
            return await DisableProtectionOnIOSAsync();
#elif WINDOWS
            return await DisableProtectionOnWindowsAsync();
#elif MACCATALYST
            return await DisableProtectionOnMacCatalystAsync();
#else
            return true;
#endif
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disabling screenshot protection");
            return false;
        }
    }

    /// <summary>
    /// Enable protection for a specific page or component
    /// </summary>
    public async Task<bool> EnableProtectionForPageAsync(string pageName)
    {
        try
        {
            var protectedPages = await GetProtectedPagesAsync();
            if (!protectedPages.Contains(pageName))
            {
                protectedPages.Add(pageName);
                return await SetProtectedPagesAsync(protectedPages);
            }
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enabling protection for page {PageName}", pageName);
            return false;
        }
    }

    /// <summary>
    /// Disable protection for a specific page or component
    /// </summary>
    public async Task<bool> DisableProtectionForPageAsync(string pageName)
    {
        try
        {
            var protectedPages = await GetProtectedPagesAsync();
            if (protectedPages.Contains(pageName))
            {
                protectedPages.Remove(pageName);
                return await SetProtectedPagesAsync(protectedPages);
            }
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disabling protection for page {PageName}", pageName);
            return false;
        }
    }

    /// <summary>
    /// Get the list of pages that should have screenshot protection enabled
    /// </summary>
    public async Task<List<string>> GetProtectedPagesAsync()
    {
        try
        {
            var pagesJson = await SecureStorage.GetAsync(ProtectedPagesKey);
            if (!string.IsNullOrEmpty(pagesJson))
            {
                var pages = System.Text.Json.JsonSerializer.Deserialize<List<string>>(pagesJson);
                return pages ?? _defaultProtectedPages;
            }
            return new List<string>(_defaultProtectedPages);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting protected pages list");
            return new List<string>(_defaultProtectedPages);
        }
    }

    /// <summary>
    /// Set the list of pages that should have screenshot protection enabled
    /// </summary>
    public async Task<bool> SetProtectedPagesAsync(List<string> pageNames)
    {
        try
        {
            var pagesJson = System.Text.Json.JsonSerializer.Serialize(pageNames);
            await SecureStorage.SetAsync(ProtectedPagesKey, pagesJson);
            _logger.LogInformation("Updated protected pages list with {Count} pages", pageNames.Count);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting protected pages list");
            return false;
        }
    }

    /// <summary>
    /// Check if a specific page should have screenshot protection
    /// </summary>
    public async Task<bool> ShouldProtectPageAsync(string pageName)
    {
        try
        {
            var protectedPages = await GetProtectedPagesAsync();
            return protectedPages.Contains(pageName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if page {PageName} should be protected", pageName);
            return true; // Default to protected for security
        }
    }

#if ANDROID
    private async Task<bool> EnableProtectionOnAndroidAsync()
    {
        // TODO: Implement Android-specific screenshot protection
        // This would use WindowManager.LayoutParams.FLAG_SECURE
        _logger.LogDebug("Enabling screenshot protection on Android");
        _isProtectionEnabled = true;
        return await Task.FromResult(true); // Placeholder
    }

    private async Task<bool> DisableProtectionOnAndroidAsync()
    {
        // TODO: Implement Android-specific screenshot protection removal
        _logger.LogDebug("Disabling screenshot protection on Android");
        _isProtectionEnabled = false;
        return await Task.FromResult(true); // Placeholder
    }
#endif

#if IOS
    private async Task<bool> EnableProtectionOnIOSAsync()
    {
        // TODO: Implement iOS-specific screenshot protection
        // This would use UIApplication.shared.ignoreSnapshotOnNextApplicationLaunch
        _logger.LogDebug("Enabling screenshot protection on iOS");
        _isProtectionEnabled = true;
        return await Task.FromResult(true); // Placeholder
    }

    private async Task<bool> DisableProtectionOnIOSAsync()
    {
        // TODO: Implement iOS-specific screenshot protection removal
        _logger.LogDebug("Disabling screenshot protection on iOS");
        _isProtectionEnabled = false;
        return await Task.FromResult(true); // Placeholder
    }
#endif

#if WINDOWS
    private async Task<bool> EnableProtectionOnWindowsAsync()
    {
        // TODO: Implement Windows-specific screenshot protection
        // This would use SetWindowDisplayAffinity or similar APIs
        _logger.LogDebug("Enabling screenshot protection on Windows");
        _isProtectionEnabled = true;
        return await Task.FromResult(true); // Placeholder
    }

    private async Task<bool> DisableProtectionOnWindowsAsync()
    {
        // TODO: Implement Windows-specific screenshot protection removal
        _logger.LogDebug("Disabling screenshot protection on Windows");
        _isProtectionEnabled = false;
        return await Task.FromResult(true); // Placeholder
    }
#endif

#if MACCATALYST
    private async Task<bool> EnableProtectionOnMacCatalystAsync()
    {
        // TODO: Implement macOS-specific screenshot protection
        _logger.LogDebug("Enabling screenshot protection on macOS");
        _isProtectionEnabled = true;
        return await Task.FromResult(true); // Placeholder
    }

    private async Task<bool> DisableProtectionOnMacCatalystAsync()
    {
        // TODO: Implement macOS-specific screenshot protection removal
        _logger.LogDebug("Disabling screenshot protection on macOS");
        _isProtectionEnabled = false;
        return await Task.FromResult(true); // Placeholder
    }
#endif
}