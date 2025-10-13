using System.Threading.Tasks;
using Pilgrims.PersonalFinances.Core.Messaging.Interfaces;

namespace Pilgrims.PersonalFinances.Core.Messaging.Services
{
    /// <summary>
    /// Simple readiness gate for JS interop to avoid invoking outside WebView context.
    /// </summary>
    public class JsInteropInitializationService : IJsInteropInitializationService
    {
        private readonly TaskCompletionSource _readyTcs = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public bool IsReady => _readyTcs.Task.IsCompleted;

        public Task WaitReadyAsync() => _readyTcs.Task;

        public void MarkReady()
        {
            if (!IsReady)
            {
                _readyTcs.TrySetResult();
            }
        }
    }
}