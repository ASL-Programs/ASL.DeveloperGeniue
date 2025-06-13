namespace DeveloperGeniue.Core;

public interface IProjectManager
{
    Task<Project> LoadProjectAsync(string projectPath);
    Task<Project> LoadProjectAsync(string projectPath, CancellationToken cancellationToken);
    Task<IEnumerable<CodeFile>> GetProjectFilesAsync(string projectPath);
    Task<IEnumerable<CodeFile>> GetProjectFilesAsync(string projectPath, CancellationToken cancellationToken);
    Task<IEnumerable<string>> EnumerateProjectFilesAsync(string rootPath, CancellationToken cancellationToken);
}
