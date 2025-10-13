using Pilgrims.PersonalFinances.Core.Services.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Pilgrims.PersonalFinances.Services
{
    /// <summary>
    /// Platform-specific implementation using MAUI Preferences API.
    /// </summary>
    public sealed class PreferencesService : IPreferencesService
    {
        public Task<string?> GetAsync(string key, CancellationToken cancellationToken = default)
        {
            var value = Microsoft.Maui.Storage.Preferences.Get(key, (string?)null);
            return Task.FromResult(value);
        }

        public Task SetAsync(string key, string value, CancellationToken cancellationToken = default)
        {
            Microsoft.Maui.Storage.Preferences.Set(key, value);
            return Task.CompletedTask;
        }
    }
}