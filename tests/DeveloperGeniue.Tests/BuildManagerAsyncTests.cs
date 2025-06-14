using DeveloperGeniue.Core;
using System.Runtime.InteropServices;

namespace DeveloperGeniue.Tests;

public class BuildManagerAsyncTests
{
    [Fact]
    public async Task BuildWithCancellationTokenRuns()
    {
        var isWin = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        var lines = isWin
            ? new[] { "@echo off", "echo building %*" }
            : new[] { "echo building $@" };
        var (tempDir, oldPath) = TestHelpers.CreateFakeDotnet(lines);
        var bm = new BuildManager();
        var result = await bm.BuildProjectAsync("proj.csproj", CancellationToken.None);
        TestHelpers.CleanupFakeDotnet(tempDir, oldPath);
        Assert.True(result.Success);
    }
}
