using Serilog.Core;
using Serilog.Events;
using System.Collections.Concurrent;

namespace DeveloperGeniue.Tests;

public class InMemorySink : ILogEventSink
{
    private readonly ConcurrentBag<LogEvent> _events = new();

    public void Emit(LogEvent logEvent)
    {
        _events.Add(logEvent);
    }

    public IReadOnlyCollection<LogEvent> Events => _events;
}
