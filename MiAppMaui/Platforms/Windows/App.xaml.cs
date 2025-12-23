using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Windows.Graphics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MiAppMaui.WinUI;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : MauiWinUIApplication
{
	/// <summary>
	/// Initializes the singleton application object.  This is the first line of authored code
	/// executed, and as such is the logical equivalent of main() or WinMain().
	/// </summary>
	public App()
	{
		this.InitializeComponent();
		
		// Configurar tamaño de ventana estilo móvil
		Microsoft.Maui.Handlers.WindowHandler.Mapper.AppendToMapping(nameof(IWindow), (handler, view) =>
		{
			var nativeWindow = handler.PlatformView;
			nativeWindow.Activate();
			
			var windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);
			var windowId = Win32Interop.GetWindowIdFromWindow(windowHandle);
			var appWindow = AppWindow.GetFromWindowId(windowId);
			
			// Tamaño estilo teléfono móvil (ancho: 420px, alto: 900px)
			appWindow.Resize(new SizeInt32(420, 900));
			
			// Centrar la ventana
			if (appWindow.Presenter is OverlappedPresenter presenter)
			{
				// Opcional: hacer que la ventana no sea redimensionable
				// presenter.IsResizable = false;
			}
		});
	}

	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}

