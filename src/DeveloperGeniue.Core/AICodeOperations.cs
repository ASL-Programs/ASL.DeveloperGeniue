using DeveloperGeniue.Core.AI;

namespace DeveloperGeniue.Core;

/// <summary>
/// Wraps build and test operations and sends the results to AI for analysis.
/// </summary>
public class AICodeOperations
{
    private readonly BuildManager _buildManager;
    private readonly TestManager _testManager;
    private readonly AIOrchestrator _orchestrator;

    public AICodeOperations(BuildManager buildManager, TestManager testManager, AIOrchestrator orchestrator)
    {
        _buildManager = buildManager;
        _testManager = testManager;
        _orchestrator = orchestrator;
    }

    public async Task<BuildResult> BuildAndAnalyzeAsync(string projectPath)
    {
        var result = await _buildManager.BuildProjectAsync(projectPath);
        var prompt = $"Build output for {projectPath}:\n{result.Output}\nErrors:{result.Errors}";
        await _orchestrator.ExecuteAsync(new AIRequest(prompt, "OpenAI"));
        return result;
    }

    public async Task<TestResult> TestAndAnalyzeAsync(string projectPath)
    {
        var result = await _testManager.RunTestsAsync(projectPath);
        var prompt = $"Test results for {projectPath}: Passed {result.PassedTests}, Failed {result.FailedTests}.";
        await _orchestrator.ExecuteAsync(new AIRequest(prompt, "Claude"));
        return result;
    }
}
