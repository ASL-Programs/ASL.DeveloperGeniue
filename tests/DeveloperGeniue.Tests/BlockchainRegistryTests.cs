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
        await registry.RegisterCommitAsync(hash, "main", new[]{"feat"});
        var result = await registry.VerifyCommitAsync(hash);
        Assert.True(result);
        Assert.True(await registry.VerifyFeatureAsync(hash, "feat"));
        var token = await registry.MintNftAsync(hash);
        Assert.False(string.IsNullOrEmpty(token));
        File.Delete(temp);
    }
}
