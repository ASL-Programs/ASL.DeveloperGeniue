using System.Diagnostics;
using System.Text.RegularExpressions;

namespace DeveloperGeniue.Core;

public class TestManager : ITestManager
{
    public async Task<TestResult> RunTestsAsync(string projectPath)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"test \"{projectPath}\" --logger:trx --collect:\"XPlat Code Coverage\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            }
        };

        process.Start();
        var output = await process.StandardOutput.ReadToEndAsync();
        await process.WaitForExitAsync();

        return ParseTestResults(output);
    }

    private static TestResult ParseTestResults(string output)
    {
        var result = new TestResult { Output = output };

        var summary = Regex.Match(output, @"Failed:\s*(\d+),\s*Passed:\s*(\d+),\s*Skipped:\s*(\d+),\s*Total:\s*(\d+)");
        if (summary.Success)
        {
            result.FailedTests = int.Parse(summary.Groups[1].Value);
            result.PassedTests = int.Parse(summary.Groups[2].Value);
            result.SkippedTests = int.Parse(summary.Groups[3].Value);
            result.TotalTests = int.Parse(summary.Groups[4].Value);
        }

        var duration = Regex.Match(output, @"Test execution time: ([0-9.]+)");
        if (duration.Success && double.TryParse(duration.Groups[1].Value, out var seconds))
        {
            result.Duration = TimeSpan.FromSeconds(seconds);
        }

        result.Success = output.Contains("Test Run Successful");
        return result;
    }
}
