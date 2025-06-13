using System.Collections.Concurrent;

namespace DeveloperGeniue.Core.AI;

public class AIOrchestrator
{
    private readonly ConcurrentDictionary<string, IAIClient> _providers = new();

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
}
