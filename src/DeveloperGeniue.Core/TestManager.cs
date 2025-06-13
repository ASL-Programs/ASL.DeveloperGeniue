using System.Diagnostics;
using System.Text.RegularExpressions;

namespace DeveloperGeniue.Core;

public class TestManager : ITestManager
{
    public async Task<TestResult> RunTestsAsync(string projectPath)
    {

        process.Start();
        var output = await process.StandardOutput.ReadToEndAsync();
        await process.WaitForExitAsync();

    }

    private static TestResult ParseTestResults(string output)
    {
        return result;
    }
}
