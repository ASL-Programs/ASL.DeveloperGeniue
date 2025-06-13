namespace DeveloperGeniue.Core;

public interface IEvolutionTracker
{
    Task RecordAsync(EvolutionRecord record);
    Task<IReadOnlyList<EvolutionRecord>> GetRecordsAsync();
}
