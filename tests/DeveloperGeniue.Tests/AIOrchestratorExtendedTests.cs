using DeveloperGeniue.Core;
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
        var file = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        var config = new ConfigurationService(file);
        await config.SetSettingAsync("OpenAIApiKey", "test");
        await config.SetSettingAsync("ClaudeApiKey", "test");
        var orchestrator = new AIOrchestrator(config);
        orchestrator.RegisterProvider("OpenAI", client);
        await orchestrator.AnalyzeCodeAsync("class C{}", CancellationToken.None);
        Assert.Equal("OpenAI", client.ReceivedRequest?.Provider);
        File.Delete(file);
    }
}
