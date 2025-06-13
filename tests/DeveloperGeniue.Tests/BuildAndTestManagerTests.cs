using DeveloperGeniue.Core;

namespace DeveloperGeniue.Tests;

public class BuildAndTestManagerTests
{
    [Fact]
    public async Task BuildSolutionSucceeds()
    {
        var root = GetRepoRoot();
        var sln = Path.Combine(root, "DeveloperGeniue.sln");
        var manager = new BuildManager();
        var result = await manager.BuildProjectAsync(sln);
        Assert.True(result.Success, result.Output + result.Errors);
    }

    [Fact]
    public async Task RunTempTests()
    {
        var temp = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(temp);
        var csproj = Path.Combine(temp, "DemoTests.csproj");
        var csprojContent = "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><TargetFramework>net8.0</TargetFramework><IsTestProject>true</IsTestProject></PropertyGroup><ItemGroup><PackageReference Include=\"Microsoft.NET.Test.Sdk\" Version=\"17.6.0\" /><PackageReference Include=\"xunit\" Version=\"2.4.2\" /><PackageReference Include=\"xunit.runner.visualstudio\" Version=\"2.4.5\" /></ItemGroup></Project>";
        await File.WriteAllTextAsync(csproj, csprojContent);
        await File.WriteAllTextAsync(Path.Combine(temp, "UnitTest1.cs"), "public class UnitTest1 { [Xunit.Fact] public void T() { Xunit.Assert.True(true); } }");

        var manager = new TestManager();
        var result = await manager.RunTestsAsync(csproj);
        Directory.Delete(temp, true);

        Assert.True(result.Success);
        Assert.True(result.TotalTests > 0);
    }

    private static string GetRepoRoot()
    {
        var dir = AppContext.BaseDirectory;
        for (int i = 0; i < 5; i++)
        {
            dir = Directory.GetParent(dir)!.FullName;
        }
        return dir;
    }
}
