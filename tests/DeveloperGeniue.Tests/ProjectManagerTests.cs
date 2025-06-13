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
    public async Task LoadProjectScansFiles()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        var csprojPath = Path.Combine(tempDir, "Test.csproj");
        var csprojContent = "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><TargetFramework>net8.0</TargetFramework></PropertyGroup></Project>";
        await File.WriteAllTextAsync(csprojPath, csprojContent);
        var csFile = Path.Combine(tempDir, "Class1.cs");
        await File.WriteAllTextAsync(csFile, "class C {}\n");

        var pm = new ProjectManager();
        var project = await pm.LoadProjectAsync(csprojPath);

        Assert.Contains(project.Files, f => f.Path == csFile);

        Directory.Delete(tempDir, true);
    }

    [Fact]
    public async Task LoadProjectAnalyzesDependencies()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        var csprojPath = Path.Combine(tempDir, "Test.csproj");
        var csprojContent = @"<Project Sdk=""Microsoft.NET.Sdk""><ItemGroup><PackageReference Include=""Newtonsoft.Json"" Version=""13.0.1"" /></ItemGroup><PropertyGroup><TargetFramework>net8.0</TargetFramework></PropertyGroup></Project>";
        await File.WriteAllTextAsync(csprojPath, csprojContent);

        var pm = new ProjectManager();
        var project = await pm.LoadProjectAsync(csprojPath);

        Assert.Contains("Newtonsoft.Json", project.Dependencies);
        Directory.Delete(tempDir, true);
    }
}
