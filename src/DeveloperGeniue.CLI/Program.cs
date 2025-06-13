using DeveloperGeniue.Core;

if (args.Length == 0 || args[0].Equals("--help", StringComparison.OrdinalIgnoreCase))
{
    Console.WriteLine("DeveloperGeniue CLI");
    Console.WriteLine("Commands:");
    Console.WriteLine("  scan [path]       - Discover projects in the specified path");
    Console.WriteLine("  build <path>      - Build the given solution or project");
    Console.WriteLine("  test <path>       - Run tests for the given project");
    Console.WriteLine("  config get <key>  - Display a configuration value");
    Console.WriteLine("  config set <key> <value> - Set a configuration value");
if (args[0].Equals("config", StringComparison.OrdinalIgnoreCase))
{
    if (args.Length < 3 || !(args[1].Equals("get", StringComparison.OrdinalIgnoreCase) || args[1].Equals("set", StringComparison.OrdinalIgnoreCase)))
    {
        Console.WriteLine("Usage: config get <key> | config set <key> <value>");
        return;
    }

    var service = new ConfigurationService();
    if (args[1].Equals("get", StringComparison.OrdinalIgnoreCase))
    {
        var value = await service.GetSettingAsync<string>(args[2]);
        Console.WriteLine(value ?? string.Empty);
    }
    else
    {
        if (args.Length < 4)
        {
            Console.WriteLine("Usage: config set <key> <value>");
            return;
        }
        await service.SetSettingAsync(args[2], args[3]);
        Console.WriteLine("Saved.");
    }
    return;
}

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
