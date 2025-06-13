using DeveloperGeniue.AI;
using DeveloperGeniue.Core;
using Xunit;

namespace DeveloperGeniue.Tests;

public class PredictiveAnalyticsServiceTests
{
    [Fact]
    public void AnalyzeReturnsDetailedReport()
    {
        var project = new Project
        {
            Files = new()
            {
                new CodeFile { Content = "line1\nline2" }
            }
        };
        var build = new BuildResult { Success = true };
        var tests = new TestResult { PassedTests = 2, FailedTests = 1, TotalTests = 3 };

        var service = new PredictiveAnalyticsService();
        var report = service.Analyze(project, build, tests);

        Assert.Equal(1, report.TotalFiles);
        Assert.True(report.BuildSucceeded);
        Assert.InRange(report.TestPassRate, 0.66, 0.67);
        Assert.Contains("build succeeded", report.Summary);
    }
}
