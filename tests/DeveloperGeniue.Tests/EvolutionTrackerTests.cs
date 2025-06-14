using DeveloperGeniue.Core;

namespace DeveloperGeniue.Tests;

public class EvolutionTrackerTests
{
    [Fact]
    public async Task RecordsArePersisted()
    {
        var temp = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        var tracker = new EvolutionTracker(temp);
        var record = new EvolutionRecord("abc123", DateTime.UtcNow, "initial commit", "dev");
        await tracker.RecordAsync(record);

        var records = await tracker.GetRecordsAsync();
        Assert.Single(records);
        Assert.Equal("abc123", records[0].CommitHash);
        File.Delete(temp);
    }
}
