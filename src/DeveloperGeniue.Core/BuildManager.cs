using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Logging;

namespace DeveloperGeniue.Core;

public class BuildManager : IBuildManager
{
    private readonly ILogger<BuildManager> _logger;

    public BuildManager()
        : this(Microsoft.Extensions.Logging.Abstractions.NullLogger<BuildManager>.Instance)
    {
    }

    public BuildManager(ILogger<BuildManager> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<BuildResult> BuildProjectAsync(string projectPath)
        => await BuildProjectAsync(projectPath, CancellationToken.None);

    public async Task<BuildResult> BuildProjectAsync(string projectPath, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Building project {ProjectPath}", projectPath);
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
        try
        {
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            await process.WaitForExitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            errors.AppendLine(ex.Message);
            _logger.LogError(ex, "Build failed for {ProjectPath}", projectPath);
            return new BuildResult
            {
                Success = false,
                Output = output.ToString(),
                Errors = errors.ToString(),
                Duration = sw.Elapsed,
                ExitCode = process.ExitCode
            };
        }
        finally
        {
            sw.Stop();
        }

        var result = new BuildResult
        {
            Success = process.ExitCode == 0,
            Output = output.ToString(),
            Errors = errors.ToString(),
            Duration = sw.Elapsed,
            ExitCode = process.ExitCode
        };
        _logger.LogInformation("Build completed with exit code {ExitCode} in {Duration}", result.ExitCode, result.Duration);
        return result;
    }
}
