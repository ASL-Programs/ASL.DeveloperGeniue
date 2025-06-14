using DeveloperGeniue.Core;

namespace DeveloperGeniue.Tests;

public class ProjectManagerAsyncTests
{
    [Fact]
    public async Task EnumerateProjectFilesAsyncMatchesSync()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        File.WriteAllText(Path.Combine(tempDir, "Test.csproj"), "<Project/>");
        var pm = new ProjectManager();
        var sync = pm.EnumerateProjectFiles(tempDir).ToList();
        var asyncResult = await pm.EnumerateProjectFilesAsync(tempDir, CancellationToken.None);
        Directory.Delete(tempDir, true);
        Assert.Equal(sync, asyncResult.ToList());
    }

    [Fact]
    public async Task GetProjectFilesAsyncReadsContent()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        var file = Path.Combine(tempDir, "Test.csproj");
        var content = "<Project/>";
        await File.WriteAllTextAsync(file, content);

        var pm = new ProjectManager();
        var result = await pm.GetProjectFilesAsync(tempDir, CancellationToken.None);

        Directory.Delete(tempDir, true);

        var codeFile = Assert.Single(result);
        Assert.Equal(file, codeFile.Path);
        Assert.Equal(content, codeFile.Content);
    }
}
