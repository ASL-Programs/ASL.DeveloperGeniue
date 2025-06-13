using System.Xml.Linq;
using System.IO;

namespace DeveloperGeniue.Core;

public class ProjectManager : IProjectManager
{
    public async Task<Project> LoadProjectAsync(string projectPath)
    {
        var project = new Project
        {
            Name = System.IO.Path.GetFileNameWithoutExtension(projectPath),
            Path = projectPath,
            Type = DetectProjectType(projectPath),
            Framework = DetectTargetFramework(projectPath)
        };

        return project;
    }

    public IEnumerable<string> EnumerateProjectFiles(string root)
    {
        foreach (var file in Directory.EnumerateFiles(root, "*.csproj", SearchOption.TopDirectoryOnly))
        {
            yield return file;
        }

        foreach (var dir in Directory.EnumerateDirectories(root))
        {
            var name = Path.GetFileName(dir);
            if (string.Equals(name, "bin", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(name, "obj", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(name, ".git", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            foreach (var file in EnumerateProjectFiles(dir))
                yield return file;
        }
    }

    public async Task<IEnumerable<CodeFile>> GetProjectFilesAsync(string projectPath)
    {
        var files = EnumerateProjectFiles(projectPath)

            .Select(async f => new CodeFile
            {
                Path = f,
                Content = await File.ReadAllTextAsync(f),
                Type = DetermineFileType(f),
                LastModified = File.GetLastWriteTime(f)
            });

        return await Task.WhenAll(files);
    }

    public IEnumerable<string> EnumerateProjectFiles(string rootPath)
    {
        var ignored = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "bin", "obj", ".git" };
        return Directory
            .EnumerateFiles(rootPath, "*.csproj", SearchOption.AllDirectories)
            .Where(f => !IsInIgnoredDirectory(f, ignored));
    }

    private static bool IsInIgnoredDirectory(string filePath, HashSet<string> ignored)
    {
        var dir = new FileInfo(filePath).Directory;
        while (dir != null)
        {
            if (ignored.Contains(dir.Name))
            {
                return true;
            }
            dir = dir.Parent;
        }
        return false;
    }

    private static ProjectType DetectProjectType(string projectPath)
    {
        var text = File.ReadAllText(projectPath);
        if (text.Contains("Sdk=\"Microsoft.NET.Sdk.Web\"", StringComparison.OrdinalIgnoreCase))
            return ProjectType.WebApi;
        if (text.Contains("Sdk=\"Microsoft.NET.Sdk.WinForms\"", StringComparison.OrdinalIgnoreCase))
            return ProjectType.WinForms;
        if (text.Contains("Blazor", StringComparison.OrdinalIgnoreCase))
            return ProjectType.Blazor;
        if (text.Contains("<OutputType>Exe</OutputType>", StringComparison.OrdinalIgnoreCase))
            return ProjectType.Console;
        return ProjectType.Library;
    }

    private static string DetectTargetFramework(string projectPath)
    {
        try
        {
            var doc = XDocument.Load(projectPath);
            var tf = doc.Descendants("TargetFramework").FirstOrDefault();
            return tf?.Value ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }

    private static CodeFileType DetermineFileType(string filePath)
    {
        return System.IO.Path.GetExtension(filePath).ToLowerInvariant() switch
        {
            ".cs" => CodeFileType.CSharp,
            ".csproj" => CodeFileType.Project,
            ".sln" => CodeFileType.Solution,
            ".json" => CodeFileType.Json,
            ".xml" => CodeFileType.Xml,
            ".resx" => CodeFileType.Resx,
            _ => CodeFileType.Other
        };
    }

}
