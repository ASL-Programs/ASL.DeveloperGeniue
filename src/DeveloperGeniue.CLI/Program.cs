using DeveloperGeniue.Core;

if (args.Length == 0 || args[0].Equals("--help", StringComparison.OrdinalIgnoreCase))
{
    Console.WriteLine("DeveloperGeniue CLI");
    Console.WriteLine("Commands:");
    Console.WriteLine("  scan [path]   - Discover projects in the specified path");
    Console.WriteLine("  build <path>  - Build the given solution or project");
    Console.WriteLine("  test <path>   - Run tests for the given project");
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
    var projectFiles = Directory.GetFiles(path, "*.csproj", SearchOption.AllDirectories);
    foreach (var projectFile in projectFiles)
    {
        var project = await manager.LoadProjectAsync(projectFile);
        Console.WriteLine($"{project.Name} - {project.Framework} - {project.Type}");
    }
    return;
}

if (args[0].Equals("build", StringComparison.OrdinalIgnoreCase))
{
    if (args.Length < 2)
    {
        Console.WriteLine("Please provide a project or solution path.");
        return;
    }

    var manager = new BuildManager();
    var result = await manager.BuildProjectAsync(args[1]);
    Console.WriteLine(result.Output);
    if (!result.Success)
    {
        Console.Error.WriteLine(result.Errors);
    }
    return;
}

if (args[0].Equals("test", StringComparison.OrdinalIgnoreCase))
{
    if (args.Length < 2)
    {
        Console.WriteLine("Please provide a test project path.");
        return;
    }

    var manager = new TestManager();
    var result = await manager.RunTestsAsync(args[1]);
    Console.WriteLine($"Passed {result.Passed}/{result.Total}");
    return;
}

Console.WriteLine("Unknown command. Use --help for usage.");
