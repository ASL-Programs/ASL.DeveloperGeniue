using System.Diagnostics;

namespace DeveloperGeniue.Visualization;

/// <summary>
/// Minimal integration with an external AR viewer.  The service expects the
/// environment variable <c>UNITY_AR_EXEC</c> to point to a Unity executable that
/// launches an AR Foundation based project capable of loading the specified
/// code base.
/// </summary>
public class AugmentedRealityService : IAugmentedRealityService
{
    public Task StartCodeReviewAsync(string projectPath)
    {
        var unityPath = Environment.GetEnvironmentVariable("UNITY_AR_EXEC");
        if (string.IsNullOrWhiteSpace(unityPath))
            throw new InvalidOperationException("UNITY_AR_EXEC is not configured.");

        var psi = new ProcessStartInfo
        {
            FileName = unityPath,
            Arguments = $"--project-path \"{projectPath}\"",
            UseShellExecute = true
        };
        Process.Start(psi);
        return Task.CompletedTask;
    }
}
