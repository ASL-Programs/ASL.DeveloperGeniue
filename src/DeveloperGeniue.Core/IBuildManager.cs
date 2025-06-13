namespace DeveloperGeniue.Core;

public interface IBuildManager
{
    Task<BuildResult> BuildProjectAsync(string projectPath);
}
