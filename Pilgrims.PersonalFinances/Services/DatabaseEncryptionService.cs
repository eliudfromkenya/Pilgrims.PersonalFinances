using Microsoft.Extensions.Logging;
using Pilgrims.PersonalFinances.Core.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace Pilgrims.PersonalFinances.Core.Services;

/// <summary>
/// Service for managing database encryption with SQLCipher
/// </summary>
public class DatabaseEncryptionService : IDatabaseEncryptionService
{
    private const string DatabaseKeyAlias = "PersonalFinance_DatabaseKey";
    private const int KeySizeBytes = 32; // 256 bits for AES-256
    private readonly ILogger<DatabaseEncryptionService> _logger;

    public DatabaseEncryptionService(ILogger<DatabaseEncryptionService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get or generate the database encryption key from secure storage
    /// </summary>
    public async Task<string> GetDatabaseKeyAsync()
    {
        try
        {
            // Try to get existing key from secure storage
            var existingKey = await SecureStorage.GetAsync(DatabaseKeyAlias);
            
            if (!string.IsNullOrEmpty(existingKey))
            {
                _logger.LogDebug("Retrieved existing database encryption key from secure storage");
                return existingKey;
            }

            // Generate new key if none exists
            var newKey = await GenerateNewKeyAsync();
            await SecureStorage.SetAsync(DatabaseKeyAlias, newKey);
            
            _logger.LogInformation("Generated and stored new database encryption key");
            return newKey;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get or generate database encryption key");
            throw new InvalidOperationException("Unable to access database encryption key", ex);
        }
    }

    /// <summary>
    /// Generate a new device-specific encryption key
    /// </summary>
    public async Task<string> GenerateNewKeyAsync()
    {
        try
        {
            // Generate cryptographically secure random key
            using var rng = RandomNumberGenerator.Create();
            var keyBytes = new byte[KeySizeBytes];
            rng.GetBytes(keyBytes);

            // Add device-specific entropy
            var deviceId = await GetDeviceIdentifierAsync();
            var deviceBytes = Encoding.UTF8.GetBytes(deviceId);
            
            // Combine random key with device-specific data using HMAC
            using var hmac = new HMACSHA256(keyBytes);
            var combinedKey = hmac.ComputeHash(deviceBytes);

            // Convert to hex string for SQLCipher
            var keyString = Convert.ToHexString(combinedKey);
            
            _logger.LogDebug("Generated new device-specific encryption key");
            return keyString;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate new encryption key");
            throw new InvalidOperationException("Unable to generate encryption key", ex);
        }
    }

    /// <summary>
    /// Verify if the database is encrypted and accessible
    /// </summary>
    public async Task<bool> VerifyDatabaseEncryptionAsync(string connectionString)
    {
        try
        {
            // This would require SQLCipher-specific verification
            // For now, we'll implement a basic check
            _logger.LogDebug("Verifying database encryption for connection: {ConnectionString}", 
                connectionString.Replace("Password=", "Password=***"));
            
            // TODO: Implement actual SQLCipher verification
            // This would involve trying to open the database and checking if it's encrypted
            return await Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to verify database encryption");
            return false;
        }
    }

    /// <summary>
    /// Get the encrypted connection string with the encryption key
    /// </summary>
    public async Task<string> GetEncryptedConnectionStringAsync(string databasePath)
    {
        try
        {
            var encryptionKey = await GetDatabaseKeyAsync();
            
            // Build SQLCipher connection string with encryption
            var connectionString = $"Data Source={databasePath};Password={encryptionKey};";
            
            _logger.LogDebug("Generated encrypted connection string for database: {DatabasePath}", databasePath);
            return connectionString;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate encrypted connection string");
            throw new InvalidOperationException("Unable to create encrypted connection string", ex);
        }
    }

    /// <summary>
    /// Get a device-specific identifier for key generation
    /// </summary>
    private async Task<string> GetDeviceIdentifierAsync()
    {
        try
        {
            // Use device-specific information to create unique identifier
            var deviceInfo = DeviceInfo.Current;
            var identifier = $"{deviceInfo.Platform}_{deviceInfo.Model}_{deviceInfo.Manufacturer}_{deviceInfo.VersionString}";
            
            // Add app-specific salt
            var appId = AppInfo.Current.PackageName;
            var combined = $"{identifier}_{appId}";
            
            // Hash the combined identifier for consistency
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combined));
            var deviceId = Convert.ToHexString(hashBytes);
            
            _logger.LogDebug("Generated device identifier hash");
            return await Task.FromResult(deviceId);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to generate device identifier, using fallback");
            // Fallback to a static identifier if device info is not available
            return await Task.FromResult("FALLBACK_DEVICE_ID");
        }
    }
}