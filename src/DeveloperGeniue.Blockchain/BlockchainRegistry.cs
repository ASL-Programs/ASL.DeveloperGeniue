using System.Text.Json;
using System.Collections.Generic;
using System.Linq;

namespace DeveloperGeniue.Blockchain;

/// <summary>
/// Lightweight blockchain registry simulation used to track commit metadata.
/// Each commit is stored with its origin, optional features and an NFT token
/// identifier if minted. The ledger is persisted as JSON on disk.
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
        => await RegisterCommitAsync(commitHash, "unknown", null);

    public async Task RegisterCommitAsync(string commitHash, string origin, IEnumerable<string>? features)
    {
        await _lock.WaitAsync();
        try
        {
            var blocks = await LoadAsync();
            if (blocks.All(b => b.CommitHash != commitHash))
            {
                blocks.Add(new BlockchainRecord(commitHash, origin, features?.ToList() ?? new(), null));
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
            return blocks.Any(b => b.CommitHash == commitHash);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<bool> VerifyFeatureAsync(string commitHash, string feature)
    {
        await _lock.WaitAsync();
        try
        {
            var blocks = await LoadAsync();
            var record = blocks.FirstOrDefault(b => b.CommitHash == commitHash);
            return record != null && record.Features.Contains(feature);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<string> MintNftAsync(string commitHash)
    {
        await _lock.WaitAsync();
        try
        {
            var blocks = await LoadAsync();
            var record = blocks.FirstOrDefault(b => b.CommitHash == commitHash);
            if (record == null)
                throw new InvalidOperationException("Commit not found");

            if (string.IsNullOrEmpty(record.NftToken))
            {
                record.NftToken = Guid.NewGuid().ToString("N");
                await SaveAsync(blocks);
            }
            return record.NftToken!;
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task<List<BlockchainRecord>> LoadAsync()
    {
        if (!File.Exists(_ledgerPath))
            return new List<BlockchainRecord>();
        var json = await File.ReadAllTextAsync(_ledgerPath);
        return JsonSerializer.Deserialize<List<BlockchainRecord>>(json) ?? new();
    }

    private async Task SaveAsync(List<BlockchainRecord> blocks)
    {
        var json = JsonSerializer.Serialize(blocks);
        await File.WriteAllTextAsync(_ledgerPath, json);
    }
}
