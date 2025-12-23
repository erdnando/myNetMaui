# 🔒 Configuración de Secrets

Este proyecto utiliza un archivo `Secrets.cs` para almacenar credenciales sensibles que **NO deben subirse a GitHub**.

## 📋 Configuración Inicial

1. **Copia el archivo de ejemplo:**
   ```bash
   cp Secrets.cs.example Secrets.cs
   ```

2. **Edita `Secrets.cs` con tus credenciales reales:**
   - Abre `Secrets.cs`
   - Reemplaza `TU-CLIENT-ID-AQUI` con tu Google Client ID
   - Reemplaza `TU-CLIENT-SECRET-AQUI` con tu Google Client Secret

3. **Obtén tus credenciales de Google:**
   - Ve a [Google Cloud Console](https://console.cloud.google.com/apis/credentials)
   - Crea un proyecto (si no tienes uno)
   - Habilita la API de Google Sign-In
   - Crea credenciales OAuth 2.0
   - Configura las URIs de redirección:
     - Windows: `http://localhost:5000/`
     - Android: `com.erdnando.miappmaui:/oauth2redirect`

## ⚠️ IMPORTANTE

- ❌ **NUNCA** subas `Secrets.cs` a GitHub
- ✅ El archivo ya está en `.gitignore`
- ✅ Sube `Secrets.cs.example` como plantilla para otros desarrolladores
- ✅ Comparte las credenciales de forma segura (no por email ni chat público)

## 📁 Estructura

```
MiAppMaui/
├── Secrets.cs           ← ❌ NO se sube (contiene credenciales reales)
├── Secrets.cs.example   ← ✅ SÍ se sube (plantilla sin credenciales)
└── Services/
    └── GoogleAuthService.cs  ← Usa las constantes de Secrets.cs
```

## 🔧 Uso en el Código

```csharp
// En cualquier parte del código puedes usar:
var clientId = Secrets.GoogleClientId;
var clientSecret = Secrets.GoogleClientSecret;
```

## 🆘 Solución de Problemas

**Error: "The name 'Secrets' does not exist"**
- Asegúrate de haber creado el archivo `Secrets.cs` desde `Secrets.cs.example`
- Verifica que el namespace sea `MiAppMaui`

**Error de autenticación con Google**
- Verifica que las credenciales en `Secrets.cs` sean correctas
- Confirma que las URIs de redirección estén configuradas en Google Cloud Console
