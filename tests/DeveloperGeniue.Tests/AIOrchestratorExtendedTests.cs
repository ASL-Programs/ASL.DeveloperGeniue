using DeveloperGeniue.Core.AI;
using System.Threading.Tasks;
using Xunit;

namespace DeveloperGeniue.Tests;

public class AIOrchestratorExtendedTests
{
    private class StubClient : IAIClient
    {
        public AIRequest? ReceivedRequest { get; private set; }
        public Task<AIResponse> GetCompletionAsync(AIRequest request, CancellationToken cancellationToken = default)
        {
            ReceivedRequest = request;
            return Task.FromResult(new AIResponse("ok"));
        }
    }

    [Fact]
    public async Task AnalyzeCodeAsyncUsesOpenAI()
    {
        var client = new StubClient();
        var orchestrator = new AIOrchestrator();
        orchestrator.RegisterProvider("OpenAI", client);
        await orchestrator.AnalyzeCodeAsync("class C{}", CancellationToken.None);
        Assert.Equal("OpenAI", client.ReceivedRequest?.Provider);
    }
}
