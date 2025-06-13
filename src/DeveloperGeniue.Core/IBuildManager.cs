namespace DeveloperGeniue.Core;

public interface IBuildManager
{
    Task<BuildResult> BuildProjectAsync(string projectPath);
    Task<BuildResult> BuildProjectAsync(string projectPath, CancellationToken cancellationToken);
}
