using System.Threading;
using System.Threading.Tasks;

namespace Pilgrims.PersonalFinances.Core.Services.Interfaces
{
    /// <summary>
    /// Abstraction for storing and retrieving simple preference values.
    /// </summary>
    public interface IPreferencesService
    {
        /// <summary>
        /// Gets a string value from preferences for the specified key.
        /// Returns null when the key is not present.
        /// </summary>
        Task<string?> GetAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets a string value in preferences for the specified key.
        /// </summary>
        Task SetAsync(string key, string value, CancellationToken cancellationToken = default);
    }
}