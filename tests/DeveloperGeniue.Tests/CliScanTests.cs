using System.Diagnostics;
using DeveloperGeniue.Core;

namespace DeveloperGeniue.Tests;

public class CliScanTests
{
    [Fact]
    public async Task ScanExcludesBinAndObjDirectories()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        var csprojContent = "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><TargetFramework>net8.0</TargetFramework></PropertyGroup></Project>";
        await File.WriteAllTextAsync(Path.Combine(tempDir, "Valid.csproj"), csprojContent);
        Directory.CreateDirectory(Path.Combine(tempDir, "bin"));
        await File.WriteAllTextAsync(Path.Combine(tempDir, "bin", "Skip.csproj"), csprojContent);
        Directory.CreateDirectory(Path.Combine(tempDir, "obj"));
        await File.WriteAllTextAsync(Path.Combine(tempDir, "obj", "Skip2.csproj"), csprojContent);

        var repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../"));
        var cliProj = Path.Combine(repoRoot, "src", "DeveloperGeniue.CLI", "DeveloperGeniue.CLI.csproj");

        var psi = new ProcessStartInfo("dotnet", $"run --project \"{cliProj}\" -- scan \"{tempDir}\"")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            WorkingDirectory = repoRoot
        };

        using var proc = Process.Start(psi)!;
        string output = await proc.StandardOutput.ReadToEndAsync();
        await proc.WaitForExitAsync();

        Directory.Delete(tempDir, true);

        Assert.Contains("Valid", output);
        Assert.DoesNotContain("Skip", output);
    }
}
