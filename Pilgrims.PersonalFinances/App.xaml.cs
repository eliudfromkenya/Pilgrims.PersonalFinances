namespace Pilgrims.PersonalFinances
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(new MainPage()) { Title = "Pilgrims.PersonalFinances" };
            
            // Set window to start maximized (only supported on Windows)
#if WINDOWS
            window.MaximumHeight = double.PositiveInfinity;
            window.MaximumWidth = double.PositiveInfinity;
            
            // Configure window to start maximized
            window.Created += (s, e) =>
            {
                var platformWindow = window.Handler?.PlatformView as Microsoft.UI.Xaml.Window;
                if (platformWindow != null)
                {
                    var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(
                        Microsoft.UI.Win32Interop.GetWindowIdFromWindow(
                            WinRT.Interop.WindowNative.GetWindowHandle(platformWindow)));
                    
                    if (appWindow != null)
                    {
                        appWindow.Resize(new Windows.Graphics.SizeInt32(1920, 1080));
                        var presenter = appWindow.Presenter as Microsoft.UI.Windowing.OverlappedPresenter;
                        presenter?.Maximize();
                    }
                }
            };
#endif
            
            return window;
        }
    }
}
