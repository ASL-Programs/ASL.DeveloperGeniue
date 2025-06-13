using System.Text.Json;

namespace DeveloperGeniue.Blockchain;

/// <summary>
/// Very lightweight blockchain registry simulation used to track commit hashes.
/// Commit hashes are appended to a local file acting as a blockchain ledger.
/// </summary>
public class BlockchainRegistry : IBlockchainRegistry
{
    private readonly string _ledgerPath;
    private readonly SemaphoreSlim _lock = new(1,1);

    public BlockchainRegistry(string? ledgerPath = null)
    {
        _ledgerPath = ledgerPath ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "devgen_blockchain.json");
    }

    public async Task RegisterCommitAsync(string commitHash)
    {
        await _lock.WaitAsync();
        try
        {
            var blocks = await LoadAsync();
            if (!blocks.Contains(commitHash))
            {
                blocks.Add(commitHash);
                await SaveAsync(blocks);
            }
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<bool> VerifyCommitAsync(string commitHash)
    {
        await _lock.WaitAsync();
        try
        {
            var blocks = await LoadAsync();
            return blocks.Contains(commitHash);
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task<HashSet<string>> LoadAsync()
    {
        if (!File.Exists(_ledgerPath))
            return new HashSet<string>();
        var json = await File.ReadAllTextAsync(_ledgerPath);
        return JsonSerializer.Deserialize<HashSet<string>>(json) ?? new();
    }

    private async Task SaveAsync(HashSet<string> blocks)
    {
        var json = JsonSerializer.Serialize(blocks);
        await File.WriteAllTextAsync(_ledgerPath, json);
    }
}
