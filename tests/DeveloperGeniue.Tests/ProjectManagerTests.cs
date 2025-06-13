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

    [Fact]
    public void EnumerateProjectFilesSkipsIgnoredDirectories()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        File.WriteAllText(Path.Combine(tempDir, "Root.csproj"), "<Project/>");

        Directory.CreateDirectory(Path.Combine(tempDir, "bin"));
        File.WriteAllText(Path.Combine(tempDir, "bin", "ShouldIgnore.csproj"), "<Project/>");
        Directory.CreateDirectory(Path.Combine(tempDir, "obj"));
        File.WriteAllText(Path.Combine(tempDir, "obj", "Ignore2.csproj"), "<Project/>");
        Directory.CreateDirectory(Path.Combine(tempDir, ".git"));
        File.WriteAllText(Path.Combine(tempDir, ".git", "Ignore3.csproj"), "<Project/>");

        Directory.CreateDirectory(Path.Combine(tempDir, "sub"));
        File.WriteAllText(Path.Combine(tempDir, "sub", "Nested.csproj"), "<Project/>");

        var pm = new ProjectManager();
        var found = pm.EnumerateProjectFiles(tempDir).ToList();

        Directory.Delete(tempDir, true);

        Assert.Contains(Path.Combine(tempDir, "Root.csproj"), found);
        Assert.Contains(Path.Combine(tempDir, "sub", "Nested.csproj"), found);
        Assert.DoesNotContain(Path.Combine(tempDir, "bin", "ShouldIgnore.csproj"), found);
        Assert.DoesNotContain(Path.Combine(tempDir, "obj", "Ignore2.csproj"), found);
        Assert.DoesNotContain(Path.Combine(tempDir, ".git", "Ignore3.csproj"), found);
    }
}
