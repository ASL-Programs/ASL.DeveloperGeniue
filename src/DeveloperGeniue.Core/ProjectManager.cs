using System.Xml.Linq;
using System.IO;
using System.Collections.Generic;
using System.Linq;


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

        await ScanProjectFilesAsync(project);
        await AnalyzeDependenciesAsync(project);

        return project;
    }

    public async Task<IEnumerable<CodeFile>> GetProjectFilesAsync(string projectPath)
    {
        var allowedExtensions = new[] { ".cs", ".csproj", ".sln", ".json", ".xml", ".resx" };
        var files = Directory.GetFiles(projectPath, "*.*", SearchOption.AllDirectories)
            .Where(f => allowedExtensions.Contains(System.IO.Path.GetExtension(f))
                && !IsInIgnoredDirectory(f))

            .Select(async f => new CodeFile
            {
                Path = f,
                Content = await File.ReadAllTextAsync(f),
                Type = DetermineFileType(f),
                LastModified = File.GetLastWriteTime(f)
            });

        return await Task.WhenAll(files);
    }

    public async Task ScanProjectFilesAsync(Project project)
    {
        var dir = System.IO.Path.GetDirectoryName(project.Path) ?? project.Path;
        project.Files = (await GetProjectFilesAsync(dir)).ToList();
    }

    public Task AnalyzeDependenciesAsync(Project project)
    {
        try
        {
            var doc = XDocument.Load(project.Path);
            var deps = doc.Descendants("PackageReference")
                .Select(e => e.Attribute("Include")?.Value)
                .Where(v => !string.IsNullOrWhiteSpace(v))
                .Distinct()
                .ToList();
            project.Dependencies = deps;
        }
        catch
        {
            project.Dependencies = new List<string>();
        }

        return Task.CompletedTask;
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

    private static readonly string[] _ignoredDirs = new[] { "bin", "obj", ".git" };

    private static bool IsInIgnoredDirectory(string filePath)
    {
        var parts = filePath.Split(Path.DirectorySeparatorChar);
        return parts.Any(p => _ignoredDirs.Contains(p, StringComparer.OrdinalIgnoreCase));
    }
}
