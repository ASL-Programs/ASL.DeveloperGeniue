using System.Diagnostics;
using System.Text;

namespace DeveloperGeniue.Core;

public class BuildManager : IBuildManager
{
    public async Task<BuildResult> BuildProjectAsync(string projectPath)
    {
        process.OutputDataReceived += (s, e) => { if (e.Data != null) output.AppendLine(e.Data); };
        process.ErrorDataReceived += (s, e) => { if (e.Data != null) errors.AppendLine(e.Data); };
        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        return new BuildResult
        {
            Success = process.ExitCode == 0,
            Output = output.ToString(),
            Errors = errors.ToString(),
            ExitCode = process.ExitCode
        };
    }
}
