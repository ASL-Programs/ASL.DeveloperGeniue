namespace DeveloperGeniue.Core;

public interface ICloudIntelligenceService
{
    Task<string> ExecuteAsync(string prompt, CancellationToken cancellationToken = default);
}
