using DeveloperGeniue.Core;
using System.Runtime.InteropServices;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Microsoft.Extensions.Logging;

namespace DeveloperGeniue.Tests;

public class LoggingTests
{
    [Fact]
    public async Task BuildManagerLogsMessages()
    {
        var sink = new InMemorySink();
        var logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Sink(sink)
            .CreateLogger();
        var factory = new SerilogLoggerFactory(logger, dispose: true);
        var bm = new BuildManager(factory.CreateLogger<BuildManager>());

        var isWin = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        var lines = isWin
            ? new[] { "@echo off", "echo building %*" }
            : new[] { "echo building $@" };
        var (tempDir, oldPath) = TestHelpers.CreateFakeDotnet(lines);

        await bm.BuildProjectAsync("proj.csproj");

        TestHelpers.CleanupFakeDotnet(tempDir, oldPath);

        Assert.Contains(sink.Events, e => e.Level == LogEventLevel.Information);
    }
}
