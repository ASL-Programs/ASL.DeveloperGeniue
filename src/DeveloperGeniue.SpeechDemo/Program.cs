using System.Threading.Tasks;
using System;
using DeveloperGeniue.AI.Speech;

namespace DeveloperGeniue.SpeechDemo;

internal class Program
{
    private static async Task Main()
    {
        ISpeechInterface speech = new SpeechInterface();
        var command = await speech.ListenForCommandAsync();
        Console.WriteLine($"You said: {command}");
    }
}
