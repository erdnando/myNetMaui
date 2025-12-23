using SQLite;
using System.Linq.Expressions;

namespace MiAppMaui.Services;

/// <summary>
/// Servicio genérico para operaciones CRUD con SQLite
/// Maneja la persistencia de datos local de forma asíncrona
/// </summary>
public class DatabaseService
{
    private SQLiteAsyncConnection? _database;
    private readonly string _dbPath;

    public DatabaseService()
    {
        // Configurar la ruta de la base de datos según la plataforma
        _dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "MiAppMaui.db3"
        );
    }

    /// <summary>
    /// Inicializa la conexión a la base de datos
    /// Crea las tablas automáticamente según los modelos registrados
    /// </summary>
    private async Task InitAsync()
    {
        if (_database is not null)
            return;

        _database = new SQLiteAsyncConnection(_dbPath);

        // Aquí se crearán las tablas automáticamente cuando agregues modelos
        // Ejemplo: await _database.CreateTableAsync<Todo>();
        // Por ahora dejamos esto listo para cuando creemos los modelos
    }

    /// <summary>
    /// Crea la tabla para un tipo específico si no existe
    /// </summary>
    public async Task CreateTableAsync<T>() where T : new()
    {
        await InitAsync();
        await _database!.CreateTableAsync<T>();
    }

    #region CRUD Operations

    /// <summary>
    /// Obtiene todos los elementos de un tipo específico
    /// </summary>
    /// <typeparam name="T">Tipo de entidad</typeparam>
    /// <returns>Lista de elementos</returns>
    public async Task<List<T>> GetAllAsync<T>() where T : new()
    {
        await InitAsync();
        return await _database!.Table<T>().ToListAsync();
    }

    /// <summary>
    /// Obtiene elementos que coincidan con un predicado
    /// </summary>
    /// <typeparam name="T">Tipo de entidad</typeparam>
    /// <param name="predicate">Condición de búsqueda</param>
    /// <returns>Lista de elementos que cumplen la condición</returns>
    public async Task<List<T>> GetAsync<T>(Expression<Func<T, bool>> predicate) where T : new()
    {
        await InitAsync();
        return await _database!.Table<T>().Where(predicate).ToListAsync();
    }

    /// <summary>
    /// Obtiene un elemento por su ID
    /// </summary>
    /// <typeparam name="T">Tipo de entidad</typeparam>
    /// <param name="id">ID del elemento</param>
    /// <returns>Elemento encontrado o null</returns>
    public async Task<T?> GetByIdAsync<T>(int id) where T : new()
    {
        await InitAsync();
        return await _database!.FindAsync<T>(id);
    }

    /// <summary>
    /// Guarda o actualiza un elemento
    /// Si el elemento tiene ID = 0, lo inserta; si no, lo actualiza
    /// </summary>
    /// <typeparam name="T">Tipo de entidad</typeparam>
    /// <param name="item">Elemento a guardar</param>
    /// <returns>Número de filas afectadas</returns>
    public async Task<int> SaveAsync<T>(T item) where T : new()
    {
        await InitAsync();
        
        // Verificar si el objeto tiene una propiedad "Id"
        var idProperty = typeof(T).GetProperty("Id");
        if (idProperty != null)
        {
            var id = (int)(idProperty.GetValue(item) ?? 0);
            
            if (id != 0)
            {
                // Actualizar
                return await _database!.UpdateAsync(item);
            }
        }
        
        // Insertar nuevo
        return await _database!.InsertAsync(item);
    }

    /// <summary>
    /// Actualiza un elemento existente
    /// </summary>
    /// <typeparam name="T">Tipo de entidad</typeparam>
    /// <param name="item">Elemento a actualizar</param>
    /// <returns>Número de filas afectadas</returns>
    public async Task<int> UpdateAsync<T>(T item) where T : new()
    {
        await InitAsync();
        return await _database!.UpdateAsync(item);
    }

    /// <summary>
    /// Elimina un elemento
    /// </summary>
    /// <typeparam name="T">Tipo de entidad</typeparam>
    /// <param name="item">Elemento a eliminar</param>
    /// <returns>Número de filas afectadas</returns>
    public async Task<int> DeleteAsync<T>(T item) where T : new()
    {
        await InitAsync();
        return await _database!.DeleteAsync(item);
    }

    /// <summary>
    /// Elimina un elemento por su ID
    /// </summary>
    /// <typeparam name="T">Tipo de entidad</typeparam>
    /// <param name="id">ID del elemento a eliminar</param>
    /// <returns>Número de filas afectadas</returns>
    public async Task<int> DeleteByIdAsync<T>(int id) where T : new()
    {
        await InitAsync();
        var item = await GetByIdAsync<T>(id);
        if (item != null)
        {
            return await _database!.DeleteAsync(item);
        }
        return 0;
    }

    /// <summary>
    /// Elimina todos los elementos de un tipo
    /// </summary>
    /// <typeparam name="T">Tipo de entidad</typeparam>
    /// <returns>Número de filas afectadas</returns>
    public async Task<int> DeleteAllAsync<T>() where T : new()
    {
        await InitAsync();
        return await _database!.DeleteAllAsync<T>();
    }

    /// <summary>
    /// Cuenta los elementos de un tipo
    /// </summary>
    /// <typeparam name="T">Tipo de entidad</typeparam>
    /// <returns>Número total de elementos</returns>
    public async Task<int> CountAsync<T>() where T : new()
    {
        await InitAsync();
        return await _database!.Table<T>().CountAsync();
    }

    /// <summary>
    /// Cuenta elementos que coincidan con un predicado
    /// </summary>
    /// <typeparam name="T">Tipo de entidad</typeparam>
    /// <param name="predicate">Condición de búsqueda</param>
    /// <returns>Número de elementos que cumplen la condición</returns>
    public async Task<int> CountAsync<T>(Expression<Func<T, bool>> predicate) where T : new()
    {
        await InitAsync();
        return await _database!.Table<T>().Where(predicate).CountAsync();
    }

    #endregion

    #region Batch Operations

    /// <summary>
    /// Inserta múltiples elementos de forma eficiente
    /// </summary>
    /// <typeparam name="T">Tipo de entidad</typeparam>
    /// <param name="items">Lista de elementos a insertar</param>
    /// <returns>Número de filas afectadas</returns>
    public async Task<int> InsertAllAsync<T>(IEnumerable<T> items) where T : new()
    {
        await InitAsync();
        return await _database!.InsertAllAsync(items);
    }

    /// <summary>
    /// Actualiza múltiples elementos de forma eficiente
    /// </summary>
    /// <typeparam name="T">Tipo de entidad</typeparam>
    /// <param name="items">Lista de elementos a actualizar</param>
    /// <returns>Número de filas afectadas</returns>
    public async Task<int> UpdateAllAsync<T>(IEnumerable<T> items) where T : new()
    {
        await InitAsync();
        return await _database!.UpdateAllAsync(items);
    }

    #endregion

    #region Advanced Queries

    /// <summary>
    /// Ejecuta una consulta SQL personalizada
    /// </summary>
    /// <typeparam name="T">Tipo de entidad</typeparam>
    /// <param name="query">Consulta SQL</param>
    /// <param name="args">Parámetros de la consulta</param>
    /// <returns>Lista de resultados</returns>
    public async Task<List<T>> QueryAsync<T>(string query, params object[] args) where T : new()
    {
        await InitAsync();
        return await _database!.QueryAsync<T>(query, args);
    }

    /// <summary>
    /// Ejecuta un comando SQL sin retornar resultados
    /// </summary>
    /// <param name="query">Comando SQL</param>
    /// <param name="args">Parámetros del comando</param>
    /// <returns>Número de filas afectadas</returns>
    public async Task<int> ExecuteAsync(string query, params object[] args)
    {
        await InitAsync();
        return await _database!.ExecuteAsync(query, args);
    }

    #endregion

    #region Database Maintenance

    /// <summary>
    /// Cierra la conexión a la base de datos
    /// </summary>
    public async Task CloseAsync()
    {
        if (_database != null)
        {
            await _database.CloseAsync();
            _database = null;
        }
    }

    /// <summary>
    /// Elimina completamente la base de datos
    /// ⚠️ Usar con precaución - esto borra todos los datos
    /// </summary>
    public async Task DeleteDatabaseAsync()
    {
        await CloseAsync();
        if (File.Exists(_dbPath))
        {
            File.Delete(_dbPath);
        }
    }

    /// <summary>
    /// Obtiene información sobre la base de datos
    /// </summary>
    /// <returns>Información de la BD (ruta, tamaño, etc.)</returns>
    public async Task<DatabaseInfo> GetDatabaseInfoAsync()
    {
        await InitAsync();
        
        var info = new DatabaseInfo
        {
            Path = _dbPath,
            Exists = File.Exists(_dbPath),
            SizeInBytes = File.Exists(_dbPath) ? new FileInfo(_dbPath).Length : 0
        };

        return info;
    }

    #endregion
}

/// <summary>
/// Información sobre la base de datos
/// </summary>
public class DatabaseInfo
{
    public string Path { get; set; } = string.Empty;
    public bool Exists { get; set; }
    public long SizeInBytes { get; set; }
    public string SizeFormatted => $"{SizeInBytes / 1024.0:F2} KB";
}
