using DeveloperGeniue.Core;

if (args.Length == 0 || args[0].Equals("--help", StringComparison.OrdinalIgnoreCase))
{
    Console.WriteLine("DeveloperGeniue CLI");
    Console.WriteLine("Commands:");
    Console.WriteLine("  scan [path]   - Discover projects in the specified path");
    Console.WriteLine("  build <csproj> - Build the specified project");
    Console.WriteLine("  test <csproj>  - Run tests for the specified project");
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

if (args[0].Equals("build", StringComparison.OrdinalIgnoreCase))
{
    if (args.Length < 2)
    {
        Console.WriteLine("Project file required.");
        return;
    }

    var manager = new BuildManager();
    var result = await manager.BuildProjectAsync(args[1]);
    Console.WriteLine(result.Output);

    if (!string.IsNullOrWhiteSpace(result.Errors))
        Console.WriteLine(result.Errors);
    Environment.ExitCode = result.ExitCode;

    return;
}

if (args[0].Equals("test", StringComparison.OrdinalIgnoreCase))
{
    if (args.Length < 2)
    {
        Console.WriteLine("Project file required.");
        return;
    }

    var manager = new TestManager();
    var result = await manager.RunTestsAsync(args[1]);

    Console.WriteLine(result.Output);
    Environment.ExitCode = result.Success ? 0 : 1;
    return;
}

Console.WriteLine("Unknown command. Use --help for usage.");
