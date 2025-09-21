using System;
using System.Threading.Tasks;

namespace Pilgrims.PersonalFinances.Services
{
    public interface IThemeService
    {
        /// <summary>
        /// Gets the current theme
        /// </summary>
        string CurrentTheme { get; }

        /// <summary>
        /// Event triggered when theme changes
        /// </summary>
        event Action<string>? ThemeChanged;

        /// <summary>
        /// Sets the current theme
        /// </summary>
        /// <param name="theme">Theme name (e.g., "blue-light", "green-dark")</param>
        Task SetThemeAsync(string theme);

        /// <summary>
        /// Gets the saved theme from local storage
        /// </summary>
        Task<string> GetSavedThemeAsync();

        /// <summary>
        /// Toggles between light and dark mode for current theme
        /// </summary>
        Task ToggleDarkModeAsync();

        /// <summary>
        /// Gets available themes
        /// </summary>
        string[] GetAvailableThemes();

        /// <summary>
        /// Gets available theme colors
        /// </summary>
        string[] GetAvailableColors();
    }
}