using DeveloperGeniue.Core;
using DeveloperGeniue.Core.AI;
using System.Net;
using System.Net.Http;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DeveloperGeniue.Tests;

public class OpenAIClientTests
{
    private class Handler : HttpMessageHandler
    {
        public HttpRequestMessage? Request { get; private set; }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Request = request;
            var json = "{\"choices\":[{\"message\":{\"content\":\"ok\"}}]}";
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
        var config = new ConfigurationService(file, "pass");
        await config.SetSettingAsync("OpenAIApiKey", "abc");
        var handler = new Handler();
        var client = new OpenAIClient(config, new HttpClient(handler), "pass");

        var result = await client.GetCompletionAsync(new AIRequest("hi", "OpenAI"));

        var auth = handler.Request?.Headers.GetValues("Authorization").FirstOrDefault();
        Assert.Equal("Bearer abc", auth);
        Assert.Equal("ok", result.Content);
        File.Delete(file);
    }
}
