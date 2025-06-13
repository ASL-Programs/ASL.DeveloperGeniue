using DeveloperGeniue.Core;

namespace DeveloperGeniue.Tests;

public class EvolutionAnalyticsServiceTests
{
    [Fact]
    public async Task ComputesBasicMetrics()
    {
        var temp = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        var tracker = new EvolutionTracker(temp);
        await tracker.RecordAsync(new EvolutionRecord("h1", DateTime.UtcNow.AddDays(-2), "msg", "alice"));
        await tracker.RecordAsync(new EvolutionRecord("h2", DateTime.UtcNow, "msg", "bob"));

        var service = new EvolutionAnalyticsService();
        var analytics = await service.ComputeAsync(tracker);

        Assert.Equal(2, analytics.CommitCount);
        Assert.True(analytics.CommitsPerDay > 0);
        Assert.Equal(1, analytics.CommitsPerAuthor["alice"]);
        Assert.Equal(1, analytics.CommitsPerAuthor["bob"]);
        File.Delete(temp);
    }
}
