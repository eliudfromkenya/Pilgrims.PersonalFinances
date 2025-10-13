using System.Threading.Tasks;

namespace Pilgrims.PersonalFinances.Core.Messaging.Interfaces
{
    /// <summary>
    /// Coordinates readiness of the Blazor WebView JavaScript runtime for interop calls.
    /// Prevents invoking JS outside of a valid WebView context.
    /// </summary>
    public interface IJsInteropInitializationService
    {
        /// <summary>
        /// Indicates whether the JS runtime has been marked ready for interop.
        /// </summary>
        bool IsReady { get; }

        /// <summary>
        /// Await until the JS runtime is ready for interop calls.
        /// </summary>
        Task WaitReadyAsync();

        /// <summary>
        /// Marks the JS runtime as ready. Should be called from a component after first render.
        /// </summary>
        void MarkReady();
    }
}