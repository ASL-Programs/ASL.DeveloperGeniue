using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace DeveloperGeniue.Core;

public class TestManager : ITestManager
{
    private readonly ILogger<TestManager> _logger;

    public TestManager() : this(Microsoft.Extensions.Logging.Abstractions.NullLogger<TestManager>.Instance)
    {
    }

    public TestManager(ILogger<TestManager> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<TestResult> RunTestsAsync(string projectPath)
    {
        _logger.LogInformation("Running tests for {ProjectPath}", projectPath);
        var startTime = DateTime.UtcNow;
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"test \"{projectPath}\" --logger:trx",
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

        try
        {
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            await process.WaitForExitAsync();
        }
        catch (Exception ex)
        {
            errors.AppendLine(ex.Message);
            _logger.LogError(ex, "Tests failed to run for {ProjectPath}", projectPath);
            return new TestResult
            {
                Success = false,
                Output = output.ToString(),
                Errors = errors.ToString(),
                Duration = DateTime.UtcNow - startTime
            };
        }

        var duration = DateTime.UtcNow - startTime;
        var outputText = output.ToString();
        var errorsText = errors.ToString();
        var result = ParseTestResults(outputText);
        result.Success = process.ExitCode == 0;
        result.Duration = duration;
        result.Output = outputText;
        result.Errors = errorsText;
        _logger.LogInformation("Test run completed with exit code {ExitCode} in {Duration}", process.ExitCode, duration);
        return result;
    }

    private static TestResult ParseTestResults(string output)
    {
        var result = new TestResult();

        var summaryRegex = new Regex(@"Failed:\s*(\d+),\s*Passed:\s*(\d+),\s*Skipped:\s*(\d+),\s*Total:\s*(\d+),\s*Duration:\s*([^\r\n]+)", RegexOptions.IgnoreCase);
        var match = summaryRegex.Match(output);
        if (match.Success)
        {
            result.FailedTests = int.Parse(match.Groups[1].Value);
            result.PassedTests = int.Parse(match.Groups[2].Value);
            result.SkippedTests = int.Parse(match.Groups[3].Value);
            result.TotalTests = int.Parse(match.Groups[4].Value);
            result.Duration = ParseDuration(match.Groups[5].Value.Trim());
            return result;
        }

        var altRegex = new Regex(@"Total tests:\s*(\d+).*?Passed:\s*(\d+).*?Failed:\s*(\d+).*?Skipped:\s*(\d+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        var altMatch = altRegex.Match(output);
        if (altMatch.Success)
        {
            result.TotalTests = int.Parse(altMatch.Groups[1].Value);
            result.PassedTests = int.Parse(altMatch.Groups[2].Value);
            result.FailedTests = int.Parse(altMatch.Groups[3].Value);
            result.SkippedTests = int.Parse(altMatch.Groups[4].Value);
        }

        var timeRegex = new Regex(@"Test execution time:\s*([^\r\n]+)", RegexOptions.IgnoreCase);
        var timeMatch = timeRegex.Match(output);
        if (timeMatch.Success)
        {
            result.Duration = ParseDuration(timeMatch.Groups[1].Value.Trim());
        }

        return result;
    }

    private static TimeSpan ParseDuration(string input)
    {
        var match = Regex.Match(input, @"([0-9]+(?:\.[0-9]+)?)\s*(ms|s|sec|seconds|m|min|minutes)?", RegexOptions.IgnoreCase);
        if (!match.Success)
        {
            TimeSpan.TryParse(input, out var ts);
            return ts;
        }

        var value = double.Parse(match.Groups[1].Value, System.Globalization.CultureInfo.InvariantCulture);
        var unit = match.Groups[2].Value.ToLowerInvariant();
        return unit switch
        {
            "ms" => TimeSpan.FromMilliseconds(value),
            "s" or "sec" or "seconds" => TimeSpan.FromSeconds(value),
            "m" or "min" or "minutes" => TimeSpan.FromMinutes(value),
            _ => TimeSpan.FromSeconds(value)
        };
    }
}
