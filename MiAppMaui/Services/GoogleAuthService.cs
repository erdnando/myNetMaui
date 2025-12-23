using System.Text;
using System.Text.Json;
using System.Web;

namespace MiAppMaui.Services;

public class GoogleAuthService
{
    // 🔑 Credenciales obtenidas del archivo Secrets.cs (no se sube a GitHub)
    private const string ClientId = Secrets.GoogleClientId;
    private const string ClientSecret = Secrets.GoogleClientSecret;
    
#if WINDOWS
    private const string RedirectUri = "http://localhost:5000/";
#else
    private const string RedirectUri = "https://localhost/callback";
#endif
    
    private const string AuthorizationEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";
    private const string TokenEndpoint = "https://oauth2.googleapis.com/token";
    private const string UserInfoEndpoint = "https://www.googleapis.com/oauth2/v2/userinfo";

    private readonly HttpClient _httpClient;

    public GoogleAuthService()
    {
        _httpClient = new HttpClient();
    }

    /// <summary>
    /// Inicia el flujo de autenticación con Google
    /// </summary>
    public async Task<GoogleUserInfo?> SignInAsync()
    {
        try
        {
            // Generar state y code_verifier para PKCE (seguridad adicional)
            var state = Guid.NewGuid().ToString("N");
            var codeVerifier = GenerateCodeVerifier();
            var codeChallenge = GenerateCodeChallenge(codeVerifier);

            // Construir URL de autorización
            var authUrl = $"{AuthorizationEndpoint}?" +
                $"client_id={Uri.EscapeDataString(ClientId)}&" +
                $"redirect_uri={Uri.EscapeDataString(RedirectUri)}&" +
                $"response_type=code&" +
                $"scope={Uri.EscapeDataString("openid profile email")}&" +
                $"state={state}&" +
                $"code_challenge={codeChallenge}&" +
                $"code_challenge_method=S256";

#if WINDOWS
            // En Windows, usar el navegador del sistema
            var callbackUrl = await AuthenticateWithBrowserAsync(authUrl, state);
            
            // Parsear la URL de callback
            var uri = new Uri(callbackUrl);
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
            
            System.Diagnostics.Debug.WriteLine($"📥 Callback URL recibida: {callbackUrl}");
            
            // Verificar si hay error
            var error = query["error"];
            if (!string.IsNullOrEmpty(error))
            {
                var errorDescription = query["error_description"];
                throw new Exception($"Error de Google: {error} - {errorDescription}");
            }
            
            // Verificar state
            var returnedState = query["state"];
            if (returnedState != state)
            {
                throw new Exception("State mismatch - posible ataque CSRF");
            }

            // Obtener código
            var code = query["code"];
            if (string.IsNullOrEmpty(code))
            {
                throw new Exception("No se recibió código de autorización");
            }
            
            System.Diagnostics.Debug.WriteLine($"✅ Código de autorización recibido");
#else
            // En Android/iOS usar WebAuthenticator
            var result = await WebAuthenticator.AuthenticateAsync(
                new Uri(authUrl),
                new Uri(RedirectUri));

            // Verificar state para prevenir CSRF
            if (!result.Properties.TryGetValue("state", out var returnedState) || 
                returnedState != state)
            {
                throw new Exception("State mismatch - posible ataque CSRF");
            }

            // Obtener código de autorización
            if (!result.Properties.TryGetValue("code", out var code))
            {
                throw new Exception("No se recibió código de autorización");
            }
#endif

            // Intercambiar código por token de acceso
            var accessToken = await ExchangeCodeForTokenAsync(code, codeVerifier);

            // Obtener información del usuario
            var userInfo = await GetUserInfoAsync(accessToken);

            return userInfo;
        }
        catch (TaskCanceledException)
        {
            // Usuario canceló la autenticación
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error en SignInAsync: {ex.Message}");
            throw;
        }
    }

#if WINDOWS
    /// <summary>
    /// Autenticación usando el navegador del sistema en Windows
    /// </summary>
    private async Task<string> AuthenticateWithBrowserAsync(string authUrl, string state)
    {
        // Crear servidor HTTP local temporal
        var listener = new System.Net.HttpListener();
        listener.Prefixes.Add("http://localhost:5000/");
        listener.Start();

        // Abrir el navegador
        var psi = new System.Diagnostics.ProcessStartInfo
        {
            FileName = authUrl,
            UseShellExecute = true
        };
        System.Diagnostics.Process.Start(psi);

        // Esperar la respuesta
        var context = await listener.GetContextAsync();
        var response = context.Response;

        // Obtener la URL completa con el código
        var callbackUrl = context.Request.Url?.ToString() ?? "";

        // Enviar respuesta al navegador
        var responseString = @"
            <html>
            <head><title>Autenticación Exitosa</title></head>
            <body style='font-family: Arial; text-align: center; padding: 50px;'>
                <h2>✅ ¡Autenticación exitosa!</h2>
                <p>Ya puedes cerrar esta ventana y volver a la aplicación.</p>
            </body>
            </html>";
        var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
        response.ContentLength64 = buffer.Length;
        await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        response.OutputStream.Close();
        listener.Stop();

        return callbackUrl;
    }
#endif

    /// <summary>
    /// Intercambia el código de autorización por un token de acceso
    /// </summary>
    private async Task<string> ExchangeCodeForTokenAsync(string code, string codeVerifier)
    {
        try
        {
            var tokenRequestBody = new Dictionary<string, string>
            {
                { "code", code },
                { "client_id", ClientId },
                { "client_secret", ClientSecret }, // ⚠️ Requerido para Web Client
                { "redirect_uri", RedirectUri },
                { "grant_type", "authorization_code" },
                { "code_verifier", codeVerifier }
            };

            System.Diagnostics.Debug.WriteLine($"🔑 Intercambiando código por token...");
            System.Diagnostics.Debug.WriteLine($"📍 Redirect URI: {RedirectUri}");

            var response = await _httpClient.PostAsync(
                TokenEndpoint,
                new FormUrlEncodedContent(tokenRequestBody));

            var json = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"📄 Response: {json}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error al obtener token: {response.StatusCode} - {json}");
            }

            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(json);

            if (tokenResponse?.AccessToken == null)
            {
                throw new Exception("No se pudo obtener el token de acceso");
            }

            System.Diagnostics.Debug.WriteLine($"✅ Token obtenido exitosamente");
            return tokenResponse.AccessToken;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Error en ExchangeCodeForTokenAsync: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Obtiene la información del usuario usando el token de acceso
    /// </summary>
    private async Task<GoogleUserInfo> GetUserInfoAsync(string accessToken)
    {
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

        var response = await _httpClient.GetAsync(UserInfoEndpoint);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var userInfo = JsonSerializer.Deserialize<GoogleUserInfo>(json);

        if (userInfo == null)
        {
            throw new Exception("No se pudo obtener la información del usuario");
        }

        return userInfo;
    }

    /// <summary>
    /// Genera un code_verifier aleatorio para PKCE
    /// </summary>
    private static string GenerateCodeVerifier()
    {
        var bytes = new byte[32];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");
    }

    /// <summary>
    /// Genera el code_challenge a partir del code_verifier
    /// </summary>
    private static string GenerateCodeChallenge(string codeVerifier)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(codeVerifier);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");
    }
}

// 📦 Modelos de datos

/// <summary>
/// Respuesta del endpoint de token
/// </summary>
public class TokenResponse
{
    [System.Text.Json.Serialization.JsonPropertyName("access_token")]
    public string? AccessToken { get; set; }

    [System.Text.Json.Serialization.JsonPropertyName("token_type")]
    public string? TokenType { get; set; }

    [System.Text.Json.Serialization.JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    [System.Text.Json.Serialization.JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; set; }

    [System.Text.Json.Serialization.JsonPropertyName("scope")]
    public string? Scope { get; set; }
}

/// <summary>
/// Información del usuario de Google
/// </summary>
public class GoogleUserInfo
{
    [System.Text.Json.Serialization.JsonPropertyName("id")]
    public string? Id { get; set; }

    [System.Text.Json.Serialization.JsonPropertyName("email")]
    public string? Email { get; set; }

    [System.Text.Json.Serialization.JsonPropertyName("verified_email")]
    public bool VerifiedEmail { get; set; }

    [System.Text.Json.Serialization.JsonPropertyName("name")]
    public string? Name { get; set; }

    [System.Text.Json.Serialization.JsonPropertyName("given_name")]
    public string? GivenName { get; set; }

    [System.Text.Json.Serialization.JsonPropertyName("family_name")]
    public string? FamilyName { get; set; }

    [System.Text.Json.Serialization.JsonPropertyName("picture")]
    public string? Picture { get; set; }

    [System.Text.Json.Serialization.JsonPropertyName("locale")]
    public string? Locale { get; set; }
}
