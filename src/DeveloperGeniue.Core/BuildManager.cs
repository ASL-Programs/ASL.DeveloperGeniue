using System.Diagnostics;
using System.Text;

namespace DeveloperGeniue.Core;

public class BuildManager : IBuildManager
{
    public async Task<BuildResult> BuildProjectAsync(string projectPath)
    {
        var start = DateTime.UtcNow;
        var psi = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"build \"{projectPath}\" --nologo",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = psi };
        var output = new StringBuilder();
        var errors = new StringBuilder();
        process.OutputDataReceived += (s, e) => { if (e.Data != null) output.AppendLine(e.Data); };
        process.ErrorDataReceived += (s, e) => { if (e.Data != null) errors.AppendLine(e.Data); };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        await process.WaitForExitAsync();

        return new BuildResult
        {
            Success = process.ExitCode == 0,
            Output = output.ToString(),
            Errors = errors.ToString(),
            Duration = DateTime.UtcNow - start,
            ExitCode = process.ExitCode
        };
    }
}
