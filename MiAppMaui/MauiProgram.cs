using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using MiAppMaui.ViewModels;
using MiAppMaui.Views;

namespace MiAppMaui;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		// Registrar ViewModels
		builder.Services.AddSingleton<MainViewModel>();
		
		// Registrar Views
		builder.Services.AddSingleton<MainPage>();

#if DEBUG
		builder.Logging.AddDebug();
		builder.Logging.SetMinimumLevel(LogLevel.Debug);
#endif

		return builder.Build();
	}
}
