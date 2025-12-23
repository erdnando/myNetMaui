namespace MiAppMaui.Services;

/// <summary>
/// Servicio para gestionar la sesión del usuario
/// </summary>
public class UserSessionService
{
    private const string KEY_USER_NAME = "user_name";
    private const string KEY_USER_EMAIL = "user_email";
    private const string KEY_USER_PHOTO = "user_photo_url";
    private const string KEY_IS_LOGGED_IN = "is_logged_in";

    public event EventHandler<GoogleUserInfo>? UserLoggedIn;
    public event EventHandler? UserLoggedOut;

    /// <summary>
    /// Guarda la información del usuario en las preferencias
    /// </summary>
    public void SaveUser(GoogleUserInfo userInfo)
    {
        if (userInfo == null) return;

        Preferences.Default.Set(KEY_USER_NAME, userInfo.Name ?? "");
        Preferences.Default.Set(KEY_USER_EMAIL, userInfo.Email ?? "");
        Preferences.Default.Set(KEY_USER_PHOTO, userInfo.Picture ?? "");
        Preferences.Default.Set(KEY_IS_LOGGED_IN, true);

        UserLoggedIn?.Invoke(this, userInfo);
    }

    /// <summary>
    /// Limpia la sesión del usuario
    /// </summary>
    public void ClearUser()
    {
        Preferences.Default.Remove(KEY_USER_NAME);
        Preferences.Default.Remove(KEY_USER_EMAIL);
        Preferences.Default.Remove(KEY_USER_PHOTO);
        Preferences.Default.Set(KEY_IS_LOGGED_IN, false);

        UserLoggedOut?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Obtiene el usuario guardado (si existe)
    /// </summary>
    public GoogleUserInfo? GetCurrentUser()
    {
        var isLoggedIn = Preferences.Default.Get(KEY_IS_LOGGED_IN, false);
        if (!isLoggedIn) return null;

        return new GoogleUserInfo
        {
            Name = Preferences.Default.Get(KEY_USER_NAME, string.Empty),
            Email = Preferences.Default.Get(KEY_USER_EMAIL, string.Empty),
            Picture = Preferences.Default.Get(KEY_USER_PHOTO, string.Empty)
        };
    }

    /// <summary>
    /// Verifica si hay un usuario logueado
    /// </summary>
    public bool IsLoggedIn => Preferences.Default.Get(KEY_IS_LOGGED_IN, false);

    /// <summary>
    /// Obtiene el nombre del usuario actual
    /// </summary>
    public string CurrentUserName => Preferences.Default.Get(KEY_USER_NAME, "Usuario");

    /// <summary>
    /// Obtiene el email del usuario actual
    /// </summary>
    public string CurrentUserEmail => Preferences.Default.Get(KEY_USER_EMAIL, "");

    /// <summary>
    /// Obtiene la URL de la foto del usuario actual
    /// </summary>
    public string CurrentUserPhotoUrl => Preferences.Default.Get(KEY_USER_PHOTO, "dotnet_bot.png");
}
