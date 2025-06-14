using System.Xml.Linq;
using System.IO;
using System.Linq;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace DeveloperGeniue.Core;

public class ProjectManager : IProjectManager
{
    private readonly ILogger<ProjectManager> _logger;

    public ProjectManager() : this(Microsoft.Extensions.Logging.Abstractions.NullLogger<ProjectManager>.Instance)
    {
    }

    public ProjectManager(ILogger<ProjectManager> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Project> LoadProjectAsync(string projectPath)
        => await LoadProjectAsync(projectPath, CancellationToken.None);

    public async Task<Project> LoadProjectAsync(string projectPath, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Loading project {ProjectPath}", projectPath);
        var sw = Stopwatch.StartNew();
        var project = new Project
        {
            Name = System.IO.Path.GetFileNameWithoutExtension(projectPath),
            Path = projectPath,
            Type = DetectProjectType(projectPath),
            Framework = DetectTargetFramework(projectPath)
        };

        try
        {
            var dir = System.IO.Path.GetDirectoryName(projectPath) ?? System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(projectPath));
            if (!string.IsNullOrEmpty(dir))
            {
                var files = await GetProjectFilesAsync(dir, cancellationToken);
                project.Files.AddRange(files);
            }
        }
        catch (Exception ex)
        {
            // capture error information in LoadDuration as zero and continue
            project.Files.Clear();
            project.Framework = string.Empty;
            _logger.LogError(ex, "Failed to load project {ProjectPath}", projectPath);
        }

        sw.Stop();
        project.LoadDuration = sw.Elapsed;
        _logger.LogInformation("Project loaded in {Duration}", project.LoadDuration);
        return project;
    }


    public IEnumerable<string> EnumerateProjectFiles(string root)
        => EnumerateProjectFiles(root, new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "bin", "obj", ".git" });

    private static IEnumerable<string> EnumerateProjectFiles(string rootPath, HashSet<string> ignored)
    {
        return Directory.EnumerateFiles(rootPath, "*.csproj", SearchOption.AllDirectories)
            .Where(f => !IsInIgnoredDirectory(f, ignored));
    }

    public async Task<IEnumerable<string>> EnumerateProjectFilesAsync(string rootPath, CancellationToken cancellationToken)
    {
        return await Task.Run(() => EnumerateProjectFiles(rootPath), cancellationToken);
    }


    public async Task<IEnumerable<CodeFile>> GetProjectFilesAsync(string projectPath)
        => await GetProjectFilesAsync(projectPath, CancellationToken.None);

    public async Task<IEnumerable<CodeFile>> GetProjectFilesAsync(string projectPath, CancellationToken cancellationToken)
    {
        var files = EnumerateProjectFiles(projectPath)
            .Select(async f => new CodeFile
            {
                Path = f,
                Content = await File.ReadAllTextAsync(f, cancellationToken),
                Type = DetermineFileType(f),
                LastModified = File.GetLastWriteTime(f)
            });

        return await Task.WhenAll(files);
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
