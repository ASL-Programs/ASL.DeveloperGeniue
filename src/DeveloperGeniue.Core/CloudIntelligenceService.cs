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
        // Example integration point for cloud AI providers (Azure or AWS)
        var endpoint = await _config.GetSettingAsync<string>("CloudAIEndpoint") ?? string.Empty;
        if (string.IsNullOrEmpty(endpoint))
            return string.Empty;

        var req = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = new StringContent(prompt, Encoding.UTF8, "text/plain")
        };
        var resp = await _http.SendAsync(req, cancellationToken);
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadAsStringAsync(cancellationToken);
    }
}
