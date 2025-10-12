namespace Pilgrims.PersonalFinances.Core.Services.Interfaces;

/// <summary>
/// Service for managing screenshot protection on sensitive screens
/// </summary>
public interface IScreenshotProtectionService
{
    /// <summary>
    /// Enable screenshot protection for the current screen
    /// </summary>
    Task<bool> EnableProtectionAsync();

    /// <summary>
    /// Disable screenshot protection for the current screen
    /// </summary>
    Task<bool> DisableProtectionAsync();

    /// <summary>
    /// Check if screenshot protection is currently enabled
    /// </summary>
    bool IsProtectionEnabled { get; }

    /// <summary>
    /// Check if screenshot protection is supported on the current platform
    /// </summary>
    bool IsSupported { get; }

    /// <summary>
    /// Enable protection for a specific page or component
    /// </summary>
    Task<bool> EnableProtectionForPageAsync(string pageName);

    /// <summary>
    /// Disable protection for a specific page or component
    /// </summary>
    Task<bool> DisableProtectionForPageAsync(string pageName);

    /// <summary>
    /// Get the list of pages that should have screenshot protection enabled
    /// </summary>
    Task<List<string>> GetProtectedPagesAsync();

    /// <summary>
    /// Set the list of pages that should have screenshot protection enabled
    /// </summary>
    Task<bool> SetProtectedPagesAsync(List<string> pageNames);

    /// <summary>
    /// Check if a specific page should have screenshot protection
    /// </summary>
    Task<bool> ShouldProtectPageAsync(string pageName);
}