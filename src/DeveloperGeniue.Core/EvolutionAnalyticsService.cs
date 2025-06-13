using System;
using System.Linq;
namespace DeveloperGeniue.Core;

/// <summary>
/// Generates analytics data from an <see cref="IEvolutionTracker"/> instance.
/// </summary>
public class EvolutionAnalyticsService
{
    public async Task<EvolutionAnalytics> ComputeAsync(IEvolutionTracker tracker)
    {
        var records = await tracker.GetRecordsAsync();
        if (records.Count == 0)
        {
            return new EvolutionAnalytics(0, DateTime.MinValue, DateTime.MinValue, 0);
        }

        var ordered = records.OrderBy(r => r.Timestamp).ToList();
        var first = ordered.First().Timestamp;
        var last = ordered.Last().Timestamp;
        var commitCount = ordered.Count;
        var totalDays = (last - first).TotalDays;
        double commitsPerDay = totalDays <= 0 ? commitCount : commitCount / totalDays;
        return new EvolutionAnalytics(commitCount, first, last, commitsPerDay);
    }
}
