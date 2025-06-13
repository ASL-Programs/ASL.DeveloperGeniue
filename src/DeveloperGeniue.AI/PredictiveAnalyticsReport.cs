namespace DeveloperGeniue.AI;

/// <summary>
/// Represents analytics metrics and insights for a project.
/// </summary>
public record PredictiveAnalyticsReport(
    int TotalFiles,
    double AverageLinesPerFile,
    bool BuildSucceeded,
    double TestPassRate,
    string Summary,
    string Trend);
