namespace DeveloperGeniue.Core;

public class Project
{
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public ProjectType Type { get; set; } = ProjectType.Unknown;
    public string Framework { get; set; } = string.Empty;
    public List<CodeFile> Files { get; set; } = new();
}
