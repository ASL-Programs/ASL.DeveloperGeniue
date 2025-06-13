using DeveloperGeniue.Core;

namespace DeveloperGeniue.Tests;

public class TestManagerTests
{
    [Fact]
    public async Task TestResultsAreParsed()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        var fake = Path.Combine(tempDir, "dotnet");
        var script = "#!/bin/sh\necho 'Passed!  - Failed: 0, Passed: 1, Skipped: 0, Total: 1, Duration: 1 ms'\necho 'Test Run Successful.'\necho 'Test execution time: 0.1'\n";
        await File.WriteAllTextAsync(fake, script);
        System.Diagnostics.Process.Start("chmod", $"+x {fake}").WaitForExit();
        var oldPath = Environment.GetEnvironmentVariable("PATH");
        Environment.SetEnvironmentVariable("PATH", tempDir + Path.PathSeparator + oldPath);

        var tm = new TestManager();
        var result = await tm.RunTestsAsync("proj.csproj");

        Environment.SetEnvironmentVariable("PATH", oldPath);
        Directory.Delete(tempDir, true);

        Assert.True(result.Success);
        Assert.Equal(1, result.TotalTests);
        Assert.Equal(1, result.PassedTests);
    }
}
