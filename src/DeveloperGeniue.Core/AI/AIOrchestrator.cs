using System.Collections.Concurrent;

namespace DeveloperGeniue.Core.AI;

public class AIOrchestrator
{
    private readonly ConcurrentDictionary<string, IAIClient> _providers = new();

    public AIOrchestrator()
    {
        // Register built-in providers for convenience
        RegisterProvider("OpenAI", new OpenAIClient());
        RegisterProvider("Claude", new ClaudeAIClient());
    }

    public void RegisterProvider(string name, IAIClient client)
    {
        _providers[name] = client;
    }

    public async Task<AIResponse> ExecuteAsync(AIRequest request, CancellationToken cancellationToken = default)
    {
        if (!_providers.TryGetValue(request.Provider, out var provider))
        {
            throw new InvalidOperationException($"Provider '{request.Provider}' not registered.");
        }

        return await provider.GetCompletionAsync(request, cancellationToken);
    }

    public Task<AIResponse> AnalyzeCodeAsync(string code, CancellationToken cancellationToken = default)
        => ExecuteAsync(new AIRequest($"Analyze the following C# code for potential issues:\n{code}", "OpenAI"), cancellationToken);

    public Task<AIResponse> RefactorCodeAsync(string code, CancellationToken cancellationToken = default)
        => ExecuteAsync(new AIRequest($"Refactor the following C# code for readability and performance:\n{code}", "Claude"), cancellationToken);

    public Task<AIResponse> GenerateDocumentationAsync(string code, CancellationToken cancellationToken = default)
        => ExecuteAsync(new AIRequest($"Generate documentation for this C# code:\n{code}", "OpenAI"), cancellationToken);
}
