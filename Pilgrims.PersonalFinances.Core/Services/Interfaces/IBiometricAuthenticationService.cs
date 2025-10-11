using System.Threading.Tasks;

namespace Pilgrims.PersonalFinances.Core.Services.Interfaces;

/// <summary>
/// Service for managing biometric authentication (fingerprint, face ID, etc.)
/// </summary>
public interface IBiometricAuthenticationService
{
    /// <summary>
    /// Check if biometric authentication is available on the device
    /// </summary>
    /// <returns>True if biometric authentication is available</returns>
    Task<bool> IsAvailableAsync();

    /// <summary>
    /// Check if biometric authentication is enrolled (user has set up biometrics)
    /// </summary>
    /// <returns>True if biometric authentication is enrolled</returns>
    Task<bool> IsEnrolledAsync();

    /// <summary>
    /// Get the type of biometric authentication available
    /// </summary>
    /// <returns>The biometric authentication type</returns>
    Task<BiometricAuthenticationType> GetAuthenticationTypeAsync();

    /// <summary>
    /// Authenticate using biometric authentication
    /// </summary>
    /// <param name="reason">Reason for authentication request</param>
    /// <returns>Biometric authentication result</returns>
    Task<BiometricAuthenticationResult> AuthenticateAsync(string reason = "Please verify your identity");

    /// <summary>
    /// Check if biometric authentication is enabled for the app
    /// </summary>
    /// <returns>True if enabled</returns>
    Task<bool> IsEnabledAsync();

    /// <summary>
    /// Enable or disable biometric authentication for the app
    /// </summary>
    /// <param name="enabled">Whether to enable biometric authentication</param>
    /// <returns>True if successful</returns>
    Task<bool> SetEnabledAsync(bool enabled);
}

/// <summary>
/// Types of biometric authentication
/// </summary>
public enum BiometricAuthenticationType
{
    None,
    Fingerprint,
    Face,
    Voice,
    Iris,
    Multiple
}

/// <summary>
/// Result of biometric authentication attempt
/// </summary>
public class BiometricAuthenticationResult
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public BiometricAuthenticationStatus Status { get; set; }

    public static BiometricAuthenticationResult Success()
    {
        return new BiometricAuthenticationResult
        {
            IsSuccess = true,
            Status = BiometricAuthenticationStatus.Succeeded
        };
    }

    public static BiometricAuthenticationResult Failure(string errorMessage, BiometricAuthenticationStatus status)
    {
        return new BiometricAuthenticationResult
        {
            IsSuccess = false,
            ErrorMessage = errorMessage,
            Status = status
        };
    }
}

/// <summary>
/// Status of biometric authentication
/// </summary>
public enum BiometricAuthenticationStatus
{
    Unknown,
    Succeeded,
    Failed,
    Cancelled,
    FallbackRequested,
    NotAvailable,
    NotEnrolled,
    Denied,
    TooManyAttempts,
    Lockout
}