using DeveloperGeniue.Core;
using System.Runtime.InteropServices;

namespace DeveloperGeniue.Tests;

public class TestManagerTests
{
    [Fact]
    public async Task TestResultsAreParsed()
    {
        var isWin = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        string[] lines;
        if (isWin)
        {
            lines = new[]
            {
                "@echo off",
                "echo Passed!  - Failed: 0, Passed: 1, Skipped: 0, Total: 1, Duration: 1 ms",
                "echo Test Run Successful.",
                "echo Test execution time: 0.1"
            };
        }
        else
        {
            lines = new[]
            {
                "echo 'Passed!  - Failed: 0, Passed: 1, Skipped: 0, Total: 1, Duration: 1 ms'",
                "echo 'Test Run Successful.'",
                "echo 'Test execution time: 0.1'"
            };
        }
        var (tempDir, oldPath) = TestHelpers.CreateFakeDotnet(lines);

        var tm = new TestManager();
        var result = await tm.RunTestsAsync("proj.csproj");

        TestHelpers.CleanupFakeDotnet(tempDir, oldPath);

        Assert.True(result.Success);
        Assert.Equal(1, result.TotalTests);
        Assert.Equal(1, result.PassedTests);
    }
}
