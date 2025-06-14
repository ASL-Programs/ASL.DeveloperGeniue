using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace DeveloperGeniue.Core.AI;

public class ClaudeAIClient : IAIClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfigurationService _config;
    private readonly string? _passphrase;

    public ClaudeAIClient(IConfigurationService config, HttpClient? httpClient = null, string? passphrase = null)
    {
        _config = config;
        _httpClient = httpClient ?? new HttpClient();
        _passphrase = passphrase;
    }

    public async Task<AIResponse> GetCompletionAsync(AIRequest request, CancellationToken cancellationToken = default)
    {
        var payload = new { prompt = request.Prompt, model = "claude-3" };
        using var msg = new HttpRequestMessage(HttpMethod.Post, "https://api.anthropic.com/v1/complete")
        {
            Content = JsonContent.Create(payload)
        };

        var rawKey = await _config.GetSettingAsync<string>("ClaudeApiKey") ?? string.Empty;
        var apiKey = CryptoHelper.Decrypt(rawKey, _passphrase);
        msg.Headers.Add("x-api-key", apiKey);

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
