using DeveloperGeniue.Visualization;
using System.Diagnostics;
using Xunit;

namespace DeveloperGeniue.Tests;

public class ThreeJsCodeVisualizationServiceTests
{
    [Fact]
    public async Task RenderCreatesHtmlAndLaunchesProcess()
    {
        var projectDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(projectDir);
        await File.WriteAllTextAsync(Path.Combine(projectDir, "A.cs"), "class A{}\n");

        var interceptDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(interceptDir);
        var logFile = Path.Combine(interceptDir, "called.txt");
        var openScript = Path.Combine(interceptDir, "xdg-open");
        await File.WriteAllTextAsync(openScript, "#!/bin/sh\necho $@ > \"" + logFile + "\"\n");
        Process.Start("chmod", $"+x {openScript}").WaitForExit();
        var oldPath = Environment.GetEnvironmentVariable("PATH");
        Environment.SetEnvironmentVariable("PATH", interceptDir + Path.PathSeparator + oldPath);

        var tempHtml = Path.Combine(Path.GetTempPath(), "devgen_threejs.html");
        if (File.Exists(tempHtml)) File.Delete(tempHtml);

        try
        {
            var service = new ThreeJsCodeVisualizationService();
            await service.RenderAsync(projectDir);

            Assert.True(File.Exists(tempHtml));
            Assert.True(File.Exists(logFile));
            var launched = File.ReadAllText(logFile).Trim();
            Assert.Contains(tempHtml, launched);
        }
        finally
        {
            Environment.SetEnvironmentVariable("PATH", oldPath);
            Directory.Delete(interceptDir, true);
            Directory.Delete(projectDir, true);
            if (File.Exists(tempHtml)) File.Delete(tempHtml);
        }
    }
}
