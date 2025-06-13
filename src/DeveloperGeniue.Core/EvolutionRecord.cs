namespace DeveloperGeniue.Core;

/// <summary>
/// Represents a single commit entry for evolution tracking.
/// </summary>
public record EvolutionRecord(string CommitHash, DateTime Timestamp, string Message, string Author);
