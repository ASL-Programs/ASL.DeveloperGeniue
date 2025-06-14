using DeveloperGeniue.Core;
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

        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        var fake = Path.Combine(tempDir, "dotnet");
        await File.WriteAllTextAsync(fake, "#!/bin/sh\necho building $@\n");
        System.Diagnostics.Process.Start("chmod", $"+x {fake}").WaitForExit();
        var oldPath = Environment.GetEnvironmentVariable("PATH");
        Environment.SetEnvironmentVariable("PATH", tempDir + Path.PathSeparator + oldPath);

        await bm.BuildProjectAsync("proj.csproj");

        Environment.SetEnvironmentVariable("PATH", oldPath);
        Directory.Delete(tempDir, true);

        Assert.Contains(sink.Events, e => e.Level == LogEventLevel.Information);
    }
}
