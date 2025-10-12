using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Pilgrims.PersonalFinances.Core.Services
{
    public class ThemeService : IThemeService
    {
        private readonly IJSRuntime _jsRuntime;
        private string _currentTheme = "blue-light";

        public ThemeService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public string CurrentTheme => _currentTheme;

        public event Action<string>? ThemeChanged;

        public async Task SetThemeAsync(string theme)
        {
            if (string.IsNullOrEmpty(theme) || !IsValidTheme(theme))
                return;

            _currentTheme = theme;
            
            // Save to local storage
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "selectedTheme", theme);
            
            // Apply theme to document
            await _jsRuntime.InvokeVoidAsync("document.documentElement.setAttribute", "data-theme", theme);
            
            // Notify subscribers
            ThemeChanged?.Invoke(theme);
        }

        public async Task<string> GetSavedThemeAsync()
        {
            try
            {
                var savedTheme = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "selectedTheme");
                if (!string.IsNullOrEmpty(savedTheme) && IsValidTheme(savedTheme))
                {
                    _currentTheme = savedTheme;
                    return savedTheme;
                }
            }
            catch (Exception)
            {
                // Fallback to default theme if localStorage is not available
            }
            
            return "blue-light";
        }

        public async Task ToggleDarkModeAsync()
        {
            var parts = _currentTheme.Split('-');
            if (parts.Length != 2) return;

            var color = parts[0];
            var mode = parts[1] == "light" ? "dark" : "light";
            var newTheme = $"{color}-{mode}";

            await SetThemeAsync(newTheme);
        }

        public string[] GetAvailableThemes()
        {
            return new[]
            {
                "blue-light", "blue-dark",
                "green-light", "green-dark",
                "pink-light", "pink-dark",
                "white-light", "white-dark"
            };
        }

        public string[] GetAvailableColors()
        {
            return new[] { "blue", "green", "pink", "white" };
        }

        private bool IsValidTheme(string theme)
        {
            var validThemes = GetAvailableThemes();
            return Array.Exists(validThemes, t => t == theme);
        }

        public async Task InitializeAsync()
        {
            var savedTheme = await GetSavedThemeAsync();
            await SetThemeAsync(savedTheme);
        }
    }
}