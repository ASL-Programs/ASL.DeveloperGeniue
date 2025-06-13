namespace DeveloperGeniue.Core;

/// <summary>
/// Represents analytics about the code evolution history.
/// </summary>
public record EvolutionAnalytics(
    int CommitCount,
    DateTime FirstCommit,
    DateTime LastCommit,
    double CommitsPerDay,
    IReadOnlyDictionary<string, int> CommitsPerAuthor);
