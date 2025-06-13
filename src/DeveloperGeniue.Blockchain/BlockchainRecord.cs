using System.Collections.Generic;

namespace DeveloperGeniue.Blockchain;

/// <summary>
/// Represents a single commit's entry in the blockchain ledger.
/// </summary>
public class BlockchainRecord
{
    public string CommitHash { get; init; }
    public string Origin { get; init; }
    public List<string> Features { get; init; }
    public string? NftToken { get; set; }

    public BlockchainRecord(string commitHash, string origin, List<string> features, string? nftToken)
    {
        CommitHash = commitHash;
        Origin = origin;
        Features = features;
        NftToken = nftToken;
    }
}
