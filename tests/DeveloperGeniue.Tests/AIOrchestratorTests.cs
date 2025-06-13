using DeveloperGeniue.Core.AI;

namespace DeveloperGeniue.Tests;

public class AIOrchestratorTests
{
    [Fact]
    public async Task ExecuteAsyncThrowsForUnknownProvider()
    {
        var orchestrator = new AIOrchestrator();
        await Assert.ThrowsAsync<InvalidOperationException>(() => orchestrator.ExecuteAsync(new AIRequest("hi", "none")));
    }
}
