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
        Assert.Single(project.Files);
        Assert.Equal(csprojPath, project.Files[0].Path);
        Assert.Equal(csprojContent, project.Files[0].Content);

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
