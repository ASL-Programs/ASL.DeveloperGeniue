using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace DeveloperGeniue.Core.AI;

public class ClaudeAIClient : IAIClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public ClaudeAIClient(HttpClient? httpClient = null, string? apiKey = null)
    {
        _httpClient = httpClient ?? new HttpClient();
        _apiKey = apiKey ?? Environment.GetEnvironmentVariable("CLAUDE_API_KEY") ?? string.Empty;
    }

    public async Task<AIResponse> GetCompletionAsync(AIRequest request, CancellationToken cancellationToken = default)
    {
        var payload = new { prompt = request.Prompt, model = "claude-3" };
        using var msg = new HttpRequestMessage(HttpMethod.Post, "https://api.anthropic.com/v1/complete")
        {
            Content = JsonContent.Create(payload)
        };
        msg.Headers.Add("x-api-key", _apiKey);
        var resp = await _httpClient.SendAsync(msg, cancellationToken);
        if (!resp.IsSuccessStatusCode)
        {
            return new AIResponse(string.Empty);
        }
        var json = await resp.Content.ReadAsStringAsync(cancellationToken);
        using var doc = JsonDocument.Parse(json);
        var content = doc.RootElement.GetProperty("completion").GetString();
        return new AIResponse(content ?? string.Empty);
    }
}
