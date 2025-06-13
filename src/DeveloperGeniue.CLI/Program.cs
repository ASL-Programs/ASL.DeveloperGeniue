using DeveloperGeniue.Core;
using DeveloperGeniue.Blockchain;
using DeveloperGeniue.Visualization;
using DeveloperGeniue.AI.Speech;
using System.Linq;

namespace DeveloperGeniue.CLI;

public class Program
{
    public static async Task Main(string[] args)
    {
        IConfigurationService config = args.Contains("--db") || Environment.GetEnvironmentVariable("USE_DB_CONFIG") == "1"
            ? new DatabaseConfigurationService()
            : new ConfigurationService();
        var lang = new LanguageService(config);
        await lang.GetUserLanguageAsync();
        Console.WriteLine($"Using language: {lang.CurrentLanguage}");

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
            Console.WriteLine(await lang.GetStringAsync("CLI.Command.Viz"));
            Console.WriteLine(await lang.GetStringAsync("CLI.Command.AR"));
            Console.WriteLine(await lang.GetStringAsync("CLI.Command.Provenance"));
            Console.WriteLine(await lang.GetStringAsync("CLI.Command.Evolution"));
            Console.WriteLine(await lang.GetStringAsync("CLI.Command.UI"));
            Console.WriteLine(await lang.GetStringAsync("CLI.Command.Train"));
            Console.WriteLine(await lang.GetStringAsync("CLI.Command.Voice"));

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

            Console.WriteLine(await lang.GetStringAsync("CLI.TestsPassed", result.PassedTests));
            Console.WriteLine(await lang.GetStringAsync("CLI.TestsFailed", result.FailedTests));
            Console.WriteLine(await lang.GetStringAsync("CLI.TestsSkipped", result.SkippedTests));
            Console.WriteLine(await lang.GetStringAsync("CLI.TestsTotal", result.TotalTests));
            Console.WriteLine(await lang.GetStringAsync("CLI.TestsDuration", result.Duration));

            if (!string.IsNullOrWhiteSpace(result.Errors))
            {
                Console.WriteLine(await lang.GetStringAsync("CLI.TestErrors"));
                Console.WriteLine(result.Errors);
            }

            return;
        }

        if (args[0].Equals("viz", StringComparison.OrdinalIgnoreCase))
        {
            if (args.Length < 2)
            {
                Console.WriteLine(await lang.GetStringAsync("CLI.MissingProjectPath"));
                return;
            }

            ICodeVisualizationService viz = new ThreeJsCodeVisualizationService();
            await viz.RenderAsync(args[1]);
            return;
        }

        if (args[0].Equals("ar", StringComparison.OrdinalIgnoreCase))
        {
            if (args.Length < 2)
            {
                Console.WriteLine(await lang.GetStringAsync("CLI.MissingProjectPath"));
                return;
            }

            IAugmentedRealityService ar = new AugmentedRealityService();
            await ar.StartCodeReviewAsync(args[1]);
            return;
        }

        if (args[0].Equals("provenance", StringComparison.OrdinalIgnoreCase))
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Missing commit hash.");
                return;
            }

            var registry = new BlockchainRegistry();
            await registry.RegisterCommitAsync(args[1]);
            Console.WriteLine("Commit registered to blockchain.");
            return;
        }

        if (args[0].Equals("evolution", StringComparison.OrdinalIgnoreCase))
        {
            var tracker = new EvolutionTracker();
            var service = new EvolutionAnalyticsService();
            var analytics = await service.ComputeAsync(tracker);
            Console.WriteLine($"Commits: {analytics.CommitCount}, {analytics.CommitsPerDay:F2} per day");
            return;
        }

        if (args[0].Equals("train", StringComparison.OrdinalIgnoreCase))
        {
            var trainer = new ModelTrainer();
            if (args.Length >= 2)
            {
                trainer.TrainModel(args[1]);
            }
            else
            {
                ISpeechInterface speech = new SpeechInterface();
                await trainer.TrainModelWithVoiceAsync(speech);
            }
            return;
        }

        if (args[0].Equals("voice", StringComparison.OrdinalIgnoreCase))
        {
            ISpeechInterface speech = new SpeechInterface();
            Console.WriteLine("Speak command:");
            var spoken = await speech.ListenForCommandAsync();
            var voiceArgs = spoken.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (voiceArgs.Length > 0)
                await Main(voiceArgs);
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
