using System;
using System.Threading;
using System.Threading.Tasks;
using DeveloperGeniue.AI.Speech;

namespace DeveloperGeniue.AI.Speech;

/// <summary>
/// Simple console-based implementation of <see cref="ISpeechInterface"/>.
/// In a real environment this would use a speech recognition library.
/// </summary>
public class SpeechInterface : ISpeechInterface
{
    public Task<string> ListenForCommandAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Say something (type input for demo):");
        string? input = Console.ReadLine();
        return Task.FromResult(input ?? string.Empty);
    }

    public Task SpeakAsync(string text, CancellationToken cancellationToken = default)
    {
        Console.WriteLine(text);
        return Task.CompletedTask;
    }
}
