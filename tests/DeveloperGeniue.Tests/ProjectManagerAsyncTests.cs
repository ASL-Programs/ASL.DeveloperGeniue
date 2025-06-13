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
}
