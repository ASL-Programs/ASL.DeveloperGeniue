using DeveloperGeniue.Core;
using DeveloperGeniue.Core.AI;
using System.Net;
using System.Net.Http;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DeveloperGeniue.Tests;

public class ClaudeAIClientTests
{
    private class Handler : HttpMessageHandler
    {
        public HttpRequestMessage? Request { get; private set; }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Request = request;
            var json = "{\"completion\":\"ok\"}";
            var resp = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json)
            };
            return Task.FromResult(resp);
        }
    }

    [Fact]
    public async Task UsesApiKeyFromConfiguration()
    {
        var file = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        var config = new ConfigurationService(file);
        await config.SetSettingAsync("ClaudeApiKey", "abc");
        var handler = new Handler();
        var client = new ClaudeAIClient(config, new HttpClient(handler));

        var result = await client.GetCompletionAsync(new AIRequest("hi", "Claude"));

        var key = handler.Request?.Headers.GetValues("x-api-key").FirstOrDefault();
        Assert.Equal("abc", key);
        Assert.Equal("ok", result.Content);
        File.Delete(file);
    }
}
