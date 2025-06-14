using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace DeveloperGeniue.Core;

public class ConfigurationService : IConfigurationService
{
    private readonly ILogger<ConfigurationService> _logger;
    private readonly string _filePath;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly string? _passphrase;
    private Dictionary<string, JsonElement> _cache = new();

    public event EventHandler? SettingsChanged;

    public ConfigurationService(string? filePath = null)
        : this(Microsoft.Extensions.Logging.Abstractions.NullLogger<ConfigurationService>.Instance, filePath)

    {
    }

    public ConfigurationService(ILogger<ConfigurationService> logger, string? filePath = null)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _filePath = filePath ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".developer_geniue_config.json");

        _logger.LogInformation("Initializing configuration service with file {FilePath}", _filePath);

        if (File.Exists(_filePath))
        {
            try
            {
                _cache = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(File.ReadAllText(_filePath)) ?? new();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load configuration from {FilePath}", _filePath);
                _cache = new();
            }
        }
    }

    public async Task<T?> GetSettingAsync<T>(string key)
    {
        await _lock.WaitAsync();
        try
        {
            _logger.LogInformation("Retrieving setting {Key}", key);
            if (_cache.TryGetValue(key, out var val))
            {
                return JsonSerializer.Deserialize<T>(val);
            }
            return default;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task SetSettingAsync<T>(string key, T value)
    {
        await _lock.WaitAsync();
        try
        {
            _cache[key] = JsonSerializer.SerializeToElement(value);
            try
            {
                await SaveAsync(_cache);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save setting {Key}", key);
                throw;
            }

            SettingsChanged?.Invoke(this, EventArgs.Empty);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<IDictionary<string, JsonElement>> GetAllSettingsAsync()
    {
        await _lock.WaitAsync();
        try
        {
            return new Dictionary<string, JsonElement>(_cache);
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task<Dictionary<string, JsonElement>> LoadAsync()
    {
        if (!File.Exists(_filePath))
            return new Dictionary<string, JsonElement>();
        using var stream = File.OpenRead(_filePath);
        return await JsonSerializer.DeserializeAsync<Dictionary<string, JsonElement>>(stream) ?? new();
    }

    private async Task SaveAsync(Dictionary<string, JsonElement> data)
    {
        using var stream = File.Create(_filePath);
        await JsonSerializer.SerializeAsync(stream, data, new JsonSerializerOptions { WriteIndented = true });
        _logger.LogInformation("Settings saved to {FilePath}", _filePath);
    }
}
