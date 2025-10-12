namespace Pilgrims.PersonalFinances.Core.Services
{
    public interface IWindowService
    {
        Task CloseApplicationAsync();
        Task MinimizeWindowAsync();
        Task MaximizeWindowAsync();
        Task RestoreWindowAsync();
        Task<bool> IsMaximizedAsync();
    }
}