using DeveloperGeniue.Core;

namespace DeveloperGeniue.Tests;

public class BuildManagerTests
{
    [Fact]
    public async Task BuildInvokesDotnet()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        var fake = Path.Combine(tempDir, "dotnet");
        await File.WriteAllTextAsync(fake, "#!/bin/sh\necho building $@\n");
        System.Diagnostics.Process.Start("chmod", $"+x {fake}").WaitForExit();
        var oldPath = Environment.GetEnvironmentVariable("PATH");
        Environment.SetEnvironmentVariable("PATH", tempDir + Path.PathSeparator + oldPath);

        var bm = new BuildManager();
        var result = await bm.BuildProjectAsync("proj.csproj");

        Environment.SetEnvironmentVariable("PATH", oldPath);
        Directory.Delete(tempDir, true);

        Assert.True(result.Success);
        Assert.Contains("build", result.Output);
    }
}
