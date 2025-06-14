namespace DeveloperGeniue.Core;

public interface ICloudIntelligenceService
{
    Task<string> ExecuteAsync(string prompt, CancellationToken cancellationToken = default);
    Task<string> AnalyzeCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<string> TrainModelAsync(string trainingData, CancellationToken cancellationToken = default);
}
