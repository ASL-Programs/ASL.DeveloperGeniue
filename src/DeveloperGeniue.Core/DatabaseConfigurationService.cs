using Microsoft.Data.Sqlite;
using System.Text.Json;

namespace DeveloperGeniue.Core;

/// <summary>
/// Stores configuration settings in a SQLite database.
/// </summary>
public class DatabaseConfigurationService : IConfigurationService
{
    private readonly string _connectionString;

    public DatabaseConfigurationService(string? databasePath = null)
    {
        var path = databasePath ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "geniue_config.db");
        _connectionString = $"Data Source={path}";
        EnsureDatabase();
    }

    private void EnsureDatabase()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = "CREATE TABLE IF NOT EXISTS Settings(Key TEXT PRIMARY KEY, Value TEXT NOT NULL);";
        cmd.ExecuteNonQuery();
    }

    public event EventHandler? SettingsChanged;

    public async Task<T?> GetSettingAsync<T>(string key)
    {
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
        var json = JsonSerializer.Serialize(value);
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = "INSERT INTO Settings(Key, Value) VALUES(@key,@val) ON CONFLICT(Key) DO UPDATE SET Value = excluded.Value";
        cmd.Parameters.AddWithValue("@key", key);
        cmd.Parameters.AddWithValue("@val", json);
        await cmd.ExecuteNonQueryAsync();
        SettingsChanged?.Invoke(this, EventArgs.Empty);
    }
}
