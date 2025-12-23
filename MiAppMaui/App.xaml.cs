using MiAppMaui.Services;

namespace MiAppMaui;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		// Obtener el AppShell del contenedor de servicios
		var appShell = Handler?.MauiContext?.Services.GetService<AppShell>();
		
		if (appShell == null)
		{
			throw new InvalidOperationException("AppShell no está registrado en DI");
		}
		
		return new Window(appShell);
	}
}