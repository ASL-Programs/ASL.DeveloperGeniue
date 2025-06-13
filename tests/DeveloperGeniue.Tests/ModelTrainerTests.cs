using DeveloperGeniue.AI;
using DeveloperGeniue.AI.Speech;
using Xunit;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DeveloperGeniue.Tests;

public class ModelTrainerTests
{
    private class StubSpeech : ISpeechInterface
    {
        public Task<string> ListenForCommandAsync(System.Threading.CancellationToken cancellationToken = default)
            => Task.FromResult("data.csv");

        public Task SpeakAsync(string text, System.Threading.CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }

    [Fact]
    public async Task TrainModelWithVoiceUsesSpeechInput()
    {
        var speech = new StubSpeech();
        var trainer = new ModelTrainer();
        using var sw = new StringWriter();
        Console.SetOut(sw);
        await trainer.TrainModelWithVoiceAsync(speech);
        Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
        Assert.Contains("data.csv", sw.ToString());
    }
}
