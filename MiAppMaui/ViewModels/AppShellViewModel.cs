using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiAppMaui.Services;

namespace MiAppMaui.ViewModels;

public partial class AppShellViewModel : BaseViewModel
{
    private readonly UserSessionService _sessionService;

    public AppShellViewModel(UserSessionService sessionService)
    {
        _sessionService = sessionService;
        
        // Suscribirse a eventos de sesión
        _sessionService.UserLoggedIn += OnUserLoggedIn;
        _sessionService.UserLoggedOut += OnUserLoggedOut;
        
        // Cargar usuario actual si existe
        LoadCurrentUser();
    }

    [ObservableProperty]
    private bool isLoggedIn;

    [ObservableProperty]
    private string userName = "Usuario";

    [ObservableProperty]
    private string userEmail = "";

    [ObservableProperty]
    private string userPhotoUrl = "dotnet_bot.png";

    /// <summary>
    /// Carga el usuario actual desde las preferencias
    /// </summary>
    private void LoadCurrentUser()
    {
        System.Diagnostics.Debug.WriteLine($"🔍 AppShellViewModel: Cargando usuario...");
        var user = _sessionService.GetCurrentUser();
        if (user != null)
        {
            IsLoggedIn = true;
            UserName = user.Name ?? "Usuario";
            UserEmail = user.Email ?? "";
            UserPhotoUrl = user.Picture ?? "dotnet_bot.png";
            System.Diagnostics.Debug.WriteLine($"✅ Usuario cargado en AppShell:");
            System.Diagnostics.Debug.WriteLine($"   Nombre: {UserName}");
            System.Diagnostics.Debug.WriteLine($"   Email: {UserEmail}");
            System.Diagnostics.Debug.WriteLine($"   Foto: {UserPhotoUrl}");
        }
        else
        {
            System.Diagnostics.Debug.WriteLine($"❌ No hay usuario guardado");
        }
    }

    /// <summary>
    /// Evento cuando el usuario inicia sesión
    /// </summary>
    private void OnUserLoggedIn(object? sender, GoogleUserInfo userInfo)
    {
        IsLoggedIn = true;
        UserName = userInfo.Name ?? "Usuario";
        UserEmail = userInfo.Email ?? "";
        UserPhotoUrl = userInfo.Picture ?? "dotnet_bot.png";
    }

    /// <summary>
    /// Evento cuando el usuario cierra sesión
    /// </summary>
    private void OnUserLoggedOut(object? sender, EventArgs e)
    {
        IsLoggedIn = false;
        UserName = "Usuario";
        UserEmail = "";
        UserPhotoUrl = "dotnet_bot.png";
    }

    /// <summary>
    /// Comando para ir a la página de perfil
    /// </summary>
    [RelayCommand]
    private async Task GoToProfileAsync()
    {
        await Shell.Current.GoToAsync("//LoginPage");
    }

    /// <summary>
    /// Comando para cerrar sesión
    /// </summary>
    [RelayCommand]
    private async Task SignOutAsync()
    {
        var confirm = await Shell.Current.DisplayAlert(
            "Cerrar Sesión",
            "¿Estás seguro que deseas cerrar sesión?",
            "Sí",
            "No");

        if (confirm)
        {
            _sessionService.ClearUser();
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
