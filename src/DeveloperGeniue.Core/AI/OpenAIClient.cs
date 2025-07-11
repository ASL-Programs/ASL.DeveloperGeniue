using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace DeveloperGeniue.Core.AI;

public class OpenAIClient : IAIClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfigurationService _config;
    private readonly string? _passphrase;

    public OpenAIClient(IConfigurationService config, HttpClient? httpClient = null, string? passphrase = null)
    {
        _config = config;
        _httpClient = httpClient ?? new HttpClient();
        _passphrase = passphrase;
    }

    public async Task<AIResponse> GetCompletionAsync(AIRequest request, CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            model = "gpt-4",
            messages = new[] { new { role = "user", content = request.Prompt } }
        };

        using var msg = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions")
        {
            Content = JsonContent.Create(payload)
        };

        var rawKey = await _config.GetSettingAsync<string>("OpenAIApiKey") ?? string.Empty;
        var apiKey = CryptoHelper.Decrypt(rawKey, _passphrase);
        msg.Headers.Add("Authorization", $"Bearer {apiKey}");

        var resp = await _httpClient.SendAsync(msg, cancellationToken);
        if (!resp.IsSuccessStatusCode)
        {
            return new AIResponse(string.Empty);
        }

        var json = await resp.Content.ReadAsStringAsync(cancellationToken);
        using var doc = JsonDocument.Parse(json);
        var content = doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
        return new AIResponse(content ?? string.Empty);
    }
}
