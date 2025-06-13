namespace DeveloperGeniue.Core;

public class TestResult
{
    public bool Success { get; set; }
    public int Total { get; set; }
    public int Passed { get; set; }
    public int Failed { get; set; }
    public TimeSpan Duration { get; set; }
}
