using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using MiAppMaui.ViewModels;
using MiAppMaui.Views;
using MiAppMaui.Services;

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

		// Registrar Services
		builder.Services.AddSingleton<DatabaseService>();
		builder.Services.AddSingleton<GoogleAuthService>();
		builder.Services.AddSingleton<UserSessionService>();
		
		// Registrar ViewModels
		builder.Services.AddSingleton<MainViewModel>();
		builder.Services.AddSingleton<AppShellViewModel>();
		builder.Services.AddTransient<LoginViewModel>();
		builder.Services.AddTransient<AnimationsViewModel>();
		builder.Services.AddTransient<UIControlsViewModel>();
		builder.Services.AddTransient<NotesViewModel>();
		
		// Registrar Views
		builder.Services.AddSingleton<MainPage>();
		builder.Services.AddSingleton<AppShell>();
		builder.Services.AddTransient<LoginPage>();
		builder.Services.AddTransient<AnimationsPage>();
		builder.Services.AddTransient<UIControlsPage>();
		builder.Services.AddTransient<NotesPage>();

#if DEBUG
		builder.Logging.AddDebug();
		builder.Logging.SetMinimumLevel(LogLevel.Debug);
#endif

		return builder.Build();
	}
}
