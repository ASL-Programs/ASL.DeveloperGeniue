using DeveloperGeniue.Blockchain;

namespace DeveloperGeniue.Tests;

public class BlockchainRegistryTests
{
    [Fact]
    public async Task RegisterAndVerifyCommit()
    {
        var temp = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        var registry = new BlockchainRegistry(temp);
        const string hash = "abc123";
        await registry.RegisterCommitAsync(hash);
        var result = await registry.VerifyCommitAsync(hash);
        Assert.True(result);
        File.Delete(temp);
    }
}
