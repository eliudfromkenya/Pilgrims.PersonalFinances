using Microsoft.JSInterop;

namespace Pilgrims.PersonalFinances.Core.Services
{
    public class WindowService : IWindowService
    {
        private readonly IJSRuntime _jsRuntime;

        public WindowService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task CloseApplicationAsync()
        {
            await _jsRuntime.InvokeVoidAsync("window.close");
        }

        public async Task MinimizeWindowAsync()
        {
            await _jsRuntime.InvokeVoidAsync("eval", "window.minimize && window.minimize()");
        }

        public async Task MaximizeWindowAsync()
        {
            await _jsRuntime.InvokeVoidAsync("eval", "window.maximize && window.maximize()");
        }

        public async Task RestoreWindowAsync()
        {
            await _jsRuntime.InvokeVoidAsync("eval", "window.restore && window.restore()");
        }

        public async Task<bool> IsMaximizedAsync()
        {
            return await _jsRuntime.InvokeAsync<bool>("eval", "window.outerWidth >= screen.availWidth && window.outerHeight >= screen.availHeight");
        }
    }
}