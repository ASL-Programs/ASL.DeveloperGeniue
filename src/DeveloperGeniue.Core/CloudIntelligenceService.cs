using System.Net.Http;
using System.Text;

namespace DeveloperGeniue.Core;

public class CloudIntelligenceService : ICloudIntelligenceService
{
    private readonly HttpClient _http = new();
    private readonly IConfigurationService _config;

    public CloudIntelligenceService(IConfigurationService config)
    {
        _config = config;
    }

    public async Task<string> ExecuteAsync(string prompt, CancellationToken cancellationToken = default)
    {
        // Resolve provider and endpoint from configuration
        var provider = (await _config.GetSettingAsync<string>("CloudAIProvider"))?.ToLowerInvariant();
        string? endpoint = null;
        string? apiKey = null;

        if (provider == "azure")
        {
            endpoint = await _config.GetSettingAsync<string>("AzureAIEndpoint");
            apiKey = await _config.GetSettingAsync<string>("AzureAIKey");
        }
        else if (provider == "aws")
        {
            endpoint = await _config.GetSettingAsync<string>("AWSAIEndpoint");
            apiKey = await _config.GetSettingAsync<string>("AWSAIKey");
        }
        else
        {
            endpoint = await _config.GetSettingAsync<string>("CloudAIEndpoint");
        }

        if (string.IsNullOrEmpty(endpoint))
            return string.Empty;

        var req = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = new StringContent(prompt, Encoding.UTF8, "text/plain")
        };
        if (!string.IsNullOrEmpty(apiKey))
            req.Headers.Add("Authorization", $"Bearer {apiKey}");

        var resp = await _http.SendAsync(req, cancellationToken);
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadAsStringAsync(cancellationToken);
    }
}
