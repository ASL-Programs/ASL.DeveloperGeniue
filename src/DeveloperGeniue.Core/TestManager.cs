using System.Diagnostics;
using System.Text.RegularExpressions;

namespace DeveloperGeniue.Core;

public class TestManager : ITestManager
{
    public async Task<TestResult> RunTestsAsync(string projectPath)
    {
        var start = DateTime.UtcNow;
        var psi = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"test \"{projectPath}\" --no-build --verbosity quiet",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = psi };
        process.Start();
        var output = await process.StandardOutput.ReadToEndAsync();
        await process.WaitForExitAsync();

        var result = ParseTestResults(output);
        result.Success = process.ExitCode == 0;
        result.Duration = DateTime.UtcNow - start;
        return result;
    }

    private static TestResult ParseTestResults(string output)
    {
        var result = new TestResult();
        var match = Regex.Match(output,
            @"Total tests: (?<total>\d+).+Passed: (?<passed>\d+).+Failed: (?<failed>\d+)",
            RegexOptions.Singleline);
        if (match.Success)
        {
            result.Total = int.Parse(match.Groups["total"].Value);
            result.Passed = int.Parse(match.Groups["passed"].Value);
            result.Failed = int.Parse(match.Groups["failed"].Value);
        }
        return result;
    }
}
