using DeveloperGeniue.Core;

namespace DeveloperGeniue.Tests;

public class ProjectManagerTests
{
    [Fact]
    public async Task LoadProjectReadsFramework()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        var csprojPath = Path.Combine(tempDir, "Test.csproj");
        var csprojContent = "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><TargetFramework>net8.0</TargetFramework></PropertyGroup></Project>";
        await File.WriteAllTextAsync(csprojPath, csprojContent);

        var pm = new ProjectManager();
        var project = await pm.LoadProjectAsync(csprojPath);

        Assert.Equal("Test", project.Name);
        Assert.Equal("net8.0", project.Framework);

        Directory.Delete(tempDir, true);
    }

    [Fact]
    public async Task GetProjectFilesIgnoresBinAndObj()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        var projectDir = Path.Combine(tempDir, "Project");
        Directory.CreateDirectory(Path.Combine(projectDir, "bin"));
        Directory.CreateDirectory(Path.Combine(projectDir, "obj"));

        var csprojPath = Path.Combine(projectDir, "Test.csproj");
        await File.WriteAllTextAsync(csprojPath, "<Project></Project>");
        await File.WriteAllTextAsync(Path.Combine(projectDir, "Program.cs"), "class P{}" );
        await File.WriteAllTextAsync(Path.Combine(projectDir, "bin", "Ignore.cs"), "class Bin{}" );

        var pm = new ProjectManager();
        var files = await pm.GetProjectFilesAsync(projectDir);

        Assert.DoesNotContain(files, f => f.Path.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}"));
        Assert.DoesNotContain(files, f => f.Path.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}"));

        Directory.Delete(tempDir, true);
    }
}
