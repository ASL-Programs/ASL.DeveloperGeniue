namespace DeveloperGeniue.Collaboration;

public interface IMergeConflictResolver
{
    Task<string> ResolveAsync(string baseCode, string incomingCode);
}

/// <summary>
/// Simple merge conflict resolver that concatenates conflicting code sections.
/// </summary>
public class SimpleMergeConflictResolver : IMergeConflictResolver
{
    public Task<string> ResolveAsync(string baseCode, string incomingCode)
    {
        if (string.Equals(baseCode, incomingCode, StringComparison.Ordinal))
            return Task.FromResult(baseCode);
        return Task.FromResult(baseCode + "\n" + incomingCode);
    }
}
