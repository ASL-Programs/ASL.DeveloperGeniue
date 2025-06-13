namespace DeveloperGeniue.Core;

public class CodeFile
{
    public string Path { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public CodeFileType Type { get; set; }
    public DateTime LastModified { get; set; }
}
