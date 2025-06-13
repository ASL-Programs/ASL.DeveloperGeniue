using System.Diagnostics;
using System.Text;

namespace DeveloperGeniue.Core;

public class BuildManager : IBuildManager
{
    public async Task<BuildResult> BuildProjectAsync(string projectPath)
        => await BuildProjectAsync(projectPath, CancellationToken.None);

    public async Task<BuildResult> BuildProjectAsync(string projectPath, CancellationToken cancellationToken)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"build \"{projectPath}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = psi };
        var output = new StringBuilder();
        var errors = new StringBuilder();

        process.OutputDataReceived += (s, e) => { if (e.Data != null) output.AppendLine(e.Data); };
        process.ErrorDataReceived += (s, e) => { if (e.Data != null) errors.AppendLine(e.Data); };

        var sw = Stopwatch.StartNew();
        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        await process.WaitForExitAsync(cancellationToken);
        sw.Stop();

        return new BuildResult
        {
            Success = process.ExitCode == 0,
            Output = output.ToString(),
            Errors = errors.ToString(),
            Duration = sw.Elapsed,
            ExitCode = process.ExitCode
        };
    }
}
