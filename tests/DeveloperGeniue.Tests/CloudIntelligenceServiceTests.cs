using DeveloperGeniue.Core;
using Xunit;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DeveloperGeniue.Tests;

public class CloudIntelligenceServiceTests
{
    private class Handler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("ok")
            };
            return Task.FromResult(response);
        }
    }

    [Fact]
    public async Task AnalyzeCodeCallsEndpoint()
    {
        var tempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        var config = new ConfigurationService(tempFile);
        await config.SetSettingAsync("CloudAIProvider", "azure");
        await config.SetSettingAsync("AzureAIEndpoint", "http://localhost/");
        var service = new CloudIntelligenceService(config, new HttpClient(new Handler()));
        var result = await service.AnalyzeCodeAsync("class C{}");
        Assert.Equal("ok", result);
        File.Delete(tempFile);
    }

    [Fact]
    public async Task TrainModelCallsEndpoint()
    {
        var tempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        var config = new ConfigurationService(tempFile);
        await config.SetSettingAsync("CloudAIProvider", "aws");
        await config.SetSettingAsync("AWSAIEndpoint", "http://localhost/");
        var service = new CloudIntelligenceService(config, new HttpClient(new Handler()));
        var result = await service.TrainModelAsync("data");
        Assert.Equal("ok", result);
        File.Delete(tempFile);
    }
}

