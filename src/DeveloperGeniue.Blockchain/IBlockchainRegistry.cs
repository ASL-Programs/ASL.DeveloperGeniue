namespace DeveloperGeniue.Blockchain;

public interface IBlockchainRegistry
{
    Task RegisterCommitAsync(string commitHash);
    Task<bool> VerifyCommitAsync(string commitHash);
}
