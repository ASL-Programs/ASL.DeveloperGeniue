using DeveloperGeniue.Core;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;

namespace DeveloperGeniue.Tests;

public class DatabaseConfigurationServiceTests
{
    [Fact]
    public async Task SetAndGetSetting_PersistsValue()
    {
        var dbPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        var sink = new InMemorySink();
        var logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Sink(sink)
            .CreateLogger();
        var factory = new SerilogLoggerFactory(logger, dispose: true);
        var service = new DatabaseConfigurationService(factory.CreateLogger<DatabaseConfigurationService>(), dbPath);

        await service.SetSettingAsync("TestKey", "TestValue");

        service = new DatabaseConfigurationService(factory.CreateLogger<DatabaseConfigurationService>(), dbPath);
        var result = await service.GetSettingAsync<string>("TestKey");

        Assert.Equal("TestValue", result);
        Assert.Contains(sink.Events, e => e.Level == LogEventLevel.Information);
        File.Delete(dbPath);
    }
}
