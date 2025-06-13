namespace DeveloperGeniue.Core;

public interface IQuantumReadinessService
{
    Task<QuantumCompatibilityReport> AnalyzeQuantumCompatibilityAsync(int projectId);
    Task<QuantumAlgorithmSuggestions> SuggestQuantumOptimizationsAsync(string code);
    Task<PostQuantumSecurityReport> AnalyzePostQuantumSecurityAsync(string code);
    Task<QuantumSimulationResult> SimulateQuantumPerformanceAsync(string algorithmCode);
}
