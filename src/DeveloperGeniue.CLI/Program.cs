using DeveloperGeniue.Core;

namespace DeveloperGeniue.CLI;

public class Program
{
    public static async Task Main(string[] args)
    {
        var config = new ConfigurationService();
        var lang = new LanguageService(config);

        if (args.Length >= 2 && (args[0] == "--lang" || args[0] == "lang"))
        {
            await lang.SetLanguageAsync(args[1]);
            Console.WriteLine(await lang.GetStringAsync("CLI.LanguageSet", args[1]));
            return;
        }

        if (args.Length == 0 || args[0].Equals("--help", StringComparison.OrdinalIgnoreCase))
        {

            Console.WriteLine(await lang.GetStringAsync("CLI.Header"));
            Console.WriteLine(await lang.GetStringAsync("CLI.Commands"));
            Console.WriteLine(await lang.GetStringAsync("CLI.Command.Scan"));
            Console.WriteLine(await lang.GetStringAsync("CLI.Command.Build"));
            Console.WriteLine(await lang.GetStringAsync("CLI.Command.Test"));
            Console.WriteLine(await lang.GetStringAsync("CLI.Command.UI"));

            return;
        }

        if (args[0].Equals("scan", StringComparison.OrdinalIgnoreCase))
        {
            var path = args.Length > 1 ? args[1] : Directory.GetCurrentDirectory();
            if (!Directory.Exists(path))
            {
                Console.WriteLine(await lang.GetStringAsync("CLI.MissingDirectory", path));
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
                Console.WriteLine(await lang.GetStringAsync("CLI.MissingProjectPath"));
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
                Console.WriteLine(await lang.GetStringAsync("CLI.MissingProjectPath"));
                return;
            }

            var manager = new TestManager();
            var result = await manager.RunTestsAsync(args[1]);
            return;
        }


        if (args[0].Equals("ui", StringComparison.OrdinalIgnoreCase))
        {
            await HybridHost.RunAsync();
            return;
        }

        Console.WriteLine(await lang.GetStringAsync("CLI.UnknownCommand"));

    }
}
