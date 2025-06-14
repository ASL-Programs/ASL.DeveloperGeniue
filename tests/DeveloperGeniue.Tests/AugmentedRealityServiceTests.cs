using DeveloperGeniue.Visualization;
using System.Diagnostics;
using Xunit;

namespace DeveloperGeniue.Tests;

public class AugmentedRealityServiceTests
{
    [Fact]
    public async Task LaunchesUnityExecutable()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempDir);
        var script = Path.Combine(tempDir, "unity.sh");
        var log = Path.Combine(tempDir, "args.txt");
        await File.WriteAllTextAsync(script, "#!/bin/sh\necho $@ > \"" + log + "\"\n");
        Process.Start("chmod", $"+x {script}").WaitForExit();
        Environment.SetEnvironmentVariable("UNITY_AR_EXEC", script);

        var service = new AugmentedRealityService();
        var project = Path.Combine(Path.GetTempPath(), "proj");
        await service.StartCodeReviewAsync(project);

        Assert.True(File.Exists(log));
        var args = File.ReadAllText(log).Trim();
        Assert.Contains(project, args);

        Environment.SetEnvironmentVariable("UNITY_AR_EXEC", null);
        Directory.Delete(tempDir, true);
    }
}
