using DeveloperGeniue.Core;

if (args.Length == 0 || args[0].Equals("--help", StringComparison.OrdinalIgnoreCase))
{
    Console.WriteLine("DeveloperGeniue CLI");
    Console.WriteLine("Commands:");

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
        return;
    }

    var manager = new BuildManager();
    var result = await manager.BuildProjectAsync(args[1]);
    Console.WriteLine(result.Output);

    return;
}

if (args[0].Equals("test", StringComparison.OrdinalIgnoreCase))
{
    if (args.Length < 2)
    {

        return;
    }

    var manager = new TestManager();
    var result = await manager.RunTestsAsync(args[1]);

    return;
}

Console.WriteLine("Unknown command. Use --help for usage.");
