using DeveloperGeniue.Core;

if (args.Length == 0 || args[0].Equals("--help", StringComparison.OrdinalIgnoreCase))
{
    Console.WriteLine("DeveloperGeniue CLI");
    Console.WriteLine("Commands:");
    Console.WriteLine("  scan [path]   - Discover projects in the specified path");
    return;
}

if (args[0].Equals("scan", StringComparison.OrdinalIgnoreCase))
{
    var path = args.Length > 1 ? args[1] : Directory.GetCurrentDirectory();
    if (!Directory.Exists(path))
    {
        Console.WriteLine($"Directory not found: {path}");
        return;
    }

    var manager = new ProjectManager();
    var projectFiles = manager.EnumerateProjectFiles(path);
    foreach (var projectFile in projectFiles)
    {
        var project = await manager.LoadProjectAsync(projectFile);
        Console.WriteLine($"{project.Name} - {project.Framework} - {project.Type}");
    }
    return;
}

Console.WriteLine("Unknown command. Use --help for usage.");
