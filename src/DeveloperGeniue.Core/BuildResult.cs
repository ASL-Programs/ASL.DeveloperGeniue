namespace DeveloperGeniue.Core;

public class BuildResult
{
    public bool Success { get; set; }
    public string Output { get; set; } = string.Empty;
    public string Errors { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; }
    public int ExitCode { get; set; }
}
