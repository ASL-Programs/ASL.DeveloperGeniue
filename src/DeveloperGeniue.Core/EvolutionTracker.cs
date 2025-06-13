using System.Text.Json;

namespace DeveloperGeniue.Core;

/// <summary>
/// Tracks code evolution by persisting commit metadata to a JSON database file.
/// </summary>
public class EvolutionTracker : IEvolutionTracker
{
    private readonly string _dbPath;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public EvolutionTracker(string? dbPath = null)
    {
        _dbPath = dbPath ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "devgen_evolution.json");
    }

    public async Task RecordAsync(EvolutionRecord record)
    {
        await _lock.WaitAsync();
        try
        {
            var records = await LoadAsync();
            records.Add(record);
            await SaveAsync(records);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<IReadOnlyList<EvolutionRecord>> GetRecordsAsync()
    {
        await _lock.WaitAsync();
        try
        {
            return await LoadAsync();
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task<List<EvolutionRecord>> LoadAsync()
    {
        if (!File.Exists(_dbPath))
            return new List<EvolutionRecord>();
        var json = await File.ReadAllTextAsync(_dbPath);
        return JsonSerializer.Deserialize<List<EvolutionRecord>>(json) ?? new();
    }

    private async Task SaveAsync(List<EvolutionRecord> records)
    {
        var json = JsonSerializer.Serialize(records);
        await File.WriteAllTextAsync(_dbPath, json);
    }
}
