using Microsoft.Extensions.Logging;
using Pilgrims.PersonalFinances.Services.Interfaces;
using System.Timers;

namespace Pilgrims.PersonalFinances.Services;

/// <summary>
/// Service for managing automatic app locking functionality
/// </summary>
public class AutoLockService : IAutoLockService, IDisposable
{
    private const string AutoLockEnabledKey = "AutoLockEnabled";
    private const string AutoLockTimeoutKey = "AutoLockTimeout";
    private const int DefaultTimeoutMinutes = 5;

    private readonly ILogger<AutoLockService> _logger;
    private readonly System.Timers.Timer _timer;
    private bool _isLocked = false;
    private bool _disposed = false;

    public AutoLockService(ILogger<AutoLockService> logger)
    {
        _logger = logger;
        _timer = new System.Timers.Timer();
        _timer.Elapsed += OnTimerElapsed;
        _timer.AutoReset = false; // Only fire once
    }

    /// <summary>
    /// Event raised when the app should be locked
    /// </summary>
    public event EventHandler? AppShouldLock;

    /// <summary>
    /// Check if the app is currently locked
    /// </summary>
    public bool IsLocked => _isLocked;

    /// <summary>
    /// Get the current auto-lock timeout in minutes
    /// </summary>
    public async Task<int> GetTimeoutAsync()
    {
        try
        {
            var timeout = await SecureStorage.GetAsync(AutoLockTimeoutKey);
            if (int.TryParse(timeout, out var result) && result >= 0)
            {
                return result;
            }
            return DefaultTimeoutMinutes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting auto-lock timeout");
            return DefaultTimeoutMinutes;
        }
    }

    /// <summary>
    /// Set the auto-lock timeout in minutes (0 to disable)
    /// </summary>
    public async Task<bool> SetTimeoutAsync(int timeoutMinutes)
    {
        try
        {
            if (timeoutMinutes < 0)
            {
                _logger.LogWarning("Invalid timeout value: {Timeout}. Must be >= 0", timeoutMinutes);
                return false;
            }

            await SecureStorage.SetAsync(AutoLockTimeoutKey, timeoutMinutes.ToString());
            
            // Restart timer with new timeout if enabled
            if (await IsEnabledAsync() && timeoutMinutes > 0)
            {
                StartTimer();
            }
            else if (timeoutMinutes == 0)
            {
                StopTimer();
            }

            _logger.LogInformation("Auto-lock timeout set to {Timeout} minutes", timeoutMinutes);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting auto-lock timeout");
            return false;
        }
    }

    /// <summary>
    /// Check if auto-lock is enabled
    /// </summary>
    public async Task<bool> IsEnabledAsync()
    {
        try
        {
            var enabled = await SecureStorage.GetAsync(AutoLockEnabledKey);
            return bool.TryParse(enabled, out var result) && result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if auto-lock is enabled");
            return false;
        }
    }

    /// <summary>
    /// Enable or disable auto-lock
    /// </summary>
    public async Task<bool> SetEnabledAsync(bool enabled)
    {
        try
        {
            await SecureStorage.SetAsync(AutoLockEnabledKey, enabled.ToString());
            
            if (enabled)
            {
                var timeout = await GetTimeoutAsync();
                if (timeout > 0)
                {
                    StartTimer();
                }
            }
            else
            {
                StopTimer();
            }

            _logger.LogInformation("Auto-lock {Status}", enabled ? "enabled" : "disabled");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting auto-lock enabled state");
            return false;
        }
    }

    /// <summary>
    /// Reset the auto-lock timer (call when user interacts with the app)
    /// </summary>
    public void ResetTimer()
    {
        if (_disposed || _isLocked) return;

        try
        {
            Task.Run(async () =>
            {
                if (await IsEnabledAsync())
                {
                    var timeout = await GetTimeoutAsync();
                    if (timeout > 0)
                    {
                        _timer.Stop();
                        _timer.Interval = TimeSpan.FromMinutes(timeout).TotalMilliseconds;
                        _timer.Start();
                        _logger.LogDebug("Auto-lock timer reset for {Timeout} minutes", timeout);
                    }
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting auto-lock timer");
        }
    }

    /// <summary>
    /// Start the auto-lock timer
    /// </summary>
    public void StartTimer()
    {
        if (_disposed || _isLocked) return;

        try
        {
            Task.Run(async () =>
            {
                if (await IsEnabledAsync())
                {
                    var timeout = await GetTimeoutAsync();
                    if (timeout > 0)
                    {
                        _timer.Stop();
                        _timer.Interval = TimeSpan.FromMinutes(timeout).TotalMilliseconds;
                        _timer.Start();
                        _logger.LogDebug("Auto-lock timer started for {Timeout} minutes", timeout);
                    }
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting auto-lock timer");
        }
    }

    /// <summary>
    /// Stop the auto-lock timer
    /// </summary>
    public void StopTimer()
    {
        if (_disposed) return;

        try
        {
            _timer.Stop();
            _logger.LogDebug("Auto-lock timer stopped");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error stopping auto-lock timer");
        }
    }

    /// <summary>
    /// Lock the app immediately
    /// </summary>
    public void LockApp()
    {
        if (_disposed || _isLocked) return;

        try
        {
            _isLocked = true;
            StopTimer();
            AppShouldLock?.Invoke(this, EventArgs.Empty);
            _logger.LogInformation("App locked");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error locking app");
        }
    }

    /// <summary>
    /// Unlock the app
    /// </summary>
    public void UnlockApp()
    {
        if (_disposed) return;

        try
        {
            _isLocked = false;
            StartTimer(); // Restart timer after unlock
            _logger.LogInformation("App unlocked");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unlocking app");
        }
    }

    private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        _logger.LogDebug("Auto-lock timer elapsed, locking app");
        LockApp();
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _timer?.Stop();
            _timer?.Dispose();
            _disposed = true;
        }
    }
}