using System.Diagnostics;
using System.Text;

namespace DeveloperGeniue.Core;

public class BuildManager : IBuildManager
{
    public async Task<BuildResult> BuildProjectAsync(string projectPath)
    {
        var startTime = DateTime.UtcNow;
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"build \"{projectPath}\" --verbosity normal",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };

        var output = new StringBuilder();
        var errors = new StringBuilder();



        process.OutputDataReceived += (s, e) => { if (e.Data != null) output.AppendLine(e.Data); };
        process.ErrorDataReceived += (s, e) => { if (e.Data != null) errors.AppendLine(e.Data); };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        await process.WaitForExitAsync();

        var duration = DateTime.UtcNow - startTime;

        return new BuildResult
        {
            Success = process.ExitCode == 0,
            Output = output.ToString(),
            Errors = errors.ToString(),
            Duration = duration,
            ExitCode = process.ExitCode
        };
    }
}
