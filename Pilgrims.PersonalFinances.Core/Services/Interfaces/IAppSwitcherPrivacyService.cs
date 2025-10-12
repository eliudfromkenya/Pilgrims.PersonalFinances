namespace Pilgrims.PersonalFinances.Core.Services.Interfaces;

/// <summary>
/// Service for managing app switcher privacy to hide sensitive data when app is in background
/// </summary>
public interface IAppSwitcherPrivacyService
{
    /// <summary>
    /// Event raised when the app is about to enter background (should hide sensitive data)
    /// </summary>
    event EventHandler? AppEnteringBackground;

    /// <summary>
    /// Event raised when the app is returning to foreground (can show sensitive data)
    /// </summary>
    event EventHandler? AppReturningToForeground;

    /// <summary>
    /// Enable app switcher privacy protection
    /// </summary>
    Task<bool> EnablePrivacyProtectionAsync();

    /// <summary>
    /// Disable app switcher privacy protection
    /// </summary>
    Task<bool> DisablePrivacyProtectionAsync();

    /// <summary>
    /// Check if privacy protection is currently enabled
    /// </summary>
    Task<bool> IsPrivacyProtectionEnabledAsync();

    /// <summary>
    /// Check if privacy protection is supported on the current platform
    /// </summary>
    bool IsSupported { get; }

    /// <summary>
    /// Show privacy overlay (blur/hide sensitive content)
    /// </summary>
    Task ShowPrivacyOverlayAsync();

    /// <summary>
    /// Hide privacy overlay (show normal content)
    /// </summary>
    Task HidePrivacyOverlayAsync();

    /// <summary>
    /// Check if privacy overlay is currently shown
    /// </summary>
    bool IsPrivacyOverlayVisible { get; }

    /// <summary>
    /// Set custom privacy overlay content or message
    /// </summary>
    Task<bool> SetPrivacyOverlayContentAsync(string content);

    /// <summary>
    /// Get the current privacy overlay content
    /// </summary>
    Task<string> GetPrivacyOverlayContentAsync();
}