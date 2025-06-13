namespace DeveloperGeniue.Core;

public interface ITestManager
{
    Task<TestResult> RunTestsAsync(string projectPath);
}
