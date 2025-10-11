namespace Pilgrims.PersonalFinances.Services.Interfaces;

/// <summary>
/// Service for managing automatic app locking functionality
/// </summary>
public interface IAutoLockService
{
    /// <summary>
    /// Event raised when the app should be locked
    /// </summary>
    event EventHandler? AppShouldLock;

    /// <summary>
    /// Get the current auto-lock timeout in minutes
    /// </summary>
    Task<int> GetTimeoutAsync();

    /// <summary>
    /// Set the auto-lock timeout in minutes (0 to disable)
    /// </summary>
    Task<bool> SetTimeoutAsync(int timeoutMinutes);

    /// <summary>
    /// Check if auto-lock is enabled
    /// </summary>
    Task<bool> IsEnabledAsync();

    /// <summary>
    /// Enable or disable auto-lock
    /// </summary>
    Task<bool> SetEnabledAsync(bool enabled);

    /// <summary>
    /// Reset the auto-lock timer (call when user interacts with the app)
    /// </summary>
    void ResetTimer();

    /// <summary>
    /// Start the auto-lock timer
    /// </summary>
    void StartTimer();

    /// <summary>
    /// Stop the auto-lock timer
    /// </summary>
    void StopTimer();

    /// <summary>
    /// Check if the app is currently locked
    /// </summary>
    bool IsLocked { get; }

    /// <summary>
    /// Lock the app immediately
    /// </summary>
    void LockApp();

    /// <summary>
    /// Unlock the app
    /// </summary>
    void UnlockApp();
}