using DeveloperGeniue.Core;
using DeveloperGeniue.Core.AI;

namespace DeveloperGeniue.Tests;

public class AIOrchestratorTests
{
    [Fact]
    public async Task ExecuteAsyncThrowsForUnknownProvider()
    {
        var file = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        var config = new ConfigurationService(file);
        await config.SetSettingAsync("OpenAIApiKey", "test");
        await config.SetSettingAsync("ClaudeApiKey", "test");
        var orchestrator = new AIOrchestrator(config);
        await Assert.ThrowsAsync<InvalidOperationException>(() => orchestrator.ExecuteAsync(new AIRequest("hi", "none")));
        File.Delete(file);
    }
}
