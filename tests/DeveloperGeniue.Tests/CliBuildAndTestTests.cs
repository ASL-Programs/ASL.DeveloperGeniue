using System.Diagnostics;

namespace DeveloperGeniue.Tests;

public class CliBuildAndTestTests
{
    [Fact]
    public async Task BuildCommandBuildsProject()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        var csproj = Path.Combine(tempDir, "Demo.csproj");
        var csprojContent = "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net8.0</TargetFramework></PropertyGroup></Project>";
        await File.WriteAllTextAsync(csproj, csprojContent);
        await File.WriteAllTextAsync(Path.Combine(tempDir, "Program.cs"), "Console.WriteLine(1);");

        var repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../"));
        var cliProj = Path.Combine(repoRoot, "src", "DeveloperGeniue.CLI", "DeveloperGeniue.CLI.csproj");

        var psi = new ProcessStartInfo("dotnet", $"run --project \"{cliProj}\" -- build \"{csproj}\"")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            WorkingDirectory = repoRoot
        };

        using var proc = Process.Start(psi)!;
        string output = await proc.StandardOutput.ReadToEndAsync();
        string errors = await proc.StandardError.ReadToEndAsync();
        await proc.WaitForExitAsync();

        Directory.Delete(tempDir, true);

        Assert.Equal(0, proc.ExitCode);
        Assert.Contains("Build succeeded", output + errors, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task TestCommandRunsTests()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        var csproj = Path.Combine(tempDir, "DemoTests.csproj");
        var csprojContent = "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><TargetFramework>net8.0</TargetFramework><IsTestProject>true</IsTestProject></PropertyGroup><ItemGroup><PackageReference Include=\"Microsoft.NET.Test.Sdk\" Version=\"17.6.0\" /><PackageReference Include=\"xunit\" Version=\"2.4.2\" /><PackageReference Include=\"xunit.runner.visualstudio\" Version=\"2.4.5\" /></ItemGroup></Project>";
        await File.WriteAllTextAsync(csproj, csprojContent);
        await File.WriteAllTextAsync(Path.Combine(tempDir, "UnitTest1.cs"), "public class UnitTest1 { [Xunit.Fact] public void T() { Xunit.Assert.True(true); } }");

        var repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../"));
        var cliProj = Path.Combine(repoRoot, "src", "DeveloperGeniue.CLI", "DeveloperGeniue.CLI.csproj");

        var psi = new ProcessStartInfo("dotnet", $"run --project \"{cliProj}\" -- test \"{csproj}\"")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            WorkingDirectory = repoRoot
        };

        using var proc = Process.Start(psi)!;
        string output = await proc.StandardOutput.ReadToEndAsync();
        string errors = await proc.StandardError.ReadToEndAsync();
        await proc.WaitForExitAsync();

        Directory.Delete(tempDir, true);

        Assert.Equal(0, proc.ExitCode);
        Assert.Contains("Tests Passed: 1", output);
        Assert.Contains("Tests Failed: 0", output);
    }
}
