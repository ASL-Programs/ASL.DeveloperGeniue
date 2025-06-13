namespace DeveloperGeniue.Blockchain;

public interface IBlockchainRegistry
{
    Task RegisterCommitAsync(string commitHash);
    Task RegisterCommitAsync(string commitHash, string origin, IEnumerable<string>? features);
    Task<bool> VerifyCommitAsync(string commitHash);
    Task<bool> VerifyFeatureAsync(string commitHash, string feature);
    Task<string> MintNftAsync(string commitHash);
}
