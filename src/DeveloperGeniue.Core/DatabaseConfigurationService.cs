using Microsoft.Data.Sqlite;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace DeveloperGeniue.Core;

/// <summary>
/// Stores configuration settings in a SQLite database.
/// </summary>
public class DatabaseConfigurationService : IConfigurationService
{
    private readonly ILogger<DatabaseConfigurationService> _logger;
    private readonly string _connectionString;
    private readonly string? _passphrase;


    public DatabaseConfigurationService(string? databasePath = null)
        : this(Microsoft.Extensions.Logging.Abstractions.NullLogger<DatabaseConfigurationService>.Instance, databasePath)
    {
    }

    public DatabaseConfigurationService(ILogger<DatabaseConfigurationService> logger, string? databasePath = null)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        var path = databasePath ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "geniue_config.db");
        _connectionString = $"Data Source={path}";
        _passphrase = passphrase;
        EnsureDatabase();
    }

    private void EnsureDatabase()
    {
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS Settings(Key TEXT PRIMARY KEY, Value TEXT NOT NULL);";
            cmd.ExecuteNonQuery();
            _logger.LogInformation("Configuration database initialized at {ConnectionString}", _connectionString);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize configuration database at {ConnectionString}", _connectionString);
            throw;
        }
    }

    public event EventHandler? SettingsChanged;

    public async Task<T?> GetSettingAsync<T>(string key)
    {
        _logger.LogInformation("Retrieving setting {Key}", key);
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT Value FROM Settings WHERE Key = @key";
        cmd.Parameters.AddWithValue("@key", key);
        var result = await cmd.ExecuteScalarAsync();
        if (result is string s)
            return JsonSerializer.Deserialize<T>(s);
        return default;
    }

    public async Task<IDictionary<string, JsonElement>> GetAllSettingsAsync()
    {
        var dict = new Dictionary<string, JsonElement>();
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT Key, Value FROM Settings";
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var key = reader.GetString(0);
            var value = JsonSerializer.Deserialize<JsonElement>(reader.GetString(1));
            dict[key] = value;
        }
        return dict;
    }

    public async Task SetSettingAsync<T>(string key, T value)
    {
        string json;
        if (key.EndsWith("ApiKey", StringComparison.OrdinalIgnoreCase) && value is string str)
        {
            var encrypted = CryptoHelper.Encrypt(str, _passphrase);
            json = JsonSerializer.Serialize(encrypted);
        }
        else
        {
            json = JsonSerializer.Serialize(value);
        }
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = "INSERT INTO Settings(Key, Value) VALUES(@key,@val) ON CONFLICT(Key) DO UPDATE SET Value = excluded.Value";
        cmd.Parameters.AddWithValue("@key", key);
        cmd.Parameters.AddWithValue("@val", json);
        try
        {
            await cmd.ExecuteNonQueryAsync();
            _logger.LogInformation("Persisted setting {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to persist setting {Key}", key);
            throw;
        }
        SettingsChanged?.Invoke(this, EventArgs.Empty);
    }
}
