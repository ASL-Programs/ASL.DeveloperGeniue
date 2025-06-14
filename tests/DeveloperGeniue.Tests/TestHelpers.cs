using System.Runtime.InteropServices;
using System.Diagnostics;

namespace DeveloperGeniue.Tests;

internal static class TestHelpers
{
    public static (string TempDir, string OldPath) CreateFakeDotnet(params string[] scriptLines)
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        var fileName = isWindows ? "dotnet.cmd" : "dotnet";
        var newline = isWindows ? "\r\n" : "\n";
        var script = string.Join(newline, scriptLines) + newline;
        if (!isWindows)
        {
            script = "#!/bin/sh" + newline + script;
        }
        var path = Path.Combine(tempDir, fileName);
        File.WriteAllText(path, script);
        if (!isWindows)
        {
            Process.Start("chmod", $"+x {path}").WaitForExit();
        }
        var oldPath = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
        Environment.SetEnvironmentVariable("PATH", tempDir + Path.PathSeparator + oldPath);
        return (tempDir, oldPath);
    }

    public static void CleanupFakeDotnet(string tempDir, string oldPath)
    {
        Environment.SetEnvironmentVariable("PATH", oldPath);
        if (Directory.Exists(tempDir))
            Directory.Delete(tempDir, true);
    }
}
