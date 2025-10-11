using System.Threading.Tasks;

namespace Pilgrims.PersonalFinances.Core.Services.Interfaces;

/// <summary>
/// Service for managing database encryption with SQLCipher
/// </summary>
public interface IDatabaseEncryptionService
{
    /// <summary>
    /// Get or generate the database encryption key
    /// </summary>
    /// <returns>The encryption key for the database</returns>
    Task<string> GetDatabaseKeyAsync();

    /// <summary>
    /// Generate a new device-specific encryption key
    /// </summary>
    /// <returns>A new encryption key</returns>
    Task<string> GenerateNewKeyAsync();

    /// <summary>
    /// Verify if the database is encrypted and accessible
    /// </summary>
    /// <param name="connectionString">The database connection string</param>
    /// <returns>True if the database is encrypted and accessible</returns>
    Task<bool> VerifyDatabaseEncryptionAsync(string connectionString);

    /// <summary>
    /// Get the encrypted connection string with the encryption key
    /// </summary>
    /// <param name="databasePath">Path to the database file</param>
    /// <returns>Connection string with encryption parameters</returns>
    Task<string> GetEncryptedConnectionStringAsync(string databasePath);
}