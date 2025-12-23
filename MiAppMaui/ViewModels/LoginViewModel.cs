using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiAppMaui.Services;

namespace MiAppMaui.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    private readonly GoogleAuthService _googleAuthService;
    private readonly UserSessionService _sessionService;

    public LoginViewModel(GoogleAuthService googleAuthService, UserSessionService sessionService)
    {
        _googleAuthService = googleAuthService;
        _sessionService = sessionService;
        Title = "Iniciar Sesión";
        
        // Cargar usuario si ya está logueado
        LoadCurrentUser();
    }

    [ObservableProperty]
    private string? userName;

    [ObservableProperty]
    private string? userEmail;

    [ObservableProperty]
    private string? userPhotoUrl;

    [ObservableProperty]
    private bool isLoggedIn;

    [ObservableProperty]
    private string statusMessage = "Inicia sesión con tu cuenta de Google";

    /// <summary>
    /// Comando para iniciar sesión con Google
    /// </summary>
    [RelayCommand]
    private async Task SignInWithGoogleAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            StatusMessage = "Abriendo navegador...";

            var userInfo = await _googleAuthService.SignInAsync();

            if (userInfo != null)
            {
                // Autenticación exitosa
                UserName = userInfo.Name;
                UserEmail = userInfo.Email;
                UserPhotoUrl = userInfo.Picture;
                IsLoggedIn = true;
                StatusMessage = $"¡Bienvenido, {userInfo.GivenName}! 🎉";

                System.Diagnostics.Debug.WriteLine($"✅ Login exitoso:");
                System.Diagnostics.Debug.WriteLine($"   Nombre: {userInfo.Name}");
                System.Diagnostics.Debug.WriteLine($"   Email: {userInfo.Email}");
                System.Diagnostics.Debug.WriteLine($"   Foto: {userInfo.Picture}");

                // Guardar usuario en sesión
                _sessionService.SaveUser(userInfo);
                System.Diagnostics.Debug.WriteLine($"✅ Usuario guardado en sesión");

                // Navegar a la página principal
                await Shell.Current.GoToAsync("//MainPage");
            }
            else
            {
                // Usuario canceló
                StatusMessage = "Inicio de sesión cancelado";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
            await Shell.Current.DisplayAlert(
                "Error", 
                "No se pudo iniciar sesión. Por favor intenta nuevamente.", 
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
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
            UserName = null;
            UserEmail = null;
            UserPhotoUrl = null;
            IsLoggedIn = false;
            StatusMessage = "Inicia sesión con tu cuenta de Google";

            // Limpiar sesión
            _sessionService.ClearUser();
        }
    }

    /// <summary>
    /// Comando para navegar a la página principal
    /// </summary>
    [RelayCommand]
    private async Task NavigateToMainAsync()
    {
        await Shell.Current.GoToAsync("//MainPage");
    }

    /// <summary>
    /// Carga el usuario actual desde la sesión
    /// </summary>
    private void LoadCurrentUser()
    {
        var user = _sessionService.GetCurrentUser();
        if (user != null)
        {
            UserName = user.Name;
            UserEmail = user.Email;
            UserPhotoUrl = user.Picture;
            IsLoggedIn = true;
            StatusMessage = $"Bienvenido de nuevo, {user.Name}! 👋";
        }
    }

    /// <summary>
    /// Guarda el usuario en la base de datos local
    /// </summary>
    private async Task SaveUserToDatabase(GoogleUserInfo userInfo)
    {
        try
        {
            // TODO: Implementar guardado en DatabaseService
            // Ejemplo:
            // var user = new User
            // {
            //     GoogleId = userInfo.Id,
            //     Name = userInfo.Name,
            //     Email = userInfo.Email,
            //     PhotoUrl = userInfo.Picture
            // };
            // await _databaseService.SaveAsync(user);

            await Task.CompletedTask; // Placeholder por ahora
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error guardando usuario: {ex.Message}");
        }
    }
}
