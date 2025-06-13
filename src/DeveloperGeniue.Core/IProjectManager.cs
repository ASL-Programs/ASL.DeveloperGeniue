namespace DeveloperGeniue.Core;

public interface IProjectManager
{
    Task<Project> LoadProjectAsync(string projectPath);
    Task<IEnumerable<CodeFile>> GetProjectFilesAsync(string projectPath);

    Task ScanProjectFilesAsync(Project project);
    Task AnalyzeDependenciesAsync(Project project);
}
