using DeveloperGeniue.Core;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;

namespace DeveloperGeniue.Tests;

public class ConfigurationServiceTests
{
    [Fact]
    public async Task SetAndGetSetting()
    {
        var tempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        var sink = new InMemorySink();
        var logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Sink(sink)
            .CreateLogger();
        var factory = new SerilogLoggerFactory(logger, dispose: true);
        var service = new ConfigurationService(factory.CreateLogger<ConfigurationService>(), tempFile);

        await service.SetSettingAsync("TestKey", "Value");
        var result = await service.GetSettingAsync<string>("TestKey");

        Assert.Equal("Value", result);
        Assert.Contains(sink.Events, e => e.Level == LogEventLevel.Information);
        File.Delete(tempFile);
    }
}
