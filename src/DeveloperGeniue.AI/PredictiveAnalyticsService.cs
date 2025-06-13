using DeveloperGeniue.Core;
using System.Linq;
using System.Collections.Generic;

namespace DeveloperGeniue.AI;

/// <summary>
/// Collects project metrics and generates simple forecasts.
/// </summary>
public class PredictiveAnalyticsService
{
    /// <summary>
    /// Gathers statistics from the provided project and optional results to return a simple prediction summary.
    /// </summary>
    public PredictiveAnalyticsReport Analyze(Project project, BuildResult? build = null, TestResult? tests = null, IEnumerable<int>? historicalFileCounts = null)
    {
        int totalFiles = project.Files.Count;
        double avgLines = totalFiles == 0
            ? 0
            : project.Files.Average(f => string.IsNullOrEmpty(f.Content) ? 0 : f.Content.Split('\n').Length);

        bool buildSucceeded = build?.Success ?? false;
        double passRate = tests == null || tests.TotalTests == 0
            ? 0
            : tests.PassedTests / (double)tests.TotalTests;

        var summary = $"Files: {totalFiles}, Avg lines/file: {avgLines:F1}";
        if (build != null)
        {
            summary += build.Success ? ", build succeeded" : ", build failed";
        }
        if (tests != null)
        {
            summary += $", tests pass rate {passRate:P0}";
        }

        string trend = "stable";
        if (historicalFileCounts != null && historicalFileCounts.Any())
        {
            double avgHistory = historicalFileCounts.Average();
            trend = totalFiles > avgHistory
                ? "growing"
                : totalFiles < avgHistory
                    ? "shrinking"
                    : "stable";
        }

        return new PredictiveAnalyticsReport(totalFiles, avgLines, buildSucceeded, passRate, summary, trend);
    }
}
