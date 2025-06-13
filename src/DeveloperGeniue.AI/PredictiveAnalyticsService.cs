using DeveloperGeniue.Core;

namespace DeveloperGeniue.AI;

/// <summary>
/// Collects project metrics and generates simple forecasts.
/// </summary>
public class PredictiveAnalyticsService
{
    /// <summary>
    /// Gathers statistics from the provided project and optional results to return a simple prediction summary.
    /// </summary>
    public string Analyze(Project project, BuildResult? build = null, TestResult? tests = null)
    {
        var score = project.Files.Count;
        if (build?.Success == true)
        {
            score += 5;
        }
        if (tests != null)
        {
            score += tests.PassedTests - tests.FailedTests;
        }
        return $"Forecast score: {score}";
    }
}
