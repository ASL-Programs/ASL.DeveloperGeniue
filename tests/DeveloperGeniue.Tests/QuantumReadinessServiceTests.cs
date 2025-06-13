using DeveloperGeniue.Core;

namespace DeveloperGeniue.Tests;

public class QuantumReadinessServiceTests
{
    [Fact]
    public async Task AnalysisProducesReport()
    {
        var service = new QuantumReadinessService();
        var report = await service.AnalyzeQuantumCompatibilityAsync(1);
        Assert.NotNull(report);
    }

    [Fact]
    public async Task SuggestQuantumOptimizationsReturnsSuggestions()
    {
        var service = new QuantumReadinessService();
        var result = await service.SuggestQuantumOptimizationsAsync("sample code");
        Assert.NotEmpty(result.Suggestions);
    }
}
