using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;

namespace DeveloperGeniue.Core;

public static class LoggingConfiguration
{
    public static ILoggerFactory CreateLoggerFactory(string? logfile = null)
    {
        var config = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .WriteTo.Console();
        if (!string.IsNullOrWhiteSpace(logfile))
        {
            config = config.WriteTo.File(logfile);
        }

        Log.Logger = config.CreateLogger();
        return new SerilogLoggerFactory(Log.Logger, dispose: true);
    }
}
